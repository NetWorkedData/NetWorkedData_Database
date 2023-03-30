using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//=====================================================================================================================
using Sqlite3DatabaseHandle = System.IntPtr;
using static NetWorkedData.SQLite3;
using UnityEngine;
//=====================================================================================================================
#if UNITY_EDITOR
using Sqlite = NetWorkedData.Logged.SQLite3; // Have a logged interface for SQLite (Editor only !)
#else
using Sqlite = NetWorkedData.SQLite3;
#endif
//=====================================================================================================================
namespace NetWorkedData.NWDORM
{
    public class DatabaseFactory : IDatabaseFactory, IPreparedStatementFactory
    {
        static public DatabaseFactory CreateEditorDatabase(string sPath)
        {
            return new DatabaseFactory
            {
                key = null,
                name = "Editor",
                path = GetNullTerminatedUTF8(sPath)
            };
        }

        static public DatabaseFactory CreateDeviceDatabase(string sPath)
        {
            return new DatabaseFactory
            {
                key = null,
                name = "Device",
                path = GetNullTerminatedUTF8(sPath)
            };
        }

        static private byte[] GetNullTerminatedUTF8 (string s)
        {
            int utf8Length = Encoding.UTF8.GetByteCount(s);
            byte[] bytes = new byte[utf8Length + 1];
            Encoding.UTF8.GetBytes(s, 0, s.Length, bytes, 0);
            return bytes;
        }

        internal string name;
        internal byte[] path;
        internal string key;

        internal List<string> pragmas = new List<string>();

        internal Database staticDatabase = null;

        public void Key (string sKey)
        {
            key = sKey;
        }

        public void Pragma (string sPragma)
        {
            pragmas.Add(sPragma);
        }

        public Database CreateDatabase()
        {
            if (staticDatabase != null)
            {
                return staticDatabase;
            }
            return new Database(this);
        }

        public StaticDatabase StartStaticDatabase ()
        {
            return new StaticDatabase(this);
        }

        public ITransaction CreateTransaction ()
        {
            return new Transaction(CreateDatabase());
        }

        public IPrepareStatement CreatePrepareStatement(string sQuery)
        {
            return new PrepareStatement (CreateDatabase(), sQuery);
        }

        public Result Exec(string sQuery)
        {
            using (PrepareStatement tStatement = new PrepareStatement(CreateDatabase(), sQuery))
            {
                return tStatement.Step();
            }
        }
    }

    internal struct PrepareStatement : IPrepareStatement
    {
        Database db;
        bool managesDatabase;
        string query;
        Sqlite3DatabaseHandle handle;

        public string Query => query;
        public Sqlite3DatabaseHandle Handle => handle;

        public PrepareStatement(Database sDB, string sQuery)
        {
            db = sDB;
            managesDatabase = false;
            query = sQuery;
            handle = IntPtr.Zero;
        }

        public Result Step()
        {
            if (!db.IsOpened)
            {
                db.Open();
                managesDatabase = !db.isStatic;
            }

            if (handle == IntPtr.Zero)
            {
                handle = Sqlite.Prepare2(db.handle, query);
            }

            return Sqlite.Step(handle);
        }

        public void Dispose()
        {
            Sqlite.Finalize(handle);

            if (managesDatabase)
            {
                db.Dispose();
                managesDatabase = false;
            }
        }


        static public implicit operator Sqlite3DatabaseHandle(PrepareStatement stmt)
        {
            return stmt.Handle;
        }
    }

    internal struct Transaction : ITransaction
    {
        Database db;
        bool managesDatabase;
        Sqlite3DatabaseHandle handle;

        public Transaction(Database sDB)
        {
            db = sDB;
            handle = IntPtr.Zero;
            managesDatabase = false;
        }

        public void Begin()
        {
            if (handle == IntPtr.Zero)
            {
                if (!db.IsOpened)
                {
                    db.Open();
                    managesDatabase = !db.isStatic;
                }

                try
                {
                    handle = Sqlite.Prepare2(db.handle, "BEGIN TRANSACTION;");
                    Sqlite.Step(handle);
                }
                catch (Exception)
                {
                    if (handle != IntPtr.Zero)
                    {
                        Sqlite.Finalize(handle);
                        handle = IntPtr.Zero;
                    }
                    throw;
                }
                Sqlite.Finalize(handle);
            }
        }

        public void Commit()
        {
            if (handle != IntPtr.Zero)
            {
                try
                {
                    handle = Sqlite.Prepare2(db.handle, "COMMIT;");
                    Sqlite.Step(handle);
                }
                finally
                {
                    Sqlite.Finalize(handle);
                    handle = IntPtr.Zero;
                }
            }
        }

        public void Rollback()
        {
            if (handle != IntPtr.Zero)
            {
                try
                {
                    handle = Sqlite.Prepare2(db.handle, "ROLLBACK;");
                    Sqlite.Step(handle);
                }
                finally
                {
                    Sqlite.Finalize(handle);
                    handle = IntPtr.Zero;
                }
            }
        }

        public void Dispose()
        {
            Rollback();
            if (managesDatabase)
            {
                db.Dispose();
                managesDatabase = false;
            }
        }

        public IPrepareStatement CreatePrepareStatement(string sQuery)
        {
            return new PrepareStatement(db, sQuery);
        }

        public Result Exec(string sQuery)
        {
            using (PrepareStatement tStatement = new PrepareStatement(db, sQuery))
            {
                return tStatement.Step();
            }
        }
    }

    public struct StaticDatabase : IDisposable
    {
        DatabaseFactory factory;

        public StaticDatabase (DatabaseFactory sFactory)
        {
            if (sFactory.staticDatabase == null)
            {
                factory = sFactory;
                factory.staticDatabase = factory.CreateDatabase();
                factory.staticDatabase.isStatic = true;
            }
            else
            {
                factory = null;
            }
        }

        public void Dispose()
        {
            if (factory != null && factory.staticDatabase != null)
            {
                factory.staticDatabase.Dispose();
                factory.staticDatabase = null;
            }
        }
    }

    public class Database : IDisposable
    {
        internal DatabaseFactory factory;
        internal Sqlite3DatabaseHandle handle;
        public bool isStatic;

        public Database(DatabaseFactory sFactory)
        {
            factory = sFactory;
            isStatic = false;
            handle = IntPtr.Zero;
        }

        public bool IsOpened => handle != IntPtr.Zero;

        public Result Open()
        {
            Result tResult = Sqlite.Open(factory.path, out handle, (int)(SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create), IntPtr.Zero);

            if (!string.IsNullOrEmpty(factory.key))
            {
                tResult = Sqlite.Key(handle, factory.key, factory.key.Length);
            }

            foreach (string tPragma in factory.pragmas)
            {
                using (PrepareStatement tStatement = new PrepareStatement(this, tPragma))
                {
                    tStatement.Step();
                }
            }

            return tResult;
        }

        public Result Close()
        {
            Result tResult = Result.OK;

            if (handle != IntPtr.Zero)
            {
                tResult = Sqlite.Close(handle);
                handle = IntPtr.Zero;
            }

            return tResult;
        }

        public void Dispose()
        {
            Close();
        }

        ~Database()
        {
            Dispose();
        }
    }

    public interface IPrepareStatement : IDisposable
    {
        public Sqlite3DatabaseHandle Handle { get; }
        public Result Step();
    }

    public interface ITransaction : IPreparedStatementFactory, IDisposable
    {
        public void Begin();
        public void Commit();
        public void Rollback();
    }

    public interface IPreparedStatementFactory
    {
        public IPrepareStatement CreatePrepareStatement(string sQuery);

        public Result Exec(string sQuery);
    }

    public interface IDatabaseFactory
    {
        public Database CreateDatabase();
    }
}
