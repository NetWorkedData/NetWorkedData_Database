//=====================================================================================================================
//
//  ideMobi 2020©
//  All rights reserved by ideMobi
//
//=====================================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif
//=====================================================================================================================
#if UNITY_EDITOR
using Sqlite = NetWorkedData.Logged.SQLite3; // Have a logged interface for SQLite (Editor only !)
#else
using Sqlite = NetWorkedData.SQLite3;
#endif
//=====================================================================================================================
namespace NetWorkedData
{
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    public static partial class SQLite3
    {
#if UNITY_EDITOR_OSX
        private const string DLL_NAME = "sqlcipher";
#elif UNITY_EDITOR_WIN
        private const string DLL_NAME = "sqlcipher";
#elif UNITY_EDITOR_LINUX
        private const string DLL_NAME = "libsqlcipher";
#elif UNITY_STANDALONE_OSX
        private const string DLL_NAME = "sqlcipher";
#elif UNITY_STANDALONE_WIN
        private const string DLL_NAME = "sqlcipher";
#elif UNITY_STANDALONE_LINUX
        private const string DLL_NAME = "libsqlcipher";
#elif UNITY_ANDROID
		private const string DLL_NAME = "sqlcipher";
#elif UNITY_IOS
		private const string DLL_NAME = "__Internal";
#elif UNITY_TVOS
		private const string DLL_NAME = "__Internal";
#endif
    }

    public partial class NWDDataManager
    {
        //-------------------------------------------------------------------------------------------------------------
        const string KDBPrefix = "R"; // old used 'O' for 4.3.0 '' for ealier version
        //-------------------------------------------------------------------------------------------------------------
        public string GetVersion()
        {
            string rReturn = Sqlite.LibVersionNumber().ToString();
            string rCipherVersion = " (sqlcipher -error-)";
            if (EditorDatabaseLoaded == true)
            {
                IntPtr stmt = Sqlite.Prepare2(SQLiteEditorHandle, "PRAGMA cipher_version;");
                while (Sqlite.Step(stmt) == SQLite3.Result.Row)
                {
                    rCipherVersion = " (sqlcipher " + Sqlite.ColumnString(stmt, 0) + ")";
                }
                Sqlite.Finalize(stmt);
            }
            return rReturn + rCipherVersion;
        }
        //-------------------------------------------------------------------------------------------------------------
        public bool IsSecure()
        {
            return true;
        }
        //-------------------------------------------------------------------------------------------------------------
        public string DatabaseAccountName()
        {
            return KDBPrefix + NWDAppConfiguration.SharedInstance().DatabasePrefix + NWD.K_DeviceDatabaseName;
        }
        //-------------------------------------------------------------------------------------------------------------
        public string DatabaseEditorName()
        {
            return KDBPrefix + NWD.K_EditorDatabaseName;
        }
        //-------------------------------------------------------------------------------------------------------------
        public string DatabaseBuildName()
        {
            return KDBPrefix + NWD.K_BuildDatabaseName;
        }
        //-------------------------------------------------------------------------------------------------------------
        public void DatabaseEditorOpenKey(string tDatabasePathEditor)
        {
            if (IsSecure())
            {
                string tEditorPass = NWDAppConfiguration.SharedInstance().GetEditorPass();
                SQLite3.Result trResultPassword = Sqlite.Key(SQLiteEditorHandle, tEditorPass, tEditorPass.Length);
                if (trResultPassword != SQLite3.Result.OK)
                {
                    throw SQLiteException.New(trResultPassword, string.Format("Could not open database file with password: {0} ({1})", tDatabasePathEditor, trResultPassword));
                }
            }
        }
        //-------------------------------------------------------------------------------------------------------------
        public void DatabaseAccountOpenKey(string tDatabasePathAccount, string sSurProtection)
        {
            if (IsSecure())
            {
                string tAccountPass = NWDAppConfiguration.SharedInstance().GetAccountPass(sSurProtection);
                SQLite3.Result trResultPassword = Sqlite.Key(SQLiteDeviceHandle, tAccountPass, tAccountPass.Length);
                if (trResultPassword != SQLite3.Result.OK)
                {
                    throw SQLiteException.New(trResultPassword, string.Format("Could not open database file with password: {0} ({1})", tDatabasePathAccount, trResultPassword));
                }
            }
        }
        //-------------------------------------------------------------------------------------------------------------
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}
//=====================================================================================================================