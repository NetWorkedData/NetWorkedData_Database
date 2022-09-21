//=====================================================================================================================
//
//  ideMobi 2020©
//  All rights reserved by ideMobi
//
//=====================================================================================================================

using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using static NetWorkedData.SQLite3;

//=====================================================================================================================
using Sqlite3DatabaseHandle = System.IntPtr;
//=====================================================================================================================
namespace NetWorkedData.Logged
{
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    public static partial class SQLite3
    {
        //-------------------------------------------------------------------------------------------------------------
        [Flags]
        public enum LogLevel : byte
        {
            None = 0,
            Ok = 1 << 0,
            Error = 1 << 1
        }
        //-------------------------------------------------------------------------------------------------------------
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    public static partial class Sqlite3
    {
        static private readonly SQLite3.LogLevel defaultLogLevel = SQLite3.LogLevel.Error;

        private static Result Logger(Result sResult, string sMethod, SQLite3.LogLevel sLogLevel)
        {
            switch (sResult)
            {
                case Result.OK:
                case Result.Row:
                case Result.Done:
                    if (sLogLevel.HasFlag(SQLite3.LogLevel.Ok))
                    {
                        Log(sResult.ToString(), sMethod);
                    }
                    break;
                default:
                    if (sLogLevel.HasFlag(SQLite3.LogLevel.Error))
                    {
                        Log(sResult.ToString(), sMethod);
                    }
                    break;
            }
            return sResult;
        }

        private static int Logger(int sResult, string sMethod, bool sLogged = false)
        {
            if (sLogged)
            {
                Log(sResult.ToString(), sMethod);
            }
            return sResult;
        }

        private static long Logger(long sResult, string sMethod, bool sLogged = false)
        {
            if (sLogged)
            {
                Log(sResult.ToString(), sMethod);
            }
            return sResult;
        }

        private static double Logger(double sResult, string sMethod, bool sLogged = false)
        {
            if (sLogged)
            {
                Log(sResult.ToString(), sMethod);
            }
            return sResult;
        }

        private static ColType Logger(ColType sResult, string sMethod, bool sLogged = false)
        {
            if (sLogged)
            {
                Log(sResult.ToString(), sMethod);
            }
            return sResult;
        }

        private static ExtendedResult Logger(ExtendedResult sResult, string sMethod, bool sLogged = false)
        {
            if (sLogged)
            {
                Log(sResult.ToString(), sMethod);
            }
            return sResult;
        }


        private static string Logger(string sResult, string sMethod, bool sLogged = false)
        {
            if (sLogged)
            {
                Log(sResult, sMethod);
            }
            return sResult;
        }

        private static void Log (string sResult, string sMethod)
        {
            Debug.Log(sMethod + " returned `" + sResult + "`");
        }

        public static Result Open([MarshalAs(UnmanagedType.LPStr)] string filename, out Sqlite3DatabaseHandle db)
        {
            return Logger(NetWorkedData.SQLite3.Open(filename, out db), nameof(Open), defaultLogLevel);
        }

        public static Result Open([MarshalAs(UnmanagedType.LPStr)] string filename, out Sqlite3DatabaseHandle db, int flags, Sqlite3DatabaseHandle zvfs)
        {
            return Logger(NetWorkedData.SQLite3.Open(filename, out db, flags, zvfs), nameof(Open), defaultLogLevel);
        }

        public static Result Open(byte[] filename, out Sqlite3DatabaseHandle db, int flags, Sqlite3DatabaseHandle zvfs)
        {
            return Logger(NetWorkedData.SQLite3.Open(filename, out db, flags, zvfs), nameof(Open), defaultLogLevel);
        }

        public static Result Key(Sqlite3DatabaseHandle db, [MarshalAs(UnmanagedType.LPStr)] string key, int keylen)
        {
            return Logger(NetWorkedData.SQLite3.Key(db, key, keylen), nameof(Key), defaultLogLevel);
        }

        public static Result Open16([MarshalAs(UnmanagedType.LPWStr)] string filename, out Sqlite3DatabaseHandle db)
        {
            return Logger(NetWorkedData.SQLite3.Open16(filename, out db), nameof(Open16), defaultLogLevel);
        }

        public static Result EnableLoadExtension(Sqlite3DatabaseHandle db, int onoff)
        {
            return Logger(NetWorkedData.SQLite3.EnableLoadExtension(db, onoff), nameof(EnableLoadExtension), defaultLogLevel);
        }

        public static Result Close(Sqlite3DatabaseHandle db)
        {
            return Logger(NetWorkedData.SQLite3.Close(db), nameof(Close), defaultLogLevel);
        }

        public static Result Initialize()
        {
            return Logger(NetWorkedData.SQLite3.Initialize(), nameof(Initialize), defaultLogLevel);
        }

        public static Result Shutdown()
        {
            return Logger(NetWorkedData.SQLite3.Shutdown(), nameof(Shutdown), defaultLogLevel);
        }

        public static Result Config(ConfigOption option)
        {
            return Logger(NetWorkedData.SQLite3.Config(option), nameof(Config), defaultLogLevel);
        }

        public static Result BusyTimeout(Sqlite3DatabaseHandle db, int milliseconds)
        {
            return Logger(NetWorkedData.SQLite3.BusyTimeout(db, milliseconds), nameof(BusyTimeout), defaultLogLevel);
        }

        public static int Changes(Sqlite3DatabaseHandle db)
        {
            return Logger(NetWorkedData.SQLite3.Changes(db), nameof(Changes));
        }

        public static Result Prepare2(Sqlite3DatabaseHandle db, [MarshalAs(UnmanagedType.LPStr)] string sql, int numBytes, out Sqlite3DatabaseHandle stmt, Sqlite3DatabaseHandle pzTail)
        {
            return Logger(NetWorkedData.SQLite3.Prepare2(db, sql, numBytes, out stmt, pzTail), nameof(Prepare2), defaultLogLevel);
        }

        public static Result Step(Sqlite3DatabaseHandle stmt)
        {
            return Logger(NetWorkedData.SQLite3.Step(stmt), nameof(Step), defaultLogLevel);
        }

        public static Result Reset(Sqlite3DatabaseHandle stmt)
        {
            return Logger(NetWorkedData.SQLite3.Reset(stmt), nameof(Reset), defaultLogLevel);
        }

        public static Result Finalize(Sqlite3DatabaseHandle stmt)
        {
            return Logger(NetWorkedData.SQLite3.Finalize(stmt), nameof(Finalize), defaultLogLevel);
        }

        public static long LastInsertRowid(Sqlite3DatabaseHandle db)
        {
            return Logger(NetWorkedData.SQLite3.LastInsertRowid(db), nameof(LastInsertRowid));
        }

        public static Sqlite3DatabaseHandle Errmsg(Sqlite3DatabaseHandle db)
        {
            return NetWorkedData.SQLite3.Errmsg(db);
        }

        public static int BindParameterIndex(Sqlite3DatabaseHandle stmt, [MarshalAs(UnmanagedType.LPStr)] string name)
        {
            return Logger(NetWorkedData.SQLite3.BindParameterIndex(stmt, name), nameof(BindParameterIndex));
        }

        public static int BindNull(Sqlite3DatabaseHandle stmt, int index)
        {
            return Logger(NetWorkedData.SQLite3.BindNull(stmt, index), nameof(BindNull));
        }

        public static int BindInt(Sqlite3DatabaseHandle stmt, int index, int val)
        {
            return Logger(NetWorkedData.SQLite3.BindInt(stmt, index, val), nameof(BindInt));
        }

        public static int BindInt64(Sqlite3DatabaseHandle stmt, int index, long val)
        {
            return Logger(NetWorkedData.SQLite3.BindInt64(stmt, index, val), nameof(BindInt64));
        }

        public static int BindDouble(Sqlite3DatabaseHandle stmt, int index, double val)
        {
            return Logger(NetWorkedData.SQLite3.BindDouble(stmt, index, val), nameof(BindDouble));
        }

        public static int BindText(Sqlite3DatabaseHandle stmt, int index, [MarshalAs(UnmanagedType.LPWStr)] string val, int n, Sqlite3DatabaseHandle free)
        {
            return Logger(NetWorkedData.SQLite3.BindText(stmt, index, val, n, free), nameof(BindText));
        }

        public static int BindBlob(Sqlite3DatabaseHandle stmt, int index, byte[] val, int n, Sqlite3DatabaseHandle free)
        {
            return Logger(NetWorkedData.SQLite3.BindBlob(stmt, index, val, n, free), nameof(BindBlob));
        }

        public static int ColumnCount(Sqlite3DatabaseHandle stmt)
        {
            return Logger(NetWorkedData.SQLite3.ColumnCount(stmt), nameof(ColumnCount));
        }

        public static Sqlite3DatabaseHandle ColumnName(Sqlite3DatabaseHandle stmt, int index)
        {
            return NetWorkedData.SQLite3.ColumnName(stmt, index);
        }

        public static string ColumnName16(Sqlite3DatabaseHandle stmt, int index)
        {
            return Logger(NetWorkedData.SQLite3.ColumnName16(stmt, index), nameof(ColumnName16));
        }

        public static ColType ColumnType(Sqlite3DatabaseHandle stmt, int index)
        {
            return Logger(NetWorkedData.SQLite3.ColumnType(stmt, index), nameof(ColumnType));
        }

        public static int ColumnInt(Sqlite3DatabaseHandle stmt, int index)
        {
            return Logger(NetWorkedData.SQLite3.ColumnInt(stmt, index), nameof(ColumnInt));
        }

        public static long ColumnInt64(Sqlite3DatabaseHandle stmt, int index)
        {
            return Logger(NetWorkedData.SQLite3.ColumnInt64(stmt, index), nameof(ColumnInt64));
        }

        public static double ColumnDouble(Sqlite3DatabaseHandle stmt, int index)
        {
            return Logger(NetWorkedData.SQLite3.ColumnDouble(stmt, index), nameof(ColumnDouble));
        }

        public static Sqlite3DatabaseHandle ColumnText(Sqlite3DatabaseHandle stmt, int index)
        {
            return NetWorkedData.SQLite3.ColumnText(stmt, index);
        }

        public static Sqlite3DatabaseHandle ColumnText16(Sqlite3DatabaseHandle stmt, int index)
        {
            return NetWorkedData.SQLite3.ColumnText16(stmt, index);
        }

        public static Sqlite3DatabaseHandle ColumnBlob(Sqlite3DatabaseHandle stmt, int index)
        {
            return NetWorkedData.SQLite3.ColumnBlob(stmt, index);
        }

        public static int ColumnBytes(Sqlite3DatabaseHandle stmt, int index)
        {
            return Logger(NetWorkedData.SQLite3.ColumnBytes(stmt, index), nameof(ColumnBytes));
        }

        public static string ColumnString(Sqlite3DatabaseHandle stmt, int index)
        {
            return Logger(NetWorkedData.SQLite3.ColumnString(stmt, index), nameof(ColumnString));
        }

        public static byte[] ColumnByteArray(Sqlite3DatabaseHandle stmt, int index)
        {
            int length = ColumnBytes(stmt, index);
            byte[] result = new byte[length];
            if (length > 0)
                Marshal.Copy(ColumnBlob(stmt, index), result, 0, length);
            return result;
        }

        public static ExtendedResult ExtendedErrCode(Sqlite3DatabaseHandle db)
        {
            return Logger(NetWorkedData.SQLite3.ExtendedErrCode(db), nameof(ExtendedErrCode));
        }

        public static int LibVersionNumber()
        {
            return Logger(NetWorkedData.SQLite3.LibVersionNumber(), nameof(LibVersionNumber));
        }

        //-------------------------------------------------------------------------------------------------------------
        public static Sqlite3DatabaseHandle Prepare2(Sqlite3DatabaseHandle db, string query)
        {
            Sqlite3DatabaseHandle stmt;
            Result r = Prepare2(db, query, Encoding.UTF8.GetByteCount(query), out stmt, Sqlite3DatabaseHandle.Zero);
            if (r != Result.OK)
            {
                Debug.Log("Erronous query `" + query + "`");
                throw SQLiteException.New(r, GetErrmsg(db));
            }
            return stmt;
        }
        //-------------------------------------------------------------------------------------------------------------
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}
//=====================================================================================================================
