using NineWorldsDeep.Core;
using NineWorldsDeep.Sqlite;
using NineWorldsDeep.Synergy;
using NineWorldsDeep.Warehouse;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NineWorldsDeep.Db.Sqlite
{
    internal class DbAdapterV5 : DbAdapterBase, IDbAdapter
    {

        //public DbAdapterV5()
        //{
        //    InitializeIds(); //was in base class, caused errors
        //}

        /// <summary>
        /// returns -1 if not found, or id if found
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override int GetIdForHash(string hash, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear(); //since we will be reusing command

            cmd.CommandText =
                //"SELECT HashId FROM Hash WHERE HashValue = @hashVal";
                "SELECT " + NwdContract.COLUMN_HASH_ID +
                " FROM " + NwdContract.TABLE_HASH +
                " WHERE " + NwdContract.COLUMN_HASH_VALUE + " = @hashVal";

            cmd.Parameters.AddWithValue("@hashVal", hash);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        public override void InsertOrIgnoreHash(string hash, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText =
                //"INSERT OR IGNORE INTO Hash (HashValue) VALUES (@hash)";
                "INSERT OR IGNORE INTO " + NwdContract.TABLE_HASH +
                    " (" + NwdContract.COLUMN_HASH_VALUE + ") VALUES (@hash)";

            cmd.Parameters.AddWithValue("@hash", hash);
            cmd.ExecuteNonQuery();
        }

        public override int EnsureIdForValue(string tableName,
                                             string idColumnName,
                                             string valueColumnName,
                                             string valueToEnsure,
                                             SQLiteCommand cmd)
        {
            //prevent invalid names for columns and tables (according to NWD naming conventions)
            NwdUtils.ValidateColumnAndTableName(tableName);
            NwdUtils.ValidateColumnAndTableName(idColumnName);
            NwdUtils.ValidateColumnAndTableName(valueColumnName);

            int id = -1;

            //insert or ignore
            cmd.Parameters.Clear();
            cmd.CommandText =
                "INSERT OR IGNORE INTO " + tableName + " (" + valueColumnName + ") VALUES (@colVal)";
            cmd.Parameters.AddWithValue("@colVal", valueToEnsure);
            cmd.ExecuteNonQuery();

            //select value
            cmd.Parameters.Clear(); //since we will be reusing command
            cmd.CommandText = "SELECT " + idColumnName + " FROM " + tableName + " " +
                              "WHERE " + valueColumnName + " = @colVal";
            cmd.Parameters.AddWithValue("@colVal", valueToEnsure);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read()) //if statement cause we only need the first
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        public override void StoreTags(List<string> lst)
        {
            //INSERT OR IGNORE
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(GetDbName())))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (var tag in lst)
                        {
                            if (!string.IsNullOrWhiteSpace(tag))
                            {
                                cmd.CommandText =
                                    //"INSERT OR IGNORE INTO Tag (TagValue) VALUES (@tagValue)";
                                    "INSERT OR IGNORE INTO " +
                                        NwdContract.TABLE_TAG +
                                    " (" +
                                        NwdContract.COLUMN_TAG_VALUE +
                                    ") VALUES (@tagValue)";

                                cmd.Parameters.AddWithValue("@tagValue", tag);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        public override IEnumerable<SynergyList> GetLists(bool active)
        {
            Dictionary<string, SynergyList> lists =
                new Dictionary<string, SynergyList>();

            try
            {
                using (var conn =
                    new SQLiteConnection(@"Data Source=" +
                        Configuration.GetSqliteDbPath(GetDbName())))
                {
                    conn.Open();

                    using (var cmd = new SQLiteCommand(conn))
                    {
                        using (var transaction = conn.BeginTransaction())
                        {
                            string cmdString =
                                "SELECT sl." + NwdContract.COLUMN_SYNERGY_LIST_NAME + ", " +
                                       "si." + NwdContract.COLUMN_SYNERGY_ITEM_VALUE + ", " +
                                       "std." + NwdContract.COLUMN_SYNERGY_TO_DO_COMPLETED_AT + ", " +
                                       "std." + NwdContract.COLUMN_SYNERGY_TO_DO_ARCHIVED_AT + " " +
                                "FROM " + NwdContract.TABLE_SYNERGY_LIST + " sl " +
                                "JOIN " + NwdContract.TABLE_SYNERGY_LIST_ITEM + " sli " +
                                    "ON sl." + NwdContract.COLUMN_SYNERGY_LIST_ID + 
                                    " = sli." + NwdContract.COLUMN_SYNERGY_LIST_ID + " " +
                                "JOIN " + NwdContract.TABLE_SYNERGY_ITEM + " si " +
                                    "ON sli." + NwdContract.COLUMN_SYNERGY_ITEM_ID + 
                                    " = si." + NwdContract.COLUMN_SYNERGY_ITEM_ID + " " +
                                "JOIN " + NwdContract.TABLE_SYNERGY_TO_DO + " std " +
                                    "ON sli." + NwdContract.COLUMN_SYNERGY_LIST_ITEM_ID + " " +
                                    " = std." + NwdContract.COLUMN_SYNERGY_LIST_ITEM_ID + " " +
                                "WHERE (std." + NwdContract.COLUMN_SYNERGY_TO_DO_COMPLETED_AT + " IS NULL OR " +
                                       "std." + NwdContract.COLUMN_SYNERGY_TO_DO_COMPLETED_AT + " = '') " +
                                "AND (std." + NwdContract.COLUMN_SYNERGY_TO_DO_ARCHIVED_AT + " IS NULL OR " +
                                     "std." + NwdContract.COLUMN_SYNERGY_TO_DO_ARCHIVED_AT + " = '') ";

                            if (active)
                            {
                                cmdString += "AND sl." + NwdContract.COLUMN_SYNERGY_LIST_ACTIVATED_AT +
                                    " > sl." + NwdContract.COLUMN_SYNERGY_LIST_SHELVED_AT + " ";
                            }

                            cmd.CommandText = cmdString;

                            //cmd.Parameters.AddWithValue("@active", active);

                            using (var rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    string listName = rdr.GetString(0);
                                    string itemValue = rdr.GetString(1);
                                    string completedAt = "";
                                    string archivedAt = "";

                                    if (!rdr.IsDBNull(2))
                                    {
                                        completedAt = rdr.GetString(2);
                                    }

                                    if (!rdr.IsDBNull(3))
                                    {
                                        archivedAt = rdr.GetString(3);
                                    }

                                    if (!lists.ContainsKey(listName))
                                    {
                                        lists[listName] = new SynergyList()
                                        {
                                            Name = listName
                                        };
                                    }

                                    SynergyItem si = new SynergyItem();

                                    si.TurnOffFragmentUpdating();
                                    si.Item = itemValue;
                                    si.CompletedAt = completedAt;
                                    si.ArchivedAt = archivedAt;
                                    si.TurnOnFragmentUpdatingAndUpdate();

                                    lists[listName].AddItem(si);
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

            return lists.Values
                .OrderBy(x => x.Name)
                .ToList<SynergyList>();
        }

        public override void SetActive(string listName, bool active, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            string cmdString = "UPDATE " + NwdContract.TABLE_SYNERGY_LIST + " ";

            if (active)
            {
                cmdString += "SET " + NwdContract.COLUMN_SYNERGY_LIST_ACTIVATED_AT +
                    " = CURRENT_TIMESTAMP ";
            }
            else
            {
                cmdString += "SET " + NwdContract.COLUMN_SYNERGY_LIST_SHELVED_AT +
                    " = CURRENT_TIMESTAMP ";
            }

            cmdString = "WHERE " + NwdContract.COLUMN_SYNERGY_LIST_NAME + " = @listName" ;

            cmd.CommandText = cmdString;

            cmd.Parameters.AddWithValue("@listName", listName);
            cmd.ExecuteNonQuery();
        }

        public override void StorePaths(List<string> lst)
        {
            //INSERT OR IGNORE
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(GetDbName())))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (var path in lst)
                        {
                            if (!string.IsNullOrWhiteSpace(path))
                            {
                                cmd.CommandText =
                                    //"INSERT OR IGNORE INTO Path (PathValue) VALUES (@pathValue)";
                                    "INSERT OR IGNORE INTO " +
                                        NwdContract.TABLE_PATH +
                                    " (" +
                                        NwdContract.COLUMN_PATH_VALUE +
                                    ") VALUES (@pathValue)";

                                cmd.Parameters.AddWithValue("@pathValue", path);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        public override void UpsertFragment(int listId, int itemId, SynergyItem si, SQLiteCommand cmd)
        {
            //need to do a separate update attemp for
            //each value to avoid overwriting existing
            //values or ignoring updates to rows
            //with some values already populated
            //but others still null

            //store updatedAt, we will update this along with
            //fragment value as those are not conditional
            //updates (always overwritten on every storage op)
            string updatedAt =
                NwdUtils.GetTimeStamp_yyyyMMddHHmmss();

            int synergyListItemId = EnsureSynergyListItemId(listId, itemId, cmd);

            //attempt update for CompletedAt
            cmd.Parameters.Clear();
            cmd.CommandText =
                "UPDATE OR IGNORE " + NwdContract.TABLE_SYNERGY_TO_DO + " " +
                "SET " + NwdContract.COLUMN_SYNERGY_TO_DO_COMPLETED_AT + " = @completedAt " +
                "WHERE " + NwdContract.COLUMN_SYNERGY_LIST_ITEM_ID + " = @synergyListItemId " +
                "AND " + NwdContract.COLUMN_SYNERGY_TO_DO_COMPLETED_AT + " IS NOT @completedAt " +
                "AND (" + NwdContract.COLUMN_SYNERGY_TO_DO_COMPLETED_AT + " IS NULL " +
                     "OR " + NwdContract.COLUMN_SYNERGY_TO_DO_COMPLETED_AT + " = '') ";

            cmd.Parameters.AddWithValue("@completedAt", si.CompletedAt);
            cmd.Parameters.AddWithValue("@synergyListItemId", synergyListItemId);
            cmd.ExecuteNonQuery();

            //attempt update for ArchivedAt
            cmd.Parameters.Clear();
            cmd.CommandText =
                "UPDATE OR IGNORE " + NwdContract.TABLE_SYNERGY_TO_DO + " " +
                "SET " + NwdContract.COLUMN_SYNERGY_TO_DO_ARCHIVED_AT + " = @archivedAt " +
                "WHERE " + NwdContract.COLUMN_SYNERGY_LIST_ITEM_ID + " = @synergyListItemId " +
                "AND " + NwdContract.COLUMN_SYNERGY_TO_DO_ARCHIVED_AT + " IS NOT @archivedAt " +
                "AND (" + NwdContract.COLUMN_SYNERGY_TO_DO_ARCHIVED_AT + " IS NULL " +
                     "OR " + NwdContract.COLUMN_SYNERGY_TO_DO_ARCHIVED_AT + " = '') ";

            cmd.Parameters.AddWithValue("@archivedAt", si.ArchivedAt);
            cmd.Parameters.AddWithValue("@synergyListItemId", synergyListItemId);
            cmd.ExecuteNonQuery();

            //SHOULDN'T NEED THIS WITH V5... leaving it here until tested
            ////attempt update for Fragment
            //cmd.Parameters.Clear();
            //cmd.CommandText =adsf
            //    "UPDATE OR IGNORE " + NwdContract.TABLE_FRAGMENT + " " +
            //    "SET " + NwdContract.COLUMN_SYNERGY_LIST_ITEM_UPDATED_AT + " = @updatedAt " +
            //    "WHERE " + NwdContract.COLUMN_LIST_ID + " = @listId " +
            //    "AND " + NwdContract.COLUMN_ITEM_ID + " = @itemId ";

            //cmd.Parameters.AddWithValue("@fragVal", si.Fragment);
            //cmd.Parameters.AddWithValue("@updatedAt", updatedAt);
            //cmd.Parameters.AddWithValue("@listId", listId);
            //cmd.Parameters.AddWithValue("@itemId", itemId);
            //cmd.ExecuteNonQuery();


            //if all the above fail and are ignored, this will work
            //attempt insert for Fragment
            cmd.Parameters.Clear();
            cmd.CommandText =
            "INSERT OR IGNORE INTO " + NwdContract.TABLE_SYNERGY_TO_DO  +
                " (" + NwdContract.COLUMN_SYNERGY_LIST_ITEM_ID + ", " +
                    "" + NwdContract.COLUMN_SYNERGY_TO_DO_COMPLETED_AT + ", " +
                    "" + NwdContract.COLUMN_SYNERGY_TO_DO_ARCHIVED_AT + ", " +
                    "" + NwdContract.COLUMN_SYNERGY_TO_DO_UPDATED_AT + ") " +
            "VALUES (@listItemId, " +
                    "@completedAt, " +
                    "@archivedAt, " +
                    "@updatedAt) ";

            cmd.Parameters.AddWithValue("@listItemId", synergyListItemId);
            cmd.Parameters.AddWithValue("@completedAt", si.CompletedAt);
            cmd.Parameters.AddWithValue("@archivedAt", si.ArchivedAt);
            cmd.Parameters.AddWithValue("@updatedAt", updatedAt);
            cmd.ExecuteNonQuery();
        }

        private int EnsureSynergyListItemId(int listId, int itemId, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText =
                "INSERT OR IGNORE INTO " + NwdContract.TABLE_SYNERGY_LIST_ITEM +
                " (" +
                    NwdContract.COLUMN_SYNERGY_LIST_ID + ", " +
                    NwdContract.COLUMN_SYNERGY_ITEM_ID + 
                 ") VALUES (@listId, @itemId)";

            cmd.Parameters.AddWithValue("@listId", listId);
            cmd.Parameters.AddWithValue("@itemId", itemId);
            cmd.ExecuteNonQuery();

            //select value
            cmd.Parameters.Clear(); //since we will be reusing command

            cmd.CommandText =
                "SELECT " + NwdContract.COLUMN_SYNERGY_LIST_ITEM_ID +
                " FROM " + NwdContract.TABLE_SYNERGY_LIST_ITEM +
                " WHERE " + NwdContract.COLUMN_SYNERGY_LIST_ID + " = @listId " +
                " AND " + NwdContract.COLUMN_SYNERGY_ITEM_ID + " = @itemId ";

            cmd.Parameters.AddWithValue("@listId", listId);
            cmd.Parameters.AddWithValue("@itemId", itemId);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read()) //if statement cause we only need the first
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        public override string GetTagsForHash(string sha1Hash, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText =
                //"SELECT t.TagValue " +
                //"FROM Tag t " +
                //"JOIN FileTag ft " +
                //"ON t.TagId = ft.TagId " +
                //"JOIN File f " +
                //"ON f.FileId = ft.FileId " +
                //"JOIN FileHash fh " +
                //"ON f.FileId = fh.FileId" +
                //"JOIN Hash h " +
                //"ON h.HashId = fh.HashId " +
                //"WHERE h.HashValue = @hash "
                "SELECT t." + NwdContract.COLUMN_TAG_VALUE + " " +
                "FROM " + NwdContract.TABLE_TAG + " t " +
                "JOIN " + NwdContract.TABLE_FILE_TAG + " ft " +
                "ON t." + NwdContract.COLUMN_TAG_ID + " = ft." + NwdContract.COLUMN_TAG_ID + " " +
                "JOIN " + NwdContract.TABLE_FILE + " f " +
                "ON f." + NwdContract.COLUMN_FILE_ID + " = ft." + NwdContract.COLUMN_FILE_ID + " " +
                "JOIN " + NwdContract.TABLE_FILE_HASH + " fh " +
                "ON f." + NwdContract.COLUMN_FILE_ID + " = fh." + NwdContract.COLUMN_FILE_ID + "" +
                "JOIN " + NwdContract.TABLE_HASH + " h " +
                "ON h." + NwdContract.COLUMN_HASH_ID + " = fh." + NwdContract.COLUMN_HASH_ID + " " +
                "WHERE h." + NwdContract.COLUMN_HASH_VALUE + " = @hash ";

            cmd.Parameters.AddWithValue("@hash", sha1Hash);

            List<string> tags = new List<string>();

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string tag = rdr.GetString(0);

                    tags.Add(tag);
                }
            }

            return string.Join(", ", tags);
        }

        /// <summary>
        /// will retrieve load all paths and tags for those paths
        /// where the path begins with filePathTopFolder
        /// 
        /// any level of hierarchy is supported (i.e. "C:\NWD-AUX\" 
        /// will work, as will "C:\NWD-AUX\voicememos", with
        /// the first one including the results from the second
        /// one, thus respecting hierarchy)
        /// 
        /// Also note, passing "" as a parameter will match
        /// all paths for which tags have been stored
        /// </summary>
        /// <param name="filePathTopFolder"></param>
        public override List<PathTagLink> GetPathTagLinks(string filePathTopFolder)
        {
            //TODO: this should take device into account (overload method to default to this device if one is not supplied)
            List<PathTagLink> lst = new List<PathTagLink>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(GetDbName())))
            {
                conn.Open();

                //TODO: may be able to move path filter into a LIKE clause
                //but should test which is faster, for now just
                //filtering in code
                string cmdStr =
                    "SELECT " +
                        NwdContract.COLUMN_PATH_VALUE + ", " +
                        NwdContract.COLUMN_TAG_VALUE + " " +
                    "FROM " + NwdContract.TABLE_PATH + " " +
                    "JOIN " + NwdContract.TABLE_FILE + " " +
                    "ON " +
                        NwdContract.TABLE_PATH + "." +
                            NwdContract.COLUMN_PATH_ID +
                        " = " +
                        NwdContract.TABLE_FILE + "." +
                            NwdContract.COLUMN_PATH_ID + " " +
                    "JOIN " + NwdContract.TABLE_FILE_TAG + " " +
                    "ON " +
                        NwdContract.TABLE_FILE + "." +
                            NwdContract.COLUMN_FILE_ID +
                        " = " +
                        NwdContract.TABLE_FILE_TAG + "." +
                            NwdContract.COLUMN_FILE_ID + " " +
                    "JOIN " + NwdContract.TABLE_TAG + " " +
                    "ON " +
                        NwdContract.TABLE_FILE_TAG + "." +
                            NwdContract.COLUMN_TAG_ID +
                        " = " +
                        NwdContract.TABLE_TAG + "." +
                            NwdContract.COLUMN_TAG_ID + "";

                using (var cmd = new SQLiteCommand(cmdStr, conn))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string pathVal = rdr.GetString(0);
                            string tagVal = rdr.GetString(1);

                            if (pathVal.ToLower().StartsWith(filePathTopFolder.ToLower()))
                            {
                                //TODO: may be able to move this into a LIKE clause
                                //but should test which is faster, for now just
                                //filtering in code
                                lst.Add(new PathTagLink()
                                {
                                    PathValue = pathVal,
                                    TagValue = tagVal
                                });
                            }
                        }
                    }
                }
            }

            return lst;
        }

        public override string DeleteFile(String device, String path)
        {
            if (string.IsNullOrWhiteSpace(device))
            {
                device = Configuration.GetLocalDeviceDescription();
            }

            string outputMsg = "implementation in progress";
            string time = "";

            try
            {
                using (var conn =
                    new SQLiteConnection(@"Data Source=" +
                        Configuration.GetSqliteDbPath(GetDbName())))
                {
                    conn.Open();

                    using (var cmd = new SQLiteCommand(conn))
                    {
                        using (var transaction = conn.BeginTransaction())
                        {
                            Stopwatch sw = Stopwatch.StartNew();

                            ////////////////////////////////////////CODE HERE//////////////////////////////////////

                            //get FileId for device path
                            int fileId = GetFileIdForDevicePath(device, path, cmd);

                            //remove all FileTags for FileId
                            DeleteFileTagsForFileId(fileId, cmd);

                            //remove FileEntry for FileId
                            DeleteFileForFileId(fileId, cmd);

                            transaction.Commit();

                            sw.Stop();
                            time = sw.Elapsed.ToString("mm\\:ss\\.ff");
                        }
                    }

                    conn.Close();
                }

                outputMsg = "Finished: " + time;
            }
            catch (Exception ex)
            {
                outputMsg = "error: " + ex.Message;
            }

            return outputMsg;
        }

        public override int GetFileIdForDevicePath(String device,
                                          String path,
                                          SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText =
                "SELECT " + NwdContract.COLUMN_FILE_ID + " " +
                "FROM " + NwdContract.TABLE_FILE + " f " +
                "JOIN " + NwdContract.TABLE_PATH + " p  " +
                "ON f." + NwdContract.COLUMN_PATH_ID +
                    " = p." + NwdContract.COLUMN_PATH_ID + " " +
                "JOIN " + NwdContract.TABLE_DEVICE + " d  " +
                "ON d." + NwdContract.COLUMN_DEVICE_ID +
                    " = f." + NwdContract.COLUMN_DEVICE_ID + " " +
                "WHERE d." + NwdContract.COLUMN_DEVICE_DESCRIPTION + " = ? " +
                "AND p." + NwdContract.COLUMN_PATH_VALUE + " = ? ";

            SQLiteParameter deviceParam = new SQLiteParameter();
            deviceParam.Value = device;
            cmd.Parameters.Add(deviceParam);

            SQLiteParameter pathParam = new SQLiteParameter();
            pathParam.Value = path;
            cmd.Parameters.Add(pathParam);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        public override int GetIdForFile(int deviceId, int pathId, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear(); //since we will be reusing command

            cmd.CommandText =
                //"SELECT FileId FROM File " +
                //              "WHERE DeviceId = @deviceId " +
                //              "AND PathId = @pathId ";
                "SELECT " + NwdContract.COLUMN_FILE_ID +
                " FROM " + NwdContract.TABLE_FILE +
                " WHERE " + NwdContract.COLUMN_DEVICE_ID + " = @deviceId " +
                "AND " + NwdContract.COLUMN_PATH_ID + " = @pathId ";

            cmd.Parameters.AddWithValue("@deviceId", deviceId);
            cmd.Parameters.AddWithValue("@pathId", pathId);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        private int EnsureFile(int deviceId, int pathId, SQLiteCommand cmd)
        {
            //INSERT or IGNORE
            cmd.Parameters.Clear();
            cmd.CommandText =
                "INSERT OR IGNORE INTO " + NwdContract.TABLE_FILE +
                " (" + NwdContract.COLUMN_DEVICE_ID + ", " +
                  "" + NwdContract.COLUMN_PATH_ID + ", " + ") " +
                "VALUES (@deviceId, " +
                        "@pathId)";

            cmd.Parameters.AddWithValue("@deviceId", deviceId);
            cmd.Parameters.AddWithValue("@pathId", pathId);
            cmd.ExecuteNonQuery();

            return GetIdForFile(deviceId, pathId, cmd); 
        }

        public override int UpsertFile(int deviceId, int pathId, int hashId, string hashedAtTimeStamp, SQLiteCommand cmd)
        {
            int fileId = EnsureFile(deviceId, pathId, cmd);

            LinkFileIdToHashId(fileId, hashId, cmd);

            return fileId;
        }

        private void LinkFileIdToHashId(int fileId, int hashId, SQLiteCommand cmd)
        {
            //INSERT or IGNORE
            cmd.Parameters.Clear();
            cmd.CommandText =
                "INSERT OR IGNORE INTO " + NwdContract.TABLE_FILE_HASH +
                " (" + NwdContract.COLUMN_FILE_ID + ", " +
                  "" + NwdContract.COLUMN_HASH_ID + ") " +
                "VALUES (@fileId, " +
                        "@hashId)";
            
            cmd.Parameters.AddWithValue("@fileId", fileId);
            cmd.Parameters.AddWithValue("@hashId", hashId);
            cmd.ExecuteNonQuery();
        }

        public override void LinkFileIdToTagId(int fileId, int tagId, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText =                
                "INSERT OR IGNORE INTO " +
                    NwdContract.TABLE_FILE_TAG +
                    " (" +
                        NwdContract.COLUMN_FILE_ID + ", " +
                        NwdContract.COLUMN_TAG_ID +
                     ") " +
                "VALUES (@fileId, @tagId) ";

            cmd.Parameters.AddWithValue("@fileId", fileId);
            cmd.Parameters.AddWithValue("@tagId", tagId);
            cmd.ExecuteNonQuery();
        }

        public override void DeleteFileTagsForFileId(int fileId, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                "DELETE FROM " + NwdContract.TABLE_FILE_TAG +
                " WHERE " + NwdContract.COLUMN_FILE_ID + " = ? ";

            SQLiteParameter fileIdParam = new SQLiteParameter();
            fileIdParam.Value = fileId;
            cmd.Parameters.Add(fileIdParam);

            cmd.ExecuteNonQuery();
        }

        public override void DeleteFileForFileId(int fileId, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                "DELETE FROM " + NwdContract.TABLE_FILE +
                " WHERE " + NwdContract.COLUMN_FILE_ID + " = ? ";

            SQLiteParameter fileIdParam = new SQLiteParameter();
            fileIdParam.Value = fileId;
            cmd.Parameters.Add(fileIdParam);

            cmd.ExecuteNonQuery();
        }

        public override int GetIdForPath(string path, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear(); //since we will be reusing command

            cmd.CommandText =
                //"SELECT PathId FROM Path WHERE PathValue = @pathVal";
                "SELECT " + NwdContract.COLUMN_PATH_ID +
                " FROM " + NwdContract.TABLE_PATH +
                " WHERE " + NwdContract.COLUMN_PATH_VALUE + " = @pathVal";

            cmd.Parameters.AddWithValue("@pathVal", path);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        public override void RefreshPathIds(Dictionary<string, int> pathIds, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear(); //since we will be reusing command

            cmd.CommandText =
                //"SELECT PathId, PathValue FROM Path";
                "SELECT " +
                    NwdContract.COLUMN_PATH_ID + ", " +
                    NwdContract.COLUMN_PATH_VALUE +
                " FROM " + NwdContract.TABLE_PATH + "";

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    int id = rdr.GetInt32(0);
                    string path = rdr.GetString(1);

                    //only populate keys in dictionary
                    if (pathIds.ContainsKey(path))
                    {
                        pathIds[path] = id;
                    }
                }
            }
        }

        public override void PopulateSyncMaps(SyncProfile sp, SQLiteCommand cmd)
        {
            sp.SyncMaps.Clear();

            cmd.Parameters.Clear(); //since we will be reusing command
            cmd.CommandText =
                //"SELECT sp.SyncProfileName, " +
                //        "pSrc.PathValue AS SourcePath,  " +
                //        "pDst.PathValue AS DestPath, " +
                //        "sm.SyncDirectionId, " +
                //        "sm.SyncActionIdDefault " +
                //"FROM SyncMap AS sm " +
                //"JOIN SyncProfile AS sp " +
                //"ON sm.SyncProfileId = sp.SyncProfileId " +
                //"JOIN Path AS pSrc " +
                //"ON pSrc.PathId = sm.PathIdSource " +
                //"JOIN Path AS pDst " +
                //"ON pDst.PathId = sm.PathIdDestination " +
                //"WHERE sp.SyncProfileName = @name ";
                "SELECT sp." + NwdContract.COLUMN_SYNC_PROFILE_NAME + ", " +
                        "pSrc." + NwdContract.COLUMN_PATH_VALUE + " AS SourcePath,  " +
                        "pDst." + NwdContract.COLUMN_PATH_VALUE + " AS DestPath, " +
                        "sm." + NwdContract.COLUMN_SYNC_DIRECTION_ID + ", " +
                        "sm." + NwdContract.COLUMN_SYNC_ACTION_ID_DEFAULT + " " +
                "FROM " + NwdContract.TABLE_SYNC_MAP + " AS sm " +
                "JOIN " + NwdContract.TABLE_SYNC_PROFILE + " AS sp " +
                "ON sm." + NwdContract.COLUMN_SYNC_PROFILE_ID + " = sp." + NwdContract.COLUMN_SYNC_PROFILE_ID + " " +
                "JOIN " + NwdContract.TABLE_PATH + " AS pSrc " +
                "ON pSrc." + NwdContract.COLUMN_PATH_ID + " = sm." + NwdContract.COLUMN_PATH_ID_SOURCE + " " +
                "JOIN " + NwdContract.TABLE_PATH + " AS pDst " +
                "ON pDst." + NwdContract.COLUMN_PATH_ID + " = sm." + NwdContract.COLUMN_PATH_ID_DESTINATION + " " +
                "WHERE sp." + NwdContract.COLUMN_SYNC_PROFILE_NAME + " = @name ";

            cmd.Parameters.AddWithValue("@name", sp.Name);

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string source = rdr.GetString(1);
                    string destination = rdr.GetString(2);
                    int directionId = rdr.GetInt32(3);
                    int actionId = rdr.GetInt32(4);

                    SyncDirection direction = idDirections[directionId];
                    SyncAction action = idActions[actionId];

                    sp.SyncMaps.Add(new SyncMap(sp,
                                                direction,
                                                action)
                    {
                        Source = source,
                        Destination = destination
                    });
                }
            }
        }

        public override void RefreshProfileIds(SQLiteCommand cmd)
        {
            cmd.Parameters.Clear(); //since we will be reusing command
            cmd.CommandText =
                //"SELECT SyncProfileId, SyncProfileName FROM SyncProfile";
                "SELECT " + NwdContract.COLUMN_SYNC_PROFILE_ID + ", " +
                            NwdContract.COLUMN_SYNC_PROFILE_NAME +
                " FROM " + NwdContract.TABLE_SYNC_PROFILE + "";

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    int id = rdr.GetInt32(0);
                    string name = rdr.GetString(1);

                    nameIds[name] = id;
                    idNames[id] = name;

                }
            }
        }

        public override void RefreshActionIds(SQLiteCommand cmd)
        {
            cmd.Parameters.Clear(); //since we will be reusing command
            cmd.CommandText =
                //"SELECT SyncActionId, SyncActionValue FROM SyncAction";
                "SELECT " +
                    NwdContract.COLUMN_SYNC_ACTION_ID + ", " +
                    NwdContract.COLUMN_SYNC_ACTION_VALUE +
                " FROM " + NwdContract.TABLE_SYNC_ACTION + "";

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    int id = rdr.GetInt32(0);
                    string actionValue = rdr.GetString(1);
                    SyncAction action =
                        (SyncAction)Enum.Parse(typeof(SyncAction),
                                               actionValue);
                    if (Enum.IsDefined(typeof(SyncAction), action))
                    {
                        actionIds[action] = id;
                        idActions[id] = action;
                    }
                }
            }
        }

        public override void InsertOrIgnoreDirection(SyncDirection direction, SQLiteCommand cmd)
        {
            string directionVal = direction.ToString();
            cmd.Parameters.Clear();

            cmd.CommandText =
                //"INSERT OR IGNORE INTO SyncDirection (SyncDirectionValue) VALUES (@directionVal)";
                "INSERT OR IGNORE INTO " + NwdContract.TABLE_SYNC_DIRECTION + " (" + NwdContract.COLUMN_SYNC_DIRECTION_VALUE + ") VALUES (@directionVal)";

            cmd.Parameters.AddWithValue("@directionVal", directionVal);
            cmd.ExecuteNonQuery();
        }

        public override void InsertOrIgnoreAction(SyncAction action, SQLiteCommand cmd)
        {
            string actionVal = action.ToString();
            cmd.Parameters.Clear();

            cmd.CommandText =
                //"INSERT OR IGNORE INTO SyncAction (SyncActionValue) VALUES (@actionVal)";
                "INSERT OR IGNORE INTO " + NwdContract.TABLE_SYNC_ACTION +
                " (" + NwdContract.COLUMN_SYNC_ACTION_VALUE +
                ") VALUES (@actionVal)";

            cmd.Parameters.AddWithValue("@actionVal", actionVal);
            cmd.ExecuteNonQuery();
        }

        public override void RefreshDirectionIds(SQLiteCommand cmd)
        {
            cmd.Parameters.Clear(); //since we will be reusing command

            cmd.CommandText =
                //"SELECT SyncDirectionId, SyncDirectionValue FROM SyncDirection";
                "SELECT " +
                    NwdContract.COLUMN_SYNC_DIRECTION_ID + ", " +
                    NwdContract.COLUMN_SYNC_DIRECTION_VALUE +
                " FROM " + NwdContract.TABLE_SYNC_DIRECTION + "";

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    int id = rdr.GetInt32(0);
                    string directionValue = rdr.GetString(1);
                    SyncDirection direction =
                        (SyncDirection)Enum.Parse(typeof(SyncDirection),
                                                  directionValue);
                    if (Enum.IsDefined(typeof(SyncDirection), direction))
                    {
                        directionIds[direction] = id;
                        idDirections[id] = direction;
                    }
                }
            }
        }

        public override void InsertOrIgnorePath(string path, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText =
                //"INSERT OR IGNORE INTO Path (PathValue) VALUES (@path)";
                "INSERT OR IGNORE INTO " + NwdContract.TABLE_PATH +
                " (" + NwdContract.COLUMN_PATH_VALUE + ") VALUES (@path)";

            cmd.Parameters.AddWithValue("@path", path);
            cmd.ExecuteNonQuery();
        }

        public override void PopulatePathIds(Dictionary<string, int> pathsToIds)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(GetDbName())))
            {
                conn.Open();

                string cmdStr =
                    //"SELECT PathValue, PathId FROM Path";
                    "SELECT " +
                        NwdContract.COLUMN_PATH_VALUE + ", " +
                        NwdContract.COLUMN_PATH_ID +
                    " FROM " + NwdContract.TABLE_PATH + "";

                using (var cmd = new SQLiteCommand(cmdStr, conn))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string pathVal = rdr.GetString(0);
                            int pathId = rdr.GetInt32(1);

                            //only store those we are looking for
                            if (pathsToIds.ContainsKey(pathVal))
                            {
                                pathsToIds[pathVal] = pathId;
                            }
                        }
                    }
                }
            }
        }

        public override void StoreFileTags(List<PathToTagMapping> mappings)
        {
            //INSERT OR IGNORE
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(GetDbName())))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (var mapping in mappings)
                        {
                            cmd.CommandText =
                                "INSERT OR IGNORE INTO " +
                                    NwdContract.TABLE_FILE_TAG +
                                    " (" +
                                        NwdContract.COLUMN_FILE_ID + ", " +
                                        NwdContract.COLUMN_TAG_ID +
                                    ") VALUES (@fileId, @tagId)";

                            cmd.Parameters.AddWithValue("@fileId", mapping.FileId);
                            cmd.Parameters.AddWithValue("@tagId", mapping.TagId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        public override void StoreDeviceFiles(List<PathToTagMapping> mappings)
        {
            //INSERT OR IGNORE
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(GetDbName())))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (var mapping in mappings)
                        {
                            cmd.CommandText =
                                //"INSERT OR IGNORE INTO File (DeviceId, PathId) VALUES (@deviceId, @pathId)";
                                "INSERT OR IGNORE INTO " +
                                    NwdContract.TABLE_FILE +
                                        " (" +
                                            NwdContract.COLUMN_DEVICE_ID + ", " +
                                            NwdContract.COLUMN_PATH_ID +
                                         ") VALUES (@deviceId, @pathId)";

                            cmd.Parameters.AddWithValue("@deviceId", mapping.DeviceId);
                            cmd.Parameters.AddWithValue("@pathId", mapping.PathId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        public override void PopulateFileIds(List<PathToTagMapping> mappings)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(GetDbName())))
            {
                conn.Open();

                string cmdStr =
                    //"SELECT FileId, DeviceId, PathId FROM File";
                    "SELECT " +
                        NwdContract.COLUMN_FILE_ID + ", " +
                        NwdContract.COLUMN_DEVICE_ID + ", " +
                        NwdContract.COLUMN_PATH_ID +
                    " FROM " + NwdContract.TABLE_FILE + "";

                using (var cmd = new SQLiteCommand(cmdStr, conn))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            int fileId = rdr.GetInt32(0);
                            int deviceId = rdr.GetInt32(1);
                            int pathId = rdr.GetInt32(2);

                            //if this is too slow, we could implement
                            //a dictionary with a File entity type or something
                            //for now, this might be good enough
                            foreach (var m in mappings)
                            {
                                if (m.DeviceId == deviceId &&
                                    m.PathId == pathId)
                                {
                                    m.FileId = fileId;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void PopulateSyncProfiles(List<SyncProfile> lst, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear(); //since we will be reusing command
            cmd.CommandText =
                //"SELECT SyncProfileName FROM SyncProfile";
                "SELECT " +
                    NwdContract.COLUMN_SYNC_PROFILE_NAME +
                " FROM " + NwdContract.TABLE_SYNC_PROFILE + " ";

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string name = rdr.GetString(0);

                    SyncProfile sp = new SyncProfile(name);

                    lst.Add(sp);
                }
            }

            //cannot set command text while reader in use,
            //so need to populate after reader completes
            foreach (SyncProfile sp in lst)
            {
                PopulateSyncMaps(sp, cmd);
            }
        }

        public override void DeleteSyncMap(SyncMap sm, SQLiteCommand cmd)
        {
            Dictionary<string, int> pathIds =
                new Dictionary<string, int>();

            pathIds.Add(sm.Source, -1);
            pathIds.Add(sm.Destination, -1);
            RefreshPathIds(pathIds, cmd);

            cmd.Parameters.Clear();

            cmd.CommandText =
                "DELETE FROM " + NwdContract.TABLE_SYNC_MAP + " " +
                "WHERE " + NwdContract.COLUMN_SYNC_PROFILE_ID + " = @profileId " +
                "AND " + NwdContract.COLUMN_PATH_ID_SOURCE + " = @srcId " +
                "AND " + NwdContract.COLUMN_PATH_ID_DESTINATION + " = @destId " +
                "AND " + NwdContract.COLUMN_SYNC_DIRECTION_ID + " = @directionId";

            cmd.Parameters.AddWithValue("@profileId", GetNameId(sm.Profile.Name));
            cmd.Parameters.AddWithValue("@srcId", pathIds[sm.Source]);
            cmd.Parameters.AddWithValue("@destId", pathIds[sm.Destination]);
            cmd.Parameters.AddWithValue("@directionId", GetDirectionId(sm.SyncDirection));

            cmd.ExecuteNonQuery();
        }

        public override List<string> GetColumnNames(string tableName, SQLiteCommand cmd)
        {
            Regex regex = new Regex("^[a-zA-Z0-9_]*$");
            if (!regex.IsMatch(tableName))
            {
                throw new ArgumentException("invalid tableName: " + tableName);
            }

            List<string> cols = new List<string>();

            cmd.Parameters.Clear();
            cmd.CommandText = "PRAGMA table_info('" + tableName + "')";
            //cmd.Parameters.AddWithValue("@tableName", tableName);            

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    //PRAGMA result indexes
                    //0: cid - column id
                    //1: name - column name
                    //2: type - column type
                    //3: notnull - 0 or 1
                    //4: dflt_value - default value
                    //5: pk - 0 or 1 

                    string columnName = rdr.GetString(1);
                    cols.Add(columnName);
                }
            }

            return cols;
        }

        public override void UpsertSyncMap(int profileId, int srcId, int destId, int directionId, int actionId, SQLiteCommand cmd)
        {
            /////////see link answers below accepted answer
            //http://stackoverflow.com/questions/15277373/sqlite-upsert-update-or-insert

            // need to do an UPDATE or IGNORE followed by an INSERT or IGNORE, so the
            // update attempts first (to change default action id for example), and
            // if not found, gets ignored, so the insert would then fire.
            // if the update did succeed the insert would get ignored
            // so doing it this way, in this order, ensures 
            // a proper "UPSERT"

            //UPDATE or IGNORE
            cmd.Parameters.Clear();

            cmd.CommandText =
                //"UPDATE OR IGNORE SyncMap " +
                //"SET SyncActionIdDefault = @actionId " +
                //"WHERE SyncProfileId = @profileId " +
                //"AND PathIdSource = @srcId " +
                //"AND PathIdDestination = @destId " +
                //"AND SyncDirectionId = @directionId";
                "UPDATE OR IGNORE " + NwdContract.TABLE_SYNC_MAP + " " +
                "SET " + NwdContract.COLUMN_SYNC_ACTION_ID_DEFAULT + " = @actionId " +
                "WHERE " + NwdContract.COLUMN_SYNC_PROFILE_ID + " = @profileId " +
                "AND " + NwdContract.COLUMN_PATH_ID_SOURCE + " = @srcId " +
                "AND " + NwdContract.COLUMN_PATH_ID_DESTINATION + " = @destId " +
                "AND " + NwdContract.COLUMN_SYNC_DIRECTION_ID + " = @directionId";

            cmd.Parameters.AddWithValue("@profileId", profileId);
            cmd.Parameters.AddWithValue("@srcId", srcId);
            cmd.Parameters.AddWithValue("@destId", destId);
            cmd.Parameters.AddWithValue("@directionId", directionId);
            cmd.Parameters.AddWithValue("@actionId", actionId);
            cmd.ExecuteNonQuery();

            //INSERT or IGNORE
            cmd.Parameters.Clear();
            cmd.CommandText =
                //"INSERT OR IGNORE INTO SyncMap (SyncProfileId, " +
                //                               "PathIdSource, " +
                //                               "PathIdDestination, " +
                //                               "SyncDirectionId, " +
                //                               "SyncActionIdDefault) " +
                //"VALUES (@profileId, " +
                //        "@srcId, " +
                //        "@destId, " +
                //        "@directionId, " +
                //        "@actionId)";
                "INSERT OR IGNORE INTO " +

                    NwdContract.TABLE_SYNC_MAP +

                        " (" + NwdContract.COLUMN_SYNC_PROFILE_ID + ", " +
                          "" + NwdContract.COLUMN_PATH_ID_SOURCE + ", " +
                          "" + NwdContract.COLUMN_PATH_ID_DESTINATION + ", " +
                          "" + NwdContract.COLUMN_SYNC_DIRECTION_ID + ", " +
                          "" + NwdContract.COLUMN_SYNC_ACTION_ID_DEFAULT + ") " +

                "VALUES (@profileId, " +
                        "@srcId, " +
                        "@destId, " +
                        "@directionId, " +
                        "@actionId)";


            cmd.Parameters.AddWithValue("@profileId", profileId);
            cmd.Parameters.AddWithValue("@srcId", srcId);
            cmd.Parameters.AddWithValue("@destId", destId);
            cmd.Parameters.AddWithValue("@directionId", directionId);
            cmd.Parameters.AddWithValue("@actionId", actionId);
            cmd.ExecuteNonQuery();
        }

        public override int EnsureProfileId(string profileName, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText =
                //"INSERT OR IGNORE INTO SyncProfile (SyncProfileName) VALUES (@profileName)";
                "INSERT OR IGNORE INTO " + NwdContract.TABLE_SYNC_PROFILE +
                " (" +
                    NwdContract.COLUMN_SYNC_PROFILE_NAME +
                 ") VALUES (@profileName)";

            cmd.Parameters.AddWithValue("@profileName", profileName);
            cmd.ExecuteNonQuery();

            //select value
            cmd.Parameters.Clear(); //since we will be reusing command

            cmd.CommandText =
                //"SELECT SyncProfileId FROM SyncProfile WHERE SyncProfileName = @name";
                "SELECT " + NwdContract.COLUMN_SYNC_PROFILE_ID +
                " FROM " + NwdContract.TABLE_SYNC_PROFILE +
                " WHERE " + NwdContract.COLUMN_SYNC_PROFILE_NAME + " = @name";

            cmd.Parameters.AddWithValue("@name", profileName);

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
        /// returns -1 if not found
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override int GetExtDeviceIdForProfileId(int profileId, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear(); //since we will be reusing command

            cmd.CommandText =
                "SELECT " + NwdContract.COLUMN_DEVICE_ID_EXT +
                " FROM " + NwdContract.TABLE_SYNC_PROFILE_DEVICE +
                " WHERE " +
                    NwdContract.COLUMN_SYNC_PROFILE_ID + " = @profileId";

            cmd.Parameters.AddWithValue("@profileId", profileId);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        /// <summary>
        /// returns -1 if not found
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override int GetHostDeviceIdForProfileId(int profileId, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear(); //since we will be reusing command

            cmd.CommandText =
                "SELECT " + NwdContract.COLUMN_DEVICE_ID_HOST +
                " FROM " + NwdContract.TABLE_SYNC_PROFILE_DEVICE +
                " WHERE " +
                    NwdContract.COLUMN_SYNC_PROFILE_ID + " = @profileId";

            cmd.Parameters.AddWithValue("@profileId", profileId);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        public override void InsertOrIgnoreTag(string tag, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText =
                //"INSERT OR IGNORE INTO Tag (TagValue) VALUES (@tag)";
                "INSERT OR IGNORE INTO " + NwdContract.TABLE_TAG +
                " (" + NwdContract.COLUMN_TAG_VALUE + ") VALUES (@tag)";

            cmd.Parameters.AddWithValue("@tag", tag);
            cmd.ExecuteNonQuery();
        }

        public override Dictionary<string, int> GetIdsForTags(List<string> tags, SQLiteCommand cmd)
        {
            Dictionary<string, int> tagIds =
                new Dictionary<string, int>();

            cmd.Parameters.Clear();
            cmd.CommandText = // "SELECT TagId, TagValue FROM Tag";
                "SELECT " + NwdContract.COLUMN_TAG_ID + ", " +
                            NwdContract.COLUMN_TAG_VALUE +
                " FROM " + NwdContract.TABLE_TAG + "";

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string tag = rdr.GetString(1);
                    int tagId = rdr.GetInt32(0);

                    //we only want to populate the supplied keys
                    if (tags.Contains(tag))
                    {
                        tagIds[tag] = tagId;
                    }
                }
            }

            return tagIds;
        }

        public override void PopulateTagIds(Dictionary<string, int> tagsToIds)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(GetDbName())))
            {
                conn.Open();

                string cmdStr =
                    //"SELECT TagValue, TagId FROM Tag";
                    "SELECT " +
                        NwdContract.COLUMN_TAG_VALUE + ", " +
                        NwdContract.COLUMN_TAG_ID +
                    " FROM " + NwdContract.TABLE_TAG + "";

                using (var cmd = new SQLiteCommand(cmdStr, conn))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string tagVal = rdr.GetString(0);
                            int tagId = rdr.GetInt32(1);

                            //only store those we are looking for
                            if (tagsToIds.ContainsKey(tagVal))
                            {
                                //store as lowercase for case-insensitivity
                                //TODO: encapsulate tag in class so comparision can be case-insensitive?
                                tagsToIds[tagVal.ToLower()] = tagId;
                            }
                        }
                    }
                }
            }
        }

        public override void RefreshDeviceIds()
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(GetDbName())))
            {
                conn.Open();

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                string cmdStr =
                //"SELECT DeviceDescription, " +
                //        "DeviceFriendlyName, " +
                //        "DeviceModel, " +
                //        "DeviceType, " +
                //        "DeviceId " +
                //"FROM Device";
                "SELECT " + NwdContract.COLUMN_DEVICE_DESCRIPTION + ", " +
                    "" + NwdContract.COLUMN_DEVICE_ID + " " +
                "FROM " + NwdContract.TABLE_DEVICE + "";

                using (var cmd = new SQLiteCommand(cmdStr, conn))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string desc = rdr.GetString(0);

                            deviceIds.Add(new NwdDeviceKey()
                            {
                                Description = desc
                                //,
                                //FriendlyName = friendlyName,
                                //Model = model,
                                //DeviceType = deviceType
                            }, rdr.GetInt32(1));
                        }
                    }
                }

                conn.Close();
            }
        }

        /// <summary>
        /// executes an INSERT OR IGNORE statement for supplied device info,
        /// intended for use with device nodes not meeting the standard NwdPortableDevice
        /// format (laptops, desktops, remote servers, &c.)
        /// 
        /// Any null or whitespace values will result in no database changes
        /// </summary>
        /// <param name="device"></param>
        public override void StoreDevice(string description,
                                         string friendlyName,
                                         string model,
                                         string deviceType)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(GetDbName())))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        //TODO: change this to default each to "" if null or whitespace, change method summary when you do so (it currently mentions this limitation)
                        if (!string.IsNullOrWhiteSpace(description))
                        {
                            cmd.CommandText =
                                "INSERT OR IGNORE INTO " +
                                    NwdContract.TABLE_DEVICE + " " +
                                        "(" +
                                            NwdContract.COLUMN_DEVICE_DESCRIPTION +
                                        ") " +
                                "VALUES (@deviceDescription)";

                            cmd.Parameters.AddWithValue("@deviceDescription", description);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }
            }
        }

        public override List<string> GetTableNames(SQLiteCommand cmd)
        {
            List<string> tables = new List<string>();

            cmd.Parameters.Clear(); //since we will be reusing command
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table'";

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string tableName = rdr.GetString(0);
                    if (!tableName.Equals("android_metadata",
                                         StringComparison.CurrentCultureIgnoreCase) &&
                       !tableName.Equals("sqlite_sequence",
                                         StringComparison.CurrentCultureIgnoreCase))
                    {
                        tables.Add(tableName);
                    }
                }
            }

            return tables;
        }

        public override string GetDbName()
        {
            return "nwd-V5-test";
        }

        public string GetTagsForDevicePath(string deviceDescription, string path)
        {
            throw new NotImplementedException();
        }

        public void UpdateTagStringForCurrentDevicePath(string path)
        {
            throw new NotImplementedException();
        }
    }
}
