using NineWorldsDeep.Core;
using NineWorldsDeep.Sqlite;
using NineWorldsDeep.Synergy.V5;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Db.Sqlite
{
    public class SynergyV5SubsetDb
    {
        protected string DbName { get; private set; }

        public SynergyV5SubsetDb()
        {
            DbName = "nwd";
        }

        public List<SynergyV5List> SelectAllActiveListsDeferredLoad()
        {
            throw new NotImplementedException();
        }
                
        internal void Save(SynergyV5List synLst)
        {
            // parallels gauntlet logic
            // (Android App: http://github.com/BBuchholz/Gauntlet)

            string listName = synLst.ListName;
            string activated = 
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(synLst.ActivatedAt);
            string shelved =
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(synLst.ShelvedAt);

            //populate list id if not set, creating list if !exists
            if (synLst.ListId < 1)
            {
                EnsureSynergyV5ListName(listName);
            }

            //ensure current timestamps
            UpdateTimeStampsForSynergyV5ListName(activated, shelved, listName);

            PopulateIdAndTimeStamps(synLst);

            // for each SynergyV5ListItem,
            // do the same (populate item id, ensure, etc.)
            for (int i = 0; i < synLst.ListItems.Count; i++)
            {

                SynergyV5ListItem sli = synLst.ListItems[i];
                Save(synLst, sli, i);
            }
        }

        private void PopulateIdAndTimeStamps(SynergyV5List synLst)
        {
            string listName = synLst.ListName;

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {                        
                        try
                        {

                            cmd.CommandText =
                                SYNERGY_V5_SELECT_ID_ACTIVATED_AT_SHELVED_AT_FOR_LIST_NAME; 

                            SQLiteParameter listNameParam = new SQLiteParameter();
                            listNameParam.Value = listName;
                            cmd.Parameters.Add(listNameParam);
                            
                            using (var rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    int listId = rdr.GetInt32(0);
                                    string activatedString =
                                        rdr.GetString(1);
                                    string shelvedString =
                                        rdr.GetString(2);

                                    DateTime activated =
                                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(activatedString);

                                    DateTime shelved =
                                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(shelvedString);

                                    synLst.ListId = listId;
                                    synLst.SetTimeStamps(activated, shelved);
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            UI.Display.Exception(ex);
                            transaction.Rollback();
                        }
                    }
                }

                conn.Close();
            }
        }

        private void UpdateTimeStampsForSynergyV5ListName(string activated, string shelved, string listName)
        {
            throw new NotImplementedException();
            //SYNERGY_V5_LIST_UPDATE_ACTIVATE_AT_SHELVED_AT_FOR_LIST_NAME_X_Y_Z
            //goes here
        }

        private void EnsureSynergyV5ListName(string listName)
        {
            throw new NotImplementedException();
            //SYNERGY_V5_ENSURE_LIST_NAME_X goes here
        }

        internal void Save(SynergyV5List synLst, SynergyV5ListItem sli, int position)
        {
            if(synLst.ListId < 1)
            {
                UI.Display.Message("unable to save list item, list id not set");
            }
            else
            {
                //item id should be set, or ensured if not
                if(sli.ItemId < 1)
                {
                    //SYNERGY_V5_ENSURE_ITEM_VALUE_X goes here
                    throw new NotImplementedException();

                    sli.ItemId =
                        GetIdForSynergyV5ItemValue(sli.ItemValue);
                }

                //SYNERGY_V5_ENSURE_LIST_ITEM_POSITION_X_Y_Z goes here
                throw new NotImplementedException();
            }
        }

        private int GetIdForSynergyV5ItemValue(string itemValue)
        {
            throw new NotImplementedException();
        }

        #region "queries"

        private static string
            SYNERGY_V5_SELECT_ID_ACTIVATED_AT_SHELVED_AT_FOR_LIST_NAME =

            "SELECT " + NwdContract.COLUMN_SYNERGY_LIST_ID + ", "
                      + NwdContract.COLUMN_SYNERGY_LIST_ACTIVATED_AT + ", "
                      + NwdContract.COLUMN_SYNERGY_LIST_SHELVED_AT + " "
            + "FROM " + NwdContract.TABLE_SYNERGY_LIST + " "
            + "WHERE " + NwdContract.COLUMN_SYNERGY_LIST_NAME + " = ? ;";

        #endregion
    }

}
