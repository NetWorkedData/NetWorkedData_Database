using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//=====================================================================================================================
using Sqlite3DatabaseHandle = System.IntPtr;
using static NetWorkedData.SQLite3;
//=====================================================================================================================
#if UNITY_EDITOR
using Sqlite = NetWorkedData.Logged.SQLite3; // Have a logged interface for SQLite (Editor only !)
#else
using Sqlite = NetWorkedData.SQLite3;
#endif
//=====================================================================================================================
namespace NetWorkedData
{
    public static class NWDORM
    {
        public struct PrepareStatement : IDisposable
        {
            Sqlite3DatabaseHandle db;
            string query;
            Sqlite3DatabaseHandle handle;

            public string Query => query;
            public Sqlite3DatabaseHandle Handle => handle;

            public PrepareStatement (Sqlite3DatabaseHandle sDB, string sQuery)
            {
                db = sDB;
                query = sQuery;
                handle = IntPtr.Zero;
            }

            public Result Step ()
            {
                if (handle == IntPtr.Zero)
                {
                    handle = SQLite3.Prepare2 (db, query);
                }

                return SQLite3.Step (handle);
            }

            public void Dispose()
            {
                SQLite3.Finalize (handle);
            }

            static public implicit operator Sqlite3DatabaseHandle(PrepareStatement stmt)
            {
                return stmt.Handle;
            }
        }

        public struct Transaction : IDisposable
        {
            Sqlite3DatabaseHandle db;
            Sqlite3DatabaseHandle handle;

            public Transaction (Sqlite3DatabaseHandle sDB)
            {
                this.db = sDB;
                handle = IntPtr.Zero;
            }

            public void Begin ()
            {
                if (handle == IntPtr.Zero)
                {
                    try
                    {
                        handle = SQLite3.Prepare2(db, "BEGIN TRANSACTION;");
                        SQLite3.Step(handle);
                    }
                    catch (Exception)
                    {
                        if (handle != IntPtr.Zero)
                        {
                            SQLite3.Finalize(handle);
                            handle = IntPtr.Zero;
                        }
                        throw;
                    }
                }
            }

            public void Commit ()
            {
                if (handle != IntPtr.Zero)
                {
                    try
                    {
                        handle = SQLite3.Prepare2(db, "COMMIT;");
                        SQLite3.Step(handle);
                    }
                    finally
                    {
                        SQLite3.Finalize(handle);
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
                        handle = SQLite3.Prepare2(db, "ROLLBACK;");
                        SQLite3.Step(handle);
                    }
                    finally
                    {
                        SQLite3.Finalize(handle);
                        handle = IntPtr.Zero;
                    }
                }
            }

            public void Dispose()
            {
                Rollback();
            }
        }
    }
}
