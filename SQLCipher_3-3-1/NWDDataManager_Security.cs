//=====================================================================================================================
//
//  ideMobi 2020©
//
//  All rights reserved by ideMobi
//
//=====================================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

//=====================================================================================================================
namespace NetWorkedData
{
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    public partial class NWDDataManager
    {
        //-------------------------------------------------------------------------------------------------------------
        const string KDBPrefix = "N";
        //-------------------------------------------------------------------------------------------------------------
        public string GetVersion()
        {
            string rReturn = SQLite3.LibVersionNumber().ToString();
            if (SQLiteEditorHandle != null)
            {
                IntPtr stmt = SQLite3.Prepare2(SQLiteEditorHandle, "PRAGMA cipher_version;");
                while (SQLite3.Step(stmt) == SQLite3.Result.Row)
                {
                    rReturn = rReturn + "(sqlcipher " + SQLite3.ColumnString(stmt, 0) + ")";
                }
                SQLite3.Finalize(stmt);
            }
            return rReturn;
        }
        //-------------------------------------------------------------------------------------------------------------
        public bool IsSecure()
        {
            return true;
        }
        //-------------------------------------------------------------------------------------------------------------
        public string DatabaseAccountName()
        {
            return KDBPrefix + NWDAppConfiguration.SharedInstance().DatabasePrefix + DatabaseNameAccount;
        }
        //-------------------------------------------------------------------------------------------------------------
        public string DatabaseEditorName()
        {
            return KDBPrefix + DatabaseNameEditor;
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
                SQLite3.Result trResultPassword = SQLite3.Key(SQLiteAccountHandle, tAccountPass, tAccountPass.Length);
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