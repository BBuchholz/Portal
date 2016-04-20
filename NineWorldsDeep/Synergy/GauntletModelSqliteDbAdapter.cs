using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;

namespace NineWorldsDeep.Synergy
{
    class GauntletModelSqliteDbAdapter : IGauntletDbAdapter
    {
        public List<ToDoList> GetActiveListItems()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// gets all lists in specified status (active = true, inactive = false)
        /// </summary>
        /// <param name="active"></param>
        /// <returns></returns>
        public List<ToDoList> GetLists(bool active)
        {
            Dictionary<string, Dictionary<int, ToDoItem>> listItems =
                new Dictionary<string, Dictionary<int, ToDoItem>>();
            Dictionary<string, Dictionary<int, string>> itemFragments =
                new Dictionary<string, Dictionary<int, string>>();

            try
            {
                using (var conn =
                    new SQLiteConnection(@"Data Source=" +
                        Configuration.GetSqliteDbPath("nwd")))
                {
                    conn.Open();

                    using (var cmd = new SQLiteCommand(conn))
                    {
                        using (var transaction = conn.BeginTransaction())
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText =
                                "SELECT List.ListId, " +
                                       "Item.ItemId, " +
                                       "Item.ItemValue, " +
                                       "Status.StatusValue, " +
                                       "junction_List_Item_Status.StampedAt, " +
                                       "List.ListName, " +
                                       "Fragment.FragmentValue " +
                                "FROM List " +
                                "JOIN junction_List_Item_Status " +
                                "ON List.ListId = junction_List_Item_Status.ListId " +
                                "JOIN Item " +
                                "ON junction_List_Item_Status.ItemId = Item.ItemId " +
                                "JOIN Status " +
                                "ON junction_List_Item_Status.StatusId = Status.StatusId " +
                                "LEFT JOIN Fragment " +
                                "ON Fragment.ListId = List.ListId " +
                                "AND Fragment.ItemId = Item.ItemId " +
                                "WHERE List.ListActive = @active";

                            cmd.Parameters.AddWithValue("@active", active);

                            using (var rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    int listId = rdr.GetInt32(0);
                                    int itemId = rdr.GetInt32(1);
                                    string itemValue = rdr.GetString(2);
                                    string statusValue = rdr.GetString(3);
                                    string stampedAt = rdr.GetString(4);
                                    string listName = rdr.GetString(5);
                                    string fragmentVal = "";

                                    if (!rdr.IsDBNull(6))
                                    {
                                        fragmentVal = rdr.GetString(6);
                                    }

                                    Status status =
                                        new Status()
                                        {
                                            StatusValue = statusValue,
                                            StampedAt = stampedAt
                                        };



                                    //if (!listNameToId.ContainsKey(listName))
                                    //{
                                    //    listNameToId.Add(listName, listId);
                                    //}

                                    if (!listItems.ContainsKey(listName))
                                    {
                                        listItems.Add(listName,
                                            new Dictionary<int, ToDoItem>());
                                        itemFragments.Add(listName,
                                            new Dictionary<int, string>());
                                    }

                                    if (!listItems[listName].ContainsKey(itemId))
                                    {
                                        listItems[listName][itemId] =
                                            new ToDoItem()
                                            {
                                                ListId = listId,
                                                ItemId = itemId,
                                                ItemValue = itemValue                                                
                                            };
                                    }

                                    listItems[listName][itemId].Statuses.AddWithMerge(status);

                                    if (!string.IsNullOrWhiteSpace(fragmentVal))
                                    {
                                        itemFragments[listName][itemId] = fragmentVal;
                                    }
                                }
                            }

                            transaction.Commit();
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                //lets just throw it for now, but put something here eventually
                throw ex;
            }

            List<ToDoList> lst = new List<ToDoList>();

            foreach (string listName in listItems.Keys)
            {
                ToDoList tdl = new ToDoList() { Name = listName };

                foreach (int itemId in listItems[listName].Keys)
                {
                    ToDoItem tdi = listItems[listName][itemId];

                    if (!tdi.Statuses.ContainsNonActiveStatuses())
                    {
                        tdl.AddWithMerge(tdi);
                    }
                }

                if (!tdl.Items.ContainsOnlyInactiveStatuses())
                {
                    lst.Add(tdl);
                }
            }

            return lst.OrderBy(x => x.Name).ToList();
        }

        public string Load(IListMatrix ilm)
        {
            return Load(ilm, false);
        }

        public string Save(IListMatrix ilm)
        {
            //be sure to use one transaction for speed
            //make EnsureXXX methods accept an open connection

            string outputMsg = "implementation in progress";
            string time = "";

            try
            {
                using (var conn =
                    new SQLiteConnection(@"Data Source=" +
                        Configuration.GetSqliteDbPath("nwd")))
                {
                    conn.Open();

                    using (var cmd = new SQLiteCommand(conn))
                    {
                        using (var transaction = conn.BeginTransaction())
                        {
                            Stopwatch sw = Stopwatch.StartNew();

                            foreach (ToDoList lst in ilm.Lists)
                            {
                                //EnsureIdForListName
                                int listId = EnsureIdForListName(lst.Name, cmd);

                                foreach (ToDoItem tdi in lst.Items)
                                {
                                    //EnsureIdForItemValue
                                    int itemId = EnsureIdForItemValue(tdi.Description, cmd);

                                    //Store Fragment
                                    string timeStamp = NwdUtils.GetTimeStamp_yyyyMMddHHmmss();
                                    EnsureFragment(listId, itemId, tdi.Fragment.FragmentValue, timeStamp, cmd);
                                    
                                    //EnsureListItem
                                    ToDoItem dbTdi = EnsureListItem(listId, itemId, cmd);
                                    bool archivedIsDirty = false;
                                    bool completedIsDirty = false;

                                    if (!dbTdi.Archived && tdi.Archived)
                                    {
                                        dbTdi.Archived = tdi.Archived;
                                        archivedIsDirty = true;
                                    }

                                    if (!dbTdi.Completed && tdi.Completed)
                                    {
                                        dbTdi.Completed = tdi.Completed;
                                        completedIsDirty = true;
                                    }

                                    if (archivedIsDirty || completedIsDirty)
                                    {
                                        UpdateStatuses(dbTdi,
                                                       completedIsDirty,
                                                       archivedIsDirty,
                                                       cmd);
                                    }


                                    // -be sure to load statuses within this method
                                    // -log a created status if it doesn't exist when 
                                    //  other statuses are retrieved (get all for ListIdItemId composite)
                                    //standard merging of completed and archived here
                                    //UpdateStatus if dirty?

                                    //////////////////////////////////-> INSERT OR IGNORE template
                                    //foreach (var tag in lst)
                                    //{
                                    //    if (!string.IsNullOrWhiteSpace(tag))
                                    //    {
                                    //        cmd.CommandText =
                                    //            "INSERT OR IGNORE INTO Tag (TagValue) VALUES (@tagValue)";
                                    //        cmd.Parameters.AddWithValue("@tagValue", tag);
                                    //        cmd.ExecuteNonQuery();
                                    //    }
                                    //}

                                    //this is how it is working in other parts of the code, 
                                    //can we rewrite it to allow parallel construction with the
                                    //INSERT OR IGNORE template and run all in same transaction?
                                    //clear parameters each time?
                                    //////////////////////////////////-> READER template
                                    //string cmdStr = "SELECT HashValue, HashId FROM Hash";

                                    //using (var cmd = new SQLiteCommand(cmdStr, conn))
                                    //{
                                    //    using (var rdr = cmd.ExecuteReader())
                                    //    {
                                    //        while (rdr.Read())
                                    //        {
                                    //            hashIds.Add(rdr.GetString(0), rdr.GetInt32(1));
                                    //        }
                                    //    }
                                    //}

                                    ////////////////////// ====> rewritten template (TEST IT!)
                                    //cmd.CommandText = "SELECT ListName, ListId FROM List WHERE ...";
                                    //cmd.Parameters.Clear(); //since we will be reusing command

                                    //using (var rdr = cmd.ExecuteReader())
                                    //{
                                    //    while (rdr.Read())
                                    //    {
                                    ////        hashIds.Add(rdr.GetString(0), rdr.GetInt32(1));
                                    //    }
                                    //}

                                }
                            }

                            transaction.Commit();

                            sw.Stop();
                            time = sw.Elapsed.ToString("mm\\:ss\\.ff");
                        }
                    }

                    conn.Close();

                    outputMsg = "Saved: " + time;
                }

            }
            catch (Exception ex)
            {
                outputMsg = "Error Saving to Sqlite Db: " + ex.Message;
            }

            return outputMsg;
        }

        private void EnsureFragment(int listId, int itemId, string fragmentValue, string timeStamp, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                "INSERT OR IGNORE INTO Fragment (ListId, ItemId, FragmentValue, UpdatedAt) " +
                "VALUES (@listId, @itemId, @fragVal, @updatedAt)";
            cmd.Parameters.AddWithValue("@listId", listId);
            cmd.Parameters.AddWithValue("@itemId", itemId);
            cmd.Parameters.AddWithValue("@fragVal", fragmentValue);
            cmd.Parameters.AddWithValue("@updatedAt", timeStamp);
            cmd.ExecuteNonQuery();
        }

        private void UpdateStatuses(ToDoItem dbTdi,
                                    bool completedIsDirty,
                                    bool archivedIsDirty,
                                    SQLiteCommand cmd)
        {
            if (dbTdi.ListId < 1 || dbTdi.ItemId < 1)
            {
                throw new Exception("Invalid ListId or " +
                    "ToDoItemId in UpdateStatus()");
            }
            else
            {
                //update archived and completed
                if (archivedIsDirty)
                {
                    MarkStatus(dbTdi.ListId, dbTdi.ItemId, "Archived", cmd);
                }

                if (completedIsDirty)
                {
                    MarkStatus(dbTdi.ListId, dbTdi.ItemId, "Completed", cmd);
                }
            }
        }

        private void MarkStatus(int listId, int itemId,
            string statusValue, SQLiteCommand cmd)
        {
            int statusId = EnsureIdForStatus(statusValue, cmd);
            string timeStamp = NwdUtils.GetTimeStamp_yyyyMMddHHmmss();

            cmd.Parameters.Clear();
            cmd.CommandText =
                "INSERT OR IGNORE INTO junction_List_Item_Status (ListId, " +
                                                                 "ItemId, " +
                                                                 "StatusId, " +
                                                                 "StampedAt) " +
                "VALUES (@listId, @itemId, @statusId, @timeStamp)";
            cmd.Parameters.AddWithValue("@listId", listId);
            cmd.Parameters.AddWithValue("@itemId", itemId);
            cmd.Parameters.AddWithValue("@statusId", statusId);
            cmd.Parameters.AddWithValue("@timeStamp", timeStamp);
            cmd.ExecuteNonQuery();
        }

        private ToDoItem EnsureListItem(int listId, int itemId, SQLiteCommand cmd)
        {
            //select value
            cmd.Parameters.Clear(); //since we will be reusing command
            cmd.CommandText = "SELECT ListId, ItemId " +
                              "FROM junction_List_Item_Status " +
                              "WHERE ListId = @listId " +
                              "AND ItemId = @itemId";
            cmd.Parameters.AddWithValue("@listId", listId);
            cmd.Parameters.AddWithValue("@itemId", itemId);

            bool registered = false;

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read()) //no entries found
                {
                    //we store this so we can wait until
                    //the reader is finished before we
                    //use the same command on MarkStatus()
                    registered = true;
                }
            }

            if (!registered)
            {
                //// since it doesn't have any status records, we register it
                //// as an initial record for all entries. less specific than
                //// createdAt, but tells us when any given node first encountered
                //// this entry.
                MarkStatus(listId, itemId, "Registered", cmd);
            }

            //select value
            cmd.Parameters.Clear(); //since we will be reusing command
            cmd.CommandText =
                "SELECT List.ListId, Item.ItemId, ItemValue, StatusValue " +
                "FROM List " +
                "JOIN junction_List_Item_Status " +
                "ON List.ListId = junction_List_Item_Status.ListId " +
                "JOIN Item " +
                "ON junction_List_Item_Status.ItemId = Item.ItemId " +
                "JOIN Status " +
                "ON junction_List_Item_Status.StatusId = Status.StatusId " +
                "WHERE List.ListId = @listId " +
                "AND Item.ItemId = @itemId";
            cmd.Parameters.AddWithValue("@listId", listId);
            cmd.Parameters.AddWithValue("@itemId", itemId);

            bool completed = false;
            bool archived = false;
            string itemValue = "";
            bool first = true;

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read()) //if statement cause we only need the first
                {
                    if (first)
                    {
                        itemValue = rdr.GetString(2);
                        first = false;
                    }

                    string statusValue = rdr.GetString(3);

                    if (completed == false &&
                        statusValue.Equals("Completed",
                            StringComparison.CurrentCultureIgnoreCase))
                    {
                        completed = true;
                    }

                    if (archived == false &&
                        statusValue.Equals("Archived",
                            StringComparison.CurrentCultureIgnoreCase))
                    {
                        archived = true;
                    }
                }
            }

            ToDoItem tdi = new ToDoItem()
            {
                ItemId = itemId,
                ListId = listId,
                Description = itemValue,
                Completed = completed,
                Archived = archived
            };

            return tdi;
        }

        /// <summary>
        /// looks up status value, if not found, inserts it, then
        /// either way, retrieves the id associated with it
        /// 
        /// if for any reason the insert fails and the 
        /// id cannot be found, returns -1
        /// </summary>
        /// <param name="statusVal"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private int EnsureIdForStatus(string statusVal, SQLiteCommand cmd)
        {
            int id = -1;

            //insert or ignore
            cmd.Parameters.Clear();
            cmd.CommandText =
                "INSERT OR IGNORE INTO Status (StatusValue) VALUES (@statusVal)";
            cmd.Parameters.AddWithValue("@statusVal", statusVal);
            cmd.ExecuteNonQuery();

            //select value
            cmd.Parameters.Clear(); //since we will be reusing command
            cmd.CommandText = "SELECT StatusId FROM Status " +
                              "WHERE StatusValue = @statusVal";
            cmd.Parameters.AddWithValue("@statusVal", statusVal);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read()) //if statement cause we only need the first
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        /// <summary>
        /// looks up item value, if not found, inserts it, then
        /// either way, retrieves the id associated with it
        /// 
        /// if for any reason the insert fails and the 
        /// id cannot be found, returns -1
        /// </summary>
        /// <param name="itemVal"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private int EnsureIdForItemValue(string itemVal, SQLiteCommand cmd)
        {
            int id = -1;

            //insert or ignore
            cmd.Parameters.Clear();
            cmd.CommandText =
                "INSERT OR IGNORE INTO Item (ItemValue) VALUES (@itemVal)";
            cmd.Parameters.AddWithValue("@itemVal", itemVal);
            cmd.ExecuteNonQuery();

            //select value
            cmd.Parameters.Clear(); //since we will be reusing command
            cmd.CommandText = "SELECT ItemId FROM Item WHERE ItemValue = @itemVal";
            cmd.Parameters.AddWithValue("@itemVal", itemVal);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read()) //if statement cause we only need the first
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        /// <summary>
        /// looks up list name, if not found, inserts it, then
        /// either way, retrieves the id associated with it
        /// 
        /// if for any reason the insert fails and the 
        /// id cannot be found, returns -1
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private int EnsureIdForListName(string listName, SQLiteCommand cmd)
        {
            int id = -1;

            //insert or ignore
            cmd.Parameters.Clear();
            cmd.CommandText =
                "INSERT OR IGNORE INTO List (ListName) VALUES (@listName)";
            cmd.Parameters.AddWithValue("@listName", listName);
            cmd.ExecuteNonQuery();

            //select value
            cmd.Parameters.Clear(); //since we will be reusing command
            cmd.CommandText = "SELECT ListId FROM List WHERE ListName = @listName";
            cmd.Parameters.AddWithValue("@listName", listName);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read()) //if statement cause we only need the first
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        public void SetActive(string listName, bool active, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                "UPDATE List SET ListActive = @active WHERE ListName = @listName";
            cmd.Parameters.AddWithValue("@active", active);
            cmd.Parameters.AddWithValue("@listName", listName);
            cmd.ExecuteNonQuery();
        }

        public void SetActive(string listName, bool active)
        {
            try
            {
                using (var conn =
                    new SQLiteConnection(@"Data Source=" +
                        Configuration.GetSqliteDbPath("nwd")))
                {
                    conn.Open();

                    using (var cmd = new SQLiteCommand(conn))
                    {
                        using (var transaction = conn.BeginTransaction())
                        {
                            SetActive(listName, active, cmd);

                            transaction.Commit();
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                //lets just throw it for now, but put something here eventually
                throw ex;
            }

        }

        public string Load(IListMatrix ilm, bool loadAllStatuses)
        {
            Dictionary<int, Dictionary<int, ToDoItem>> listItemStatuses =
                new Dictionary<int, Dictionary<int, ToDoItem>>();
            Dictionary<string, int> listNameToId =
                new Dictionary<string, int>();

            Stopwatch sw = Stopwatch.StartNew();

            foreach (ToDoList tdl in GetLists(true))
            {
                ilm.EnsureList(tdl.Name).AddWithMerge(tdl.Items);
            }

            sw.Stop();
            string time = sw.Elapsed.ToString("mm\\:ss\\.ff");

            return "Loaded: " + time;
        }

        public void UpdateActiveInactive(IEnumerable<ToDoList> setToActive, IEnumerable<ToDoList> setToInactive)
        {
            try
            {
                using (var conn =
                    new SQLiteConnection(@"Data Source=" +
                        Configuration.GetSqliteDbPath("nwd")))
                {
                    conn.Open();

                    using (var cmd = new SQLiteCommand(conn))
                    {
                        using (var transaction = conn.BeginTransaction())
                        {
                            foreach (ToDoList tdl in setToActive)
                            {
                                SetActive(tdl.Name, true, cmd);
                            }

                            foreach (ToDoList tdl in setToInactive)
                            {
                                SetActive(tdl.Name, false, cmd);
                            }

                            transaction.Commit();
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                //lets just throw it for now, but put something here eventually
                throw ex;
            }
        }

        /////////////////////////////////////connection/transaction template
        //    try
        //    {
        //        using (var conn =
        //            new SQLiteConnection(@"Data Source=" +
        //                Configuration.GetSqliteDbPath("nwd")))
        //        {
        //            conn.Open();

        //            using (var cmd = new SQLiteCommand(conn))
        //            {
        //                using (var transaction = conn.BeginTransaction())
        //                {

        //////////////////////////////////////////CODE HERE//////////////////////////////////////

        //                    transaction.Commit();
        //                }
        //            }

        //            conn.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //lets just throw it for now, but put something here eventually
        //        throw ex;
        //    }
    }
}