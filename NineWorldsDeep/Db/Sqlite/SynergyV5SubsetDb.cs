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

            //populate list id if not set, creating list if !exists
            //this runs in its own transaction so the list will have written to db
            //before the later transaction needs to access its id
            if (synLst.ListId < 1)
            {
                EnsureSynergyV5ListName(listName);
            }

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        //try
                        {
                            string activated =
                                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(synLst.ActivatedAt);
                            string shelved =
                                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(synLst.ShelvedAt);
                            
                            //ensure current timestamps
                            UpdateTimeStampsForSynergyV5ListName(activated, shelved, listName, cmd);

                            PopulateIdAndTimeStamps(synLst, cmd);

                            PopulateListItems(synLst, cmd);

                            // for each SynergyV5ListItem,
                            // do the same (populate item id, ensure, etc.)
                            for (int i = 0; i < synLst.ListItems.Count; i++)
                            {

                                SynergyV5ListItem sli = synLst.ListItems[i];
                                Save(synLst, sli, i, cmd);
                            }

                            transaction.Commit();
                        }
                        //catch (Exception ex)
                        //{
                        //    //handle exception here
                        //    transaction.Rollback();

                        //    throw ex;
                        //    //UI.Display.Exception(ex);
                        //}
                    }
                }

                conn.Close();
            }

        }

        private void PopulateListItems(SynergyV5List synLst, SQLiteCommand cmd)
        {
            //mirrors synergyV5PopulateListItems() in Gauntlet

            //select list items by position for list
            cmd.Parameters.Clear();
            cmd.CommandText =
                SYNERGY_V5_SELECT_LIST_ITEMS_AND_TODOS_BY_POSITION_FOR_LIST_ID_X;
                            
            SQLiteParameter listIdParam = new SQLiteParameter();
            listIdParam.Value = synLst.ListId;
            cmd.Parameters.Add(listIdParam);
                            
            using (var rdr = cmd.ExecuteReader())
            {
                int itemId, position, listItemId, toDoId;

                String itemValue,
                        toDoActivatedAtString,
                        toDoCompletedAtString,
                        toDoArchivedAtString;

                while (rdr.Read())
                {
                    itemId = GetNullableInt32(rdr, 0);
                    itemValue = GetNullableString(rdr, 1);
                    position = GetNullableInt32(rdr, 2);
                    listItemId = GetNullableInt32(rdr, 3);
                    toDoId = GetNullableInt32(rdr, 4);
                    toDoActivatedAtString = GetNullableString(rdr, 5);
                    toDoCompletedAtString = GetNullableString(rdr, 6);
                    toDoArchivedAtString = GetNullableString(rdr, 7);

                    SynergyV5ListItem sli = new SynergyV5ListItem(itemValue);
                    sli.ItemId = itemId;
                    sli.ListItemId = listItemId;

                    if(toDoId > 0)
                    {
                        //has toDo item

                        SynergyV5ToDo toDo = new SynergyV5ToDo();
                        toDo.ToDoId = toDoId;
                                        
                        DateTime? activated =
                            TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(toDoActivatedAtString);

                        DateTime? completed =
                            TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(toDoCompletedAtString);

                        DateTime? archived =
                            TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(toDoArchivedAtString);

                        toDo.SetTimeStamps(activated, completed, archived);

                        sli.ToDo = toDo;
                    }

                    synLst.Add(position, sli);
                }
            }
                            
        }

        /// <summary>
        /// returns -1 if field is null
        /// </summary>
        /// <param name="rdr"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        private int GetNullableInt32(SQLiteDataReader rdr, int idx)
        {
            if (!rdr.IsDBNull(idx))
            {
                return rdr.GetInt32(idx);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// returns "" (empty string) if field is null
        /// </summary>
        /// <param name="rdr"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        private string GetNullableString(SQLiteDataReader rdr, int idx)
        {
            if (!rdr.IsDBNull(idx))
            {
                return rdr.GetString(idx);
            }
            else
            {
                return "";
            }
        }

        private void PopulateIdAndTimeStamps(SynergyV5List synLst, SQLiteCommand cmd)
        {
            string listName = synLst.ListName;

            cmd.Parameters.Clear();

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

                    DateTime? activated =
                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(activatedString);

                    DateTime? shelved =
                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(shelvedString);

                    synLst.ListId = listId;
                    synLst.SetTimeStamps(activated, shelved);
                }
            }

            
        }

        private void UpdateTimeStampsForSynergyV5ListName(string activated, string shelved, string listName, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = SYNERGY_V5_LIST_UPDATE_ACTIVATE_AT_SHELVED_AT_FOR_LIST_NAME_X_Y_Z;

            SQLiteParameter activatedParam = new SQLiteParameter();
            activatedParam.Value = activated;
            cmd.Parameters.Add(activatedParam);

            SQLiteParameter shelvedParam = new SQLiteParameter();
            shelvedParam.Value = shelved;
            cmd.Parameters.Add(shelvedParam);

            SQLiteParameter listNameParam = new SQLiteParameter();
            listNameParam.Value = listName;
            cmd.Parameters.Add(listNameParam);
            
            cmd.ExecuteNonQuery();
        }

        private void EnsureSynergyV5ListName(string listName)
        {
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
                            cmd.Parameters.Clear();
                            cmd.CommandText = SYNERGY_V5_ENSURE_LIST_NAME_X;

                            SQLiteParameter listNameParam = new SQLiteParameter();
                            listNameParam.Value = listName;
                            cmd.Parameters.Add(listNameParam);

                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            //handle exception here
                            transaction.Rollback();
                            UI.Display.Exception(ex);
                        }
                    }
                }

                conn.Close();
            }
        }

        internal void Save(SynergyV5List synLst, SynergyV5ListItem sli, int position, SQLiteCommand cmd)
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
                    EnsureSynergyV5ItemValue(sli.ItemValue, cmd);

                    sli.ItemId = GetIdForSynergyV5ItemValue(sli.ItemValue, cmd);
                }

                if(sli.ListItemId < 1)
                {
                    EnsureListItemPosition(synLst.ListId, sli.ItemId, position, cmd);

                    sli.ListItemId = GetListItemId(synLst.ListId, sli.ItemId, cmd);
                        
                }

                UpdatePositionForListItemId(position, sli.ListItemId, cmd);

                SynergyV5ToDo toDo = sli.ToDo;

                if(toDo != null)
                {
                    string activated =
                        TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(toDo.ActivatedAt);

                    string completed =
                        TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(toDo.CompletedAt);

                    string archived =
                        TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(toDo.ArchivedAt);

                    EnsureToDoForListItemId(sli.ListItemId,
                                            activated,
                                            completed,
                                            archived,
                                            cmd);

                    UpdateToDoForListItemId(activated,
                                            completed,
                                            archived,
                                            sli.ListItemId,
                                            cmd);
                }
            }
        }

        private void UpdateToDoForListItemId(string activated, string completed, string archived, int listItemId, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = SYNERGY_V5_UPDATE_TO_DO_WHERE_LIST_ITEM_ID_AC_CO_AR_ID;

            SQLiteParameter activatedParam = new SQLiteParameter();
            activatedParam.Value = activated;
            cmd.Parameters.Add(activatedParam);

            SQLiteParameter completedParam = new SQLiteParameter();
            completedParam.Value = completed;
            cmd.Parameters.Add(completedParam);

            SQLiteParameter archivedParam = new SQLiteParameter();
            archivedParam.Value = archived;
            cmd.Parameters.Add(archivedParam);

            SQLiteParameter listItemIdParam = new SQLiteParameter();
            listItemIdParam.Value = listItemId;
            cmd.Parameters.Add(listItemIdParam);

            cmd.ExecuteNonQuery();
        }

        private void EnsureToDoForListItemId(int listItemId, string activated, string completed, string archived, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = SYNERGY_V5_ENSURE_TO_DO_FOR_LIST_ITEM_ID_ID_AC_CO_AR;

            SQLiteParameter listItemIdParam = new SQLiteParameter();
            listItemIdParam.Value = listItemId;
            cmd.Parameters.Add(listItemIdParam);

            SQLiteParameter activatedParam = new SQLiteParameter();
            activatedParam.Value = activated;
            cmd.Parameters.Add(activatedParam);

            SQLiteParameter completedParam = new SQLiteParameter();
            completedParam.Value = completed;
            cmd.Parameters.Add(completedParam);

            SQLiteParameter archivedParam = new SQLiteParameter();
            archivedParam.Value = archived;
            cmd.Parameters.Add(archivedParam);

            cmd.ExecuteNonQuery();
        }

        private void UpdatePositionForListItemId(int position, int listItemId, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = SYNERGY_V5_UPDATE_POSITION_FOR_LIST_ITEM_ID_X_Y;

            SQLiteParameter positionParam = new SQLiteParameter();
            positionParam.Value = position;
            cmd.Parameters.Add(positionParam);

            SQLiteParameter listItemIdParam = new SQLiteParameter();
            listItemIdParam.Value = listItemId;
            cmd.Parameters.Add(listItemIdParam);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// return -1 if not found
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="itemId"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private int GetListItemId(int listId, int itemId, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText = SYNERGY_V5_SELECT_LIST_ITEM_ID_FOR_LIST_ID_ITEM_ID_X_Y;

            SQLiteParameter listIdParam = new SQLiteParameter();
            listIdParam.Value = listId;
            cmd.Parameters.Add(listIdParam);

            SQLiteParameter itemIdParam = new SQLiteParameter();
            itemIdParam.Value = itemId;
            cmd.Parameters.Add(itemIdParam);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = GetNullableInt32(rdr, 0);
                }
            }

            return id;
        }

        private void EnsureListItemPosition(int listId, int itemId, int position, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = SYNERGY_V5_ENSURE_LIST_ITEM_POSITION_X_Y_Z;

            SQLiteParameter listIdParam = new SQLiteParameter();
            listIdParam.Value = listId;
            cmd.Parameters.Add(listIdParam);

            SQLiteParameter itemIdParam = new SQLiteParameter();
            itemIdParam.Value = itemId;
            cmd.Parameters.Add(itemIdParam);
            
            SQLiteParameter positionParam = new SQLiteParameter();
            positionParam.Value = position;
            cmd.Parameters.Add(positionParam);

            cmd.ExecuteNonQuery();
        }

        private void EnsureSynergyV5ItemValue(string itemValue, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = SYNERGY_V5_ENSURE_ITEM_VALUE_X;

            SQLiteParameter itemValueParam = new SQLiteParameter();
            itemValueParam.Value = itemValue;
            cmd.Parameters.Add(itemValueParam);

            cmd.ExecuteNonQuery();
        }

        private int GetIdForSynergyV5ItemValue(string itemValue, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText = SYNERGY_V5_SELECT_ID_FOR_ITEM_VALUE_X;

            SQLiteParameter itemValueParam = new SQLiteParameter();
            itemValueParam.Value = itemValue;
            cmd.Parameters.Add(itemValueParam);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = GetNullableInt32(rdr, 0);
                }
            }

            return id;
        }

        #region "queries"

        public static string
            SYNERGY_V5_LIST_UPDATE_ACTIVATE_AT_SHELVED_AT_FOR_LIST_NAME_X_Y_Z =
            
            "UPDATE " + NwdContract.TABLE_SYNERGY_LIST + " "
            + "SET " + NwdContract.COLUMN_SYNERGY_LIST_ACTIVATED_AT + " = MAX(IFNULL(" + NwdContract.COLUMN_SYNERGY_LIST_ACTIVATED_AT + ", ''), ?), "
            + "	   " + NwdContract.COLUMN_SYNERGY_LIST_SHELVED_AT + " = MAX(IFNULL(" + NwdContract.COLUMN_SYNERGY_LIST_SHELVED_AT + ", ''), ?) "
            + "WHERE " + NwdContract.COLUMN_SYNERGY_LIST_NAME + " = ?; ";

        public static string
            SYNERGY_V5_UPDATE_TO_DO_WHERE_LIST_ITEM_ID_AC_CO_AR_ID =

            "UPDATE " + NwdContract.TABLE_SYNERGY_TO_DO + "  "
            + "SET " + NwdContract.COLUMN_SYNERGY_TO_DO_ACTIVATED_AT
                    + " = MAX(IFNULL(" + NwdContract.COLUMN_SYNERGY_TO_DO_ACTIVATED_AT + ", ''), ?), "
            + "	   " + NwdContract.COLUMN_SYNERGY_TO_DO_COMPLETED_AT
                    + " = MAX(IFNULL(" + NwdContract.COLUMN_SYNERGY_TO_DO_COMPLETED_AT + ", ''), ?), "
            + "	   " + NwdContract.COLUMN_SYNERGY_TO_DO_ARCHIVED_AT
                    + " = MAX(IFNULL(" + NwdContract.COLUMN_SYNERGY_TO_DO_ARCHIVED_AT + ", ''), ?) "
            + "WHERE " + NwdContract.COLUMN_SYNERGY_LIST_ITEM_ID + " = ?; ";

        public static string
            SYNERGY_V5_ENSURE_TO_DO_FOR_LIST_ITEM_ID_ID_AC_CO_AR =

            "INSERT OR IGNORE INTO " + NwdContract.TABLE_SYNERGY_TO_DO + "  "
            + "	(" + NwdContract.COLUMN_SYNERGY_LIST_ITEM_ID + ", "
            + "	 " + NwdContract.COLUMN_SYNERGY_TO_DO_ACTIVATED_AT + ", "
            + "	 " + NwdContract.COLUMN_SYNERGY_TO_DO_COMPLETED_AT + ", "
            + "	 " + NwdContract.COLUMN_SYNERGY_TO_DO_ARCHIVED_AT + ")  "
            + "VALUES  "
            + "	(?, ?, ?, ?); ";

        public static string
            SYNERGY_V5_UPDATE_POSITION_FOR_LIST_ITEM_ID_X_Y =

            "UPDATE " + NwdContract.TABLE_SYNERGY_LIST_ITEM + " "
            + "SET " + NwdContract.COLUMN_SYNERGY_LIST_ITEM_POSITION + " = ? "
            + "WHERE " + NwdContract.COLUMN_SYNERGY_LIST_ITEM_ID + " = ? ; ";

        public static string SYNERGY_V5_SELECT_ID_FOR_ITEM_VALUE_X =

            "SELECT " + NwdContract.COLUMN_SYNERGY_ITEM_ID + " "
            + "FROM " + NwdContract.TABLE_SYNERGY_ITEM + " "
            + "WHERE " + NwdContract.COLUMN_SYNERGY_ITEM_VALUE + " = ? ; ";

        public static string SYNERGY_V5_ENSURE_ITEM_VALUE_X =

            "INSERT OR IGNORE INTO " + NwdContract.TABLE_SYNERGY_ITEM + " "
            + "	(" + NwdContract.COLUMN_SYNERGY_ITEM_VALUE + ") "
            + "VALUES "
            + "	(?); ";

        public static string
            SYNERGY_V5_SELECT_LIST_ITEM_ID_FOR_LIST_ID_ITEM_ID_X_Y =

            "SELECT " + NwdContract.COLUMN_SYNERGY_LIST_ITEM_ID + " "
            + "FROM " + NwdContract.TABLE_SYNERGY_LIST_ITEM + " "
            + "WHERE " + NwdContract.COLUMN_SYNERGY_LIST_ID + " = ?  "
            + "AND " + NwdContract.COLUMN_SYNERGY_ITEM_ID + " = ? ; ";

        private static string
            SYNERGY_V5_SELECT_ID_ACTIVATED_AT_SHELVED_AT_FOR_LIST_NAME =

            "SELECT " + NwdContract.COLUMN_SYNERGY_LIST_ID + ", "
                      + NwdContract.COLUMN_SYNERGY_LIST_ACTIVATED_AT + ", "
                      + NwdContract.COLUMN_SYNERGY_LIST_SHELVED_AT + " "
            + "FROM " + NwdContract.TABLE_SYNERGY_LIST + " "
            + "WHERE " + NwdContract.COLUMN_SYNERGY_LIST_NAME + " = ? ;";

        private static string
            SYNERGY_V5_SELECT_LIST_ITEMS_AND_TODOS_BY_POSITION_FOR_LIST_ID_X =

            "SELECT si." + NwdContract.COLUMN_SYNERGY_ITEM_ID + ", "
            + "	    si." + NwdContract.COLUMN_SYNERGY_ITEM_VALUE + ", "
            + "	    sli." + NwdContract.COLUMN_SYNERGY_LIST_ITEM_POSITION + ", "
            + "     sli." + NwdContract.COLUMN_SYNERGY_LIST_ITEM_ID + ", "
            + "     std." + NwdContract.COLUMN_SYNERGY_TO_DO_ID + ", "
            + "     std." + NwdContract.COLUMN_SYNERGY_TO_DO_ACTIVATED_AT + ", "
            + "     std." + NwdContract.COLUMN_SYNERGY_TO_DO_COMPLETED_AT + ", "
            + "     std." + NwdContract.COLUMN_SYNERGY_TO_DO_ARCHIVED_AT + " "
            + "FROM " + NwdContract.TABLE_SYNERGY_LIST_ITEM + " sli "
            + "JOIN " + NwdContract.TABLE_SYNERGY_ITEM + " si "
            + "ON sli." + NwdContract.COLUMN_SYNERGY_ITEM_ID + " = si." + NwdContract.COLUMN_SYNERGY_ITEM_ID + " "
            + "LEFT JOIN " + NwdContract.TABLE_SYNERGY_TO_DO + " std "
            + "ON sli." + NwdContract.COLUMN_SYNERGY_LIST_ITEM_ID + " = std." + NwdContract.COLUMN_SYNERGY_LIST_ITEM_ID + " "
            + "WHERE sli." + NwdContract.COLUMN_SYNERGY_LIST_ID + " = ? "
            + "ORDER BY sli." + NwdContract.COLUMN_SYNERGY_LIST_ITEM_POSITION + "; ";

        public static string SYNERGY_V5_ENSURE_LIST_NAME_X =

            "INSERT OR IGNORE INTO " + NwdContract.TABLE_SYNERGY_LIST + " "
            + "	(" + NwdContract.COLUMN_SYNERGY_LIST_NAME + ") "
            + "VALUES "
            + "	(?); ";


        public static string SYNERGY_V5_ENSURE_LIST_ITEM_POSITION_X_Y_Z =

            "INSERT OR IGNORE INTO " + NwdContract.TABLE_SYNERGY_LIST_ITEM + " "
            + "	(" + NwdContract.COLUMN_SYNERGY_LIST_ID + ",  "
            + "	 " + NwdContract.COLUMN_SYNERGY_ITEM_ID + ",  "
            + "	 " + NwdContract.COLUMN_SYNERGY_LIST_ITEM_POSITION +  ") "
            + "VALUES "
            + "	(?, ?, ?); ";

        #endregion


        #region "templates"

        //transaction template, doesn't do anything, copy and modify for convenience
        private void TransactionTemplate()
        {
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
                            // do stuff here

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            //handle exception here
                            transaction.Rollback();
                        }
                    }
                }

                conn.Close();
            }
        }

        //query template, doesn't do anything, copy and modify for convenience
        private void SelectQueryTemplate(SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                "SELECT * FROM SomeTable WHERE Col1 = ? AND Col2 = ? ";

            SQLiteParameter col1Param = new SQLiteParameter();
            col1Param.Value = 1;
            cmd.Parameters.Add(col1Param);

            SQLiteParameter col2Param = new SQLiteParameter();
            col2Param.Value = 2;
            cmd.Parameters.Add(col2Param);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    //do something here
                }
            }
        }

        //query template, doesn't do anything, copy and modify for convenience
        private void InsertQueryTemplate(SQLiteCommand cmd)
        {

            cmd.Parameters.Clear();
            cmd.CommandText =
                "INSERT OR IGNORE INTO SomeTable" +
                " (Col1, Col2) VALUES (?, ?)";

            SQLiteParameter param1 = new SQLiteParameter();
            param1.Value = 1;
            cmd.Parameters.Add(param1);

            SQLiteParameter param2 = new SQLiteParameter();
            param2.Value = 2;
            cmd.Parameters.Add(param2);


            cmd.ExecuteNonQuery();
        }

        //query template, doesn't do anything, copy and modify for convenience
        private void UpdateQueryTemplate(SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                "update query here";

            SQLiteParameter param1 = new SQLiteParameter();
            param1.Value = 1;
            cmd.Parameters.Add(param1);

            SQLiteParameter param2 = new SQLiteParameter();
            param2.Value = 2;
            cmd.Parameters.Add(param2);

            cmd.ExecuteNonQuery();
        }

        #endregion
    }

}
