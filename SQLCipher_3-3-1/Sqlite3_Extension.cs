using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

using UnityEngine;
using Sqlite3DatabaseHandle = System.IntPtr;
using Sqlite3Statement = System.IntPtr;

namespace SQLite4Unity3d
{
	public partial class SQLiteConnection : IDisposable
	{

        //public SQLiteConnection(string databasePath, string password)
        //{
        //    this.DatabasePath = databasePath;
        //    Sqlite3DatabaseHandle handle;
        //    byte[] databasePathAsBytes = GetNullTerminatedUtf8(this.DatabasePath);
        //    SQLite3.Result r = SQLite3.Open(databasePathAsBytes, out handle, (int)(SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create), IntPtr.Zero);
        //    this._open = true;
        //    this.Handle = handle;
        //    if (r != SQLite3.Result.OK)
        //    {
        //        this._open = false;
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(password))
        //        {
        //            SQLite3.Result result = SQLite3.Key(handle, password, password.Length);
        //            if (result != SQLite3.Result.OK)
        //            {
        //                this._open = false;
        //            }
        //        }
        //    }
        //    this.StoreDateTimeAsTicks = false;
        //    this.BusyTimeout = TimeSpan.FromSeconds(0.01);
        //}




        public void MigrateTableByType(Type sType, string sOldSuffix ="_old", bool sDeleteOld = true)
        {
            // prepare the old data base with the new columns
            CreateTable(sType, CreateFlags.None);
            // get all informations about the index
            TableMapping tTableMapping = new TableMapping(sType);
            string tTableName = tTableMapping.TableName;

            Execute("DROP TABLE IF EXISTS `" + tTableName + sOldSuffix + "`");
            Execute("DROP TABLE IF EXISTS `" + tTableName + "_new`");
            Execute("DROP TABLE IF EXISTS `" + tTableName + "_old`");

            //Debug.Log ("CreateTableByType " + sType.Name);
            var indexes = new Dictionary<string, SQLite4Unity3d.SQLiteConnection.IndexInfo>();
            foreach (var c in tTableMapping.Columns)
            {
                foreach (var i in c.Indices)
                {
                    var iname = i.Name ?? tTableName + "_" + c.Name;
                    SQLite4Unity3d.SQLiteConnection.IndexInfo iinfo;
                    if (!indexes.TryGetValue(iname, out iinfo))
                    {
                        iinfo = new SQLite4Unity3d.SQLiteConnection.IndexInfo
                        {
                            IndexName = iname,
                            TableName = tTableName,
                            Unique = i.Unique,
                            Columns = new List<SQLite4Unity3d.SQLiteConnection.IndexedColumn>()
                        };
                        indexes.Add(iname, iinfo);
                    }
                    if (i.Unique != iinfo.Unique)
                        throw new Exception("All the columns in an index must have the same value for their Unique property");
                    iinfo.Columns.Add(new SQLite4Unity3d.SQLiteConnection.IndexedColumn
                    {
                        Order = i.Order,
                        ColumnName = c.Name
                    });
                }
            }
            foreach (var indexName in indexes.Keys)
            {
                Execute("DROP INDEX IF EXISTS " + tTableName + "." + indexName + "");
            }
            // copie the old data to the new table
            List<string> tColumnsList = new List<string>();
            foreach (TableMapping.Column c in tTableMapping.Columns)
            {
                tColumnsList.Add(c.Name);
                // I need to alter the null value to default in columns
                //if (!c.IsNullable)
                {
                    string tDefault = "''";
                    if (c.ColumnType == typeof(Boolean) ||
                        c.ColumnType == typeof(Byte) ||
                        c.ColumnType == typeof(UInt16) ||
                        c.ColumnType == typeof(SByte) ||
                        c.ColumnType == typeof(Int16) ||
                        c.ColumnType == typeof(Int32) ||
                        c.ColumnType == typeof(UInt32) ||
                        c.ColumnType == typeof(Int64) ||
                        c.ColumnType == typeof(Single) ||
                        c.ColumnType == typeof(Double) ||
                        c.ColumnType == typeof(Decimal) ||
                        c.ColumnType == typeof(TimeSpan) ||
                        c.ColumnType == typeof(DateTime) ||
                        c.ColumnType == typeof(DateTimeOffset)
                        )
                    {
                        tDefault = "0";
#if !NETFX_CORE
                    } else if (c.ColumnType.IsEnum) {
#else
                 } else if (c.ColumnType.GetTypeInfo().IsEnum) {
#endif
                        tDefault = "0";
                    }
                    string SQLColumns = "UPDATE `" + tTableName + "` SET `" + c.Name + "` = "+tDefault+" WHERE `"+c.Name+"` IS NULL";
                    Execute(SQLColumns);
                    Debug.Log("SQLColumns = " + SQLColumns);
                }
            }
            // rename the actual table to "xxxx_old"
            string SQLRename = "ALTER TABLE `" + tTableName + "` RENAME TO `" + tTableName + sOldSuffix+"`";
            //Debug.Log("SQLRename = " + SQLRename);
            Execute(SQLRename);
            // create new table with perfect schemas (no additionnal old columns
            CreateTable(sType, CreateFlags.None);
            string SQLCopy = "INSERT INTO `" + tTableName + "` (`" + string.Join("`, `", tColumnsList.ToArray()) + "`) SELECT `" + string.Join("`, `", tColumnsList.ToArray()) + "` FROM `" + tTableName + sOldSuffix + "`";
            //Debug.Log("SQLCopy = " + SQLCopy);
            Execute(SQLCopy);
            if (sDeleteOld == true)
            {
                string SQLDrop = "DROP TABLE IF EXISTS `" + tTableName + sOldSuffix + "`";
                //Debug.Log("SQLDrop = " + SQLDrop);
                Execute(SQLDrop);
            }
        }

		public int CreateTableByType(Type sType)
		{
            TableMapping tTableMapping = new TableMapping(sType);
            string tTableName = tTableMapping.TableName;

			//Debug.Log ("CreateTableByType " + sType.Name);

            var indexes = new Dictionary<string, SQLiteConnection.IndexInfo>();
            foreach (var c in tTableMapping.Columns)
            {
                foreach (var i in c.Indices)
                {
                    var iname = i.Name ?? tTableName + "_" + c.Name;
                    SQLiteConnection.IndexInfo iinfo;
                    if (!indexes.TryGetValue(iname, out iinfo))
                    {
                        iinfo = new SQLiteConnection.IndexInfo
                        {
                            IndexName = iname,
                            TableName = tTableName,
                            Unique = i.Unique,
                            Columns = new List<SQLiteConnection.IndexedColumn>()
                        };
                        indexes.Add(iname, iinfo);
                    }
                    if (i.Unique != iinfo.Unique)
                        throw new Exception("All the columns in an index must have the same value for their Unique property");
                    iinfo.Columns.Add(new SQLiteConnection.IndexedColumn
                    {
                        Order = i.Order,
                        ColumnName = c.Name
                    });
                }
            }
            /*
            foreach (var indexName in indexes.Keys)
            {
                Execute("DROP INDEX IF EXISTS " + tTableName + "." + indexName + "");
            }
            */
			return CreateTable(sType, CreateFlags.None);
		}

		public int DropTableByType(Type sType)
		{
			var map = GetMapping (sType);
			var query = string.Format("DROP TABLE IF EXISTS `{0}`", map.TableName);
			return Execute (query);
        }

        public int TruncateTableByType(Type sType)
        {
            var map = GetMapping(sType);
            var query = string.Format("TRUNCATE TABLE IF EXISTS `{0}`", map.TableName);
            return Execute(query);
        }


        //public bool TestDatabseIsReallyOpen()
        //{
        //    bool rReturn = true;
        //    try
        //    {
        //        // test if select is possible
        //        Execute("SELECT name FROM sqlite_master WHERE type = 'table';");
        //    }
        //    catch (SQLiteException e)
        //    {
        //        Debug.Log("I AM HERE THEN DATABASE IS NOT USABLE! IT'S PROTECTED!");
        //        rReturn = false;
        //    }
        //    return rReturn;
        //}
	}
}
