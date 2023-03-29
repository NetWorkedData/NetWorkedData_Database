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
using NetWorkedData.NWDORM;

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
        private const string DLL_NAME = "sqlcipher";
#elif UNITY_STANDALONE_OSX
        private const string DLL_NAME = "sqlcipher";
#elif UNITY_STANDALONE_WIN
        private const string DLL_NAME = "sqlcipher";
#elif UNITY_STANDALONE_LINUX
        private const string DLL_NAME = "sqlcipher";
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
        const string KDBPrefix = "O";
        //-------------------------------------------------------------------------------------------------------------
        public string GetVersion()
        {
            string rReturn = Sqlite.LibVersionNumber().ToString();
            string rCipherVersion = " (sqlcipher -error-)";
            if (EditorDatabaseLoaded == true)
            {
                using (IPrepareStatement tStatement = EditorFactory.CreatePrepareStatement ("PRAGMA cipher_version;"))
                {
                    while (tStatement.Step() == SQLite3.Result.Row)
                    {
                        rCipherVersion = " (sqlcipher " + Sqlite.ColumnString(tStatement.Handle, 0) + ")";
                    }
                }
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
                EditorFactory.Key(tEditorPass);
            }
        }
        //-------------------------------------------------------------------------------------------------------------
        public void DatabaseAccountOpenKey(string tDatabasePathAccount, string sSurProtection)
        {
            if (IsSecure())
            {
                string tAccountPass = NWDAppConfiguration.SharedInstance().GetAccountPass(sSurProtection);
                DeviceFactory.Key(tAccountPass);
            }
        }
        //-------------------------------------------------------------------------------------------------------------
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}
//=====================================================================================================================