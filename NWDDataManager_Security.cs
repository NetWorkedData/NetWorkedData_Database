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
            string rReturn = SQLite3.LibVersionNumber().ToString();
            string rCipherVersion = " (sqlcipher -error-)";
            if (EditorDatabaseLoaded == true)
            {
                IntPtr stmt = SQLite3.Prepare2(SQLiteEditorHandle, "PRAGMA cipher_version;");
                while (SQLite3.Step(stmt) == SQLite3.Result.Row)
                {
                    rCipherVersion = " (sqlcipher " + SQLite3.ColumnString(stmt, 0) + ")";
                }
                SQLite3.Finalize(stmt);
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
                SQLite3.Result trResultPassword = SQLite3.Key(SQLiteEditorHandle, tEditorPass, tEditorPass.Length);
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
                SQLite3.Result trResultPassword = SQLite3.Key(SQLiteDeviceHandle, tAccountPass, tAccountPass.Length);
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