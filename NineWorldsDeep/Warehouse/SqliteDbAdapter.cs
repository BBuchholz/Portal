using NineWorldsDeep.Core;
using NineWorldsDeep.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NineWorldsDeep.Warehouse
{
    public class SqliteDbAdapter
    {
        //private Dictionary<SyncDirection, int> directionIds =
        //    new Dictionary<SyncDirection, int>();
        //private Dictionary<SyncAction, int> actionIds =
        //    new Dictionary<SyncAction, int>();
        //private Dictionary<string, int> nameIds =
        //    new Dictionary<string, int>();

        //private Dictionary<int, SyncDirection> idDirections =
        //    new Dictionary<int, SyncDirection>();
        //private Dictionary<int, SyncAction> idActions =
        //    new Dictionary<int, SyncAction>();
        //private Dictionary<int, string> idNames =
        //    new Dictionary<int, string>();

        private Db.SqliteDbAdapter db =
            new Db.SqliteDbAdapter();

        public SqliteDbAdapter()
        {
            db.InitializeIds();
        }

        //[Obsolete("use Db.SqliteDbAdapter")]
        //private void InitializeIds()
        //{
        //    RefreshIds();

        //    bool actionsMissing = false;
        //    bool directionsMissing = false;

        //    //check dictionaries, if any are not stored
        //    //insert or ignore all (quicker and just a couple of values)
        //    foreach (SyncAction action in Enum.GetValues(typeof(SyncAction)))
        //    {
        //        if (!actionIds.ContainsKey(action))
        //        {
        //            actionsMissing = true;
        //        }
        //    }

        //    foreach (SyncDirection direction in Enum.GetValues(typeof(SyncDirection)))
        //    {
        //        if (!directionIds.ContainsKey(direction))
        //        {
        //            directionsMissing = true;
        //        }
        //    }

        //    if (actionsMissing || directionsMissing)
        //    {
        //        InsertOrIgnoreAllDirectionsAndActions();
        //    }
        //}

        public IEnumerable<SyncProfile> GetAllSyncProfiles()
        {
            List<SyncProfile> lst = new List<SyncProfile>();

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
                            PopulateSyncProfiles(lst, cmd);

                            transaction.Commit();
                        }
                    }

                    conn.Close();
                }

            }
            catch (Exception ex)
            {
                //just throw for now
                throw ex;
            }

            return lst;
        }

        private void PopulateSyncProfiles(List<SyncProfile> lst, SQLiteCommand cmd)
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
                db.PopulateSyncMaps(sp, cmd);
            }
        }
        
        public string GetTagsForSHA1Hash(string sha1Hash)
        {
            string tags = "";

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
                            tags = db.GetTagsForHash(sha1Hash, cmd);
                            transaction.Commit();
                        }
                    }

                    conn.Close();
                }

            }
            catch (Exception)
            {
                //do nothing
            }

            return tags;
        }

        public string Load(SyncProfile sp)
        {
            string outputMsg = "implementation in progress";
            string time = "";

            try
            {
                //we need to make sure our id dictionaries are refreshed
                db.RefreshIds();

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

                            db.PopulateSyncMaps(sp, cmd);

                            transaction.Commit();

                            sw.Stop();
                            time = sw.Elapsed.ToString("mm\\:ss\\.ff");
                        }
                    }

                    conn.Close();
                }

                outputMsg = "Load Sync Profile Finished: " + time;
            }
            catch (Exception ex)
            {
                outputMsg = "error: " + ex.Message;
            }

            return outputMsg;
        }

        //[Obsolete("use Db.SqliteDbAdapter")]
        //private void PopulateSyncMaps(SyncProfile sp, SQLiteCommand cmd)
        //{
        //    sp.SyncMaps.Clear();

        //    cmd.Parameters.Clear(); //since we will be reusing command
        //    cmd.CommandText =
        //        //"SELECT sp.SyncProfileName, " +
        //        //        "pSrc.PathValue AS SourcePath,  " +
        //        //        "pDst.PathValue AS DestPath, " +
        //        //        "sm.SyncDirectionId, " +
        //        //        "sm.SyncActionIdDefault " +
        //        //"FROM SyncMap AS sm " +
        //        //"JOIN SyncProfile AS sp " +
        //        //"ON sm.SyncProfileId = sp.SyncProfileId " +
        //        //"JOIN Path AS pSrc " +
        //        //"ON pSrc.PathId = sm.PathIdSource " +
        //        //"JOIN Path AS pDst " +
        //        //"ON pDst.PathId = sm.PathIdDestination " +
        //        //"WHERE sp.SyncProfileName = @name ";
        //        "SELECT sp." + NwdContract.COLUMN_SYNC_PROFILE_NAME + ", " +
        //                "pSrc." + NwdContract.COLUMN_PATH_VALUE + " AS SourcePath,  " +
        //                "pDst." + NwdContract.COLUMN_PATH_VALUE + " AS DestPath, " +
        //                "sm." + NwdContract.COLUMN_SYNC_DIRECTION_ID + ", " +
        //                "sm." + NwdContract.COLUMN_SYNC_ACTION_ID_DEFAULT + " " +
        //        "FROM " + NwdContract.TABLE_SYNC_MAP + " AS sm " +
        //        "JOIN " + NwdContract.TABLE_SYNC_PROFILE + " AS sp " +
        //        "ON sm." + NwdContract.COLUMN_SYNC_PROFILE_ID + " = sp." + NwdContract.COLUMN_SYNC_PROFILE_ID + " " +
        //        "JOIN " + NwdContract.TABLE_PATH + " AS pSrc " +
        //        "ON pSrc." + NwdContract.COLUMN_PATH_ID + " = sm." + NwdContract.COLUMN_PATH_ID_SOURCE + " " +
        //        "JOIN " + NwdContract.TABLE_PATH + " AS pDst " +
        //        "ON pDst." + NwdContract.COLUMN_PATH_ID + " = sm." + NwdContract.COLUMN_PATH_ID_DESTINATION + " " +
        //        "WHERE sp." + NwdContract.COLUMN_SYNC_PROFILE_NAME + " = @name ";

        //    cmd.Parameters.AddWithValue("@name", sp.Name);

        //    using (var rdr = cmd.ExecuteReader())
        //    {
        //        while (rdr.Read())
        //        {
        //            string source = rdr.GetString(1);
        //            string destination = rdr.GetString(2);
        //            int directionId = rdr.GetInt32(3);
        //            int actionId = rdr.GetInt32(4);

        //            SyncDirection direction = idDirections[directionId];
        //            SyncAction action = idActions[actionId];

        //            sp.SyncMaps.Add(new SyncMap(sp,
        //                                        direction,
        //                                        action)
        //            {
        //                Source = source,
        //                Destination = destination
        //            });
        //        }
        //    }
        //}

        private int EnsurePath(string path, SQLiteCommand cmd)
        {
            db.InsertOrIgnorePath(path, cmd);
            return db.GetIdForPath(path, cmd);
        }

        private int EnsureHash(string hash, SQLiteCommand cmd)
        {
            db.InsertOrIgnoreHash(hash, cmd);
            return db.GetIdForHash(hash, cmd);
        }

        /// <summary>
        /// adds all tags to database, ignoring each if it already exists
        /// returns a dictionary mapping supplied tags to their ids
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private Dictionary<string, int> EnsureTags(List<string> tags, SQLiteCommand cmd)
        {
            InsertOrIgnoreTags(tags, cmd);
            return GetIdsForTags(tags, cmd);
        }

        /// <summary>
        /// returns -1 if not found
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private int GetExtDeviceIdForProfileId(int profileId, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear(); //since we will be reusing command

            cmd.CommandText =
                //"SELECT DeviceIdExt FROM junction_Device_SyncProfile WHERE SyncProfileId = @profileId";
                "SELECT " + NwdContract.COLUMN_DEVICE_ID_EXT + 
                " FROM " + NwdContract.TABLE_JUNCTION_DEVICE_SYNC_PROFILE + 
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
        private int GetHostDeviceIdForProfileId(int profileId, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear(); //since we will be reusing command

            cmd.CommandText =
                //"SELECT DeviceIdHost FROM junction_Device_SyncProfile WHERE SyncProfileId = @profileId";
                "SELECT " + NwdContract.COLUMN_DEVICE_ID_HOST + 
                " FROM " + NwdContract.TABLE_JUNCTION_DEVICE_SYNC_PROFILE + 
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

        private Dictionary<string, int> GetIdsForTags(List<string> tags, SQLiteCommand cmd)
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

        public string StoreImport(string profileName, string extPath, string hostPath, string hash, List<string> tags)
        {
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

                            //ensure paths
                            int extPathId = EnsurePath(extPath, cmd);
                            int hostPathId = EnsurePath(hostPath, cmd);

                            //ensure hash
                            int hashId = EnsureHash(hash, cmd);

                            //ensure tags
                            Dictionary<string, int> tagIds = EnsureTags(tags, cmd);

                            //ensure profile
                            int profileId = EnsureProfileId(profileName, cmd);

                            //get external deviceId for profile
                            int extDeviceId = GetExtDeviceIdForProfileId(profileId, cmd);

                            if (extDeviceId != -1)
                            {
                                //upsert file (to update hashedAt timestamp if already exists)
                                string timeStamp = NwdUtils.GetTimeStamp_yyyyMMddHHmmss();
                                int fileId = db.UpsertFile(extDeviceId, extPathId, hashId, timeStamp, cmd);

                                //foreach tag, link File to Tag (junction table entry)
                                LinkFileIdToTagIds(fileId, tagIds, cmd);
                            }

                            //get host deviceId for profile
                            int hostDeviceId = GetHostDeviceIdForProfileId(profileId, cmd);

                            if (hostDeviceId != -1)
                            {
                                //upsert file (to update hashedAt timestamp if already exists)
                                string timeStamp = NwdUtils.GetTimeStamp_yyyyMMddHHmmss();
                                int fileId = db.UpsertFile(hostDeviceId, hostPathId, hashId, timeStamp, cmd);

                                //foreach tag, link File to Tag (junction table entry)
                                LinkFileIdToTagIds(fileId, tagIds, cmd);
                            }

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

        private void LinkFileIdToTagIds(int fileId, Dictionary<string, int> tagIds, SQLiteCommand cmd)
        {
            foreach (var tagId in tagIds.Values)
            {
                db.LinkFileIdToTagId(fileId, tagId, cmd);
            }
        }

        //[Obsolete("use Db.SqliteDbAdapter")]
        //private void InsertOrIgnoreAllDirectionsAndActions()
        //{
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
        //                    foreach (SyncAction action in Enum.GetValues(typeof(SyncAction)))
        //                    {
        //                        InsertOrIgnoreAction(action, cmd);
        //                    }

        //                    foreach (SyncDirection direction in Enum.GetValues(typeof(SyncDirection)))
        //                    {
        //                        if (!directionIds.ContainsKey(direction))
        //                        {
        //                            InsertOrIgnoreDirection(direction, cmd);
        //                        }
        //                    }
        //                    transaction.Commit();
        //                }
        //            }

        //            conn.Close();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        //just throw for now
        //        throw ex;
        //    }
        //}

        public List<string> GetColumnNames(string tableName, SQLiteCommand cmd)
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

        public List<string> GetTableNames(SQLiteCommand cmd)
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

        public string GetErdRawSource()
        {
            string outputMsg = "";

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
                            ////////////////////////////////////////CODE HERE//////////////////////////////////////

                            List<string> tables = GetTableNames(cmd);

                            foreach (string table in tables)
                            {
                                outputMsg += table + Environment.NewLine;
                                outputMsg += "-------" + Environment.NewLine;

                                List<string> cols = GetColumnNames(table, cmd);

                                foreach (string col in cols)
                                {
                                    outputMsg += col + Environment.NewLine;
                                }

                                outputMsg += Environment.NewLine;
                            }

                            transaction.Commit();

                        }
                    }

                    conn.Close();
                }

            }
            catch (Exception ex)
            {
                outputMsg = "error: " + ex.Message;
            }

            return outputMsg;
        }

        [Obsolete("use Db.SqliteDbAdapter")]
        private void InsertOrIgnoreDirection(SyncDirection direction, SQLiteCommand cmd)
        {
            string directionVal = direction.ToString();
            cmd.Parameters.Clear();

            cmd.CommandText =
                //"INSERT OR IGNORE INTO SyncDirection (SyncDirectionValue) VALUES (@directionVal)";
                "INSERT OR IGNORE INTO " + NwdContract.TABLE_SYNC_DIRECTION + " (" + NwdContract.COLUMN_SYNC_DIRECTION_VALUE + ") VALUES (@directionVal)";

            cmd.Parameters.AddWithValue("@directionVal", directionVal);
            cmd.ExecuteNonQuery();
        }

        [Obsolete("use Db.SqliteDbAdapter")]
        private void InsertOrIgnoreAction(SyncAction action, SQLiteCommand cmd)
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

        public string Delete(SyncMap sm)
        {
            string outputMsg = "implementation in progress";
            string time = "";

            try
            {
                //we need to make sure our id dictionaries are refreshed
                db.RefreshIds();

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

                            DeleteSyncMap(sm, cmd);

                            transaction.Commit();

                            sw.Stop();
                            time = sw.Elapsed.ToString("mm\\:ss\\.ff");
                        }
                    }

                    conn.Close();
                }

                outputMsg = "Delete Sync Map Finished: " + time;
            }
            catch (Exception ex)
            {
                outputMsg = "error: " + ex.Message;
            }

            return outputMsg;
        }

        //[Obsolete("use Db.SqliteDbAdapter")]
        //public void RefreshIds()
        //{
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
        //                    RefreshProfileIds(cmd);
        //                    RefreshDirectionIds(cmd);
        //                    RefreshActionIds(cmd);

        //                    transaction.Commit();
        //                }
        //            }

        //            conn.Close();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        //just throw for now
        //        throw ex;
        //    }
        //}

        //[Obsolete("use Db.SqliteDbAdapter")]
        //private void RefreshProfileIds(SQLiteCommand cmd)
        //{
        //    cmd.Parameters.Clear(); //since we will be reusing command
        //    cmd.CommandText =
        //        //"SELECT SyncProfileId, SyncProfileName FROM SyncProfile";
        //        "SELECT " + NwdContract.COLUMN_SYNC_PROFILE_ID + ", " + 
        //                    NwdContract.COLUMN_SYNC_PROFILE_NAME + 
        //        " FROM " + NwdContract.TABLE_SYNC_PROFILE + "";

        //    using (var rdr = cmd.ExecuteReader())
        //    {
        //        while (rdr.Read())
        //        {
        //            int id = rdr.GetInt32(0);
        //            string name = rdr.GetString(1);

        //            nameIds[name] = id;
        //            idNames[id] = name;

        //        }
        //    }
        //}

        //[Obsolete("use Db.SqliteDbAdapter")]
        //public void RefreshDirectionIds(SQLiteCommand cmd)
        //{
        //    cmd.Parameters.Clear(); //since we will be reusing command

        //    cmd.CommandText =
        //        //"SELECT SyncDirectionId, SyncDirectionValue FROM SyncDirection";
        //        "SELECT " + 
        //            NwdContract.COLUMN_SYNC_DIRECTION_ID + ", " + 
        //            NwdContract.COLUMN_SYNC_DIRECTION_VALUE + 
        //        " FROM " + NwdContract.TABLE_SYNC_DIRECTION + "";

        //    using (var rdr = cmd.ExecuteReader())
        //    {
        //        while (rdr.Read())
        //        {
        //            int id = rdr.GetInt32(0);
        //            string directionValue = rdr.GetString(1);
        //            SyncDirection direction =
        //                (SyncDirection)Enum.Parse(typeof(SyncDirection),
        //                                          directionValue);
        //            if (Enum.IsDefined(typeof(SyncDirection), direction))
        //            {
        //                directionIds[direction] = id;
        //                idDirections[id] = direction;
        //            }
        //        }
        //    }
        //}

        //[Obsolete("use Db.SqliteDbAdapter")]
        //public void RefreshActionIds(SQLiteCommand cmd)
        //{
        //    cmd.Parameters.Clear(); //since we will be reusing command
        //    cmd.CommandText =
        //        //"SELECT SyncActionId, SyncActionValue FROM SyncAction";
        //        "SELECT " + 
        //            NwdContract.COLUMN_SYNC_ACTION_ID + ", " + 
        //            NwdContract.COLUMN_SYNC_ACTION_VALUE + 
        //        " FROM " + NwdContract.TABLE_SYNC_ACTION + "";

        //    using (var rdr = cmd.ExecuteReader())
        //    {
        //        while (rdr.Read())
        //        {
        //            int id = rdr.GetInt32(0);
        //            string actionValue = rdr.GetString(1);
        //            SyncAction action =
        //                (SyncAction)Enum.Parse(typeof(SyncAction),
        //                                       actionValue);
        //            if (Enum.IsDefined(typeof(SyncAction), action))
        //            {
        //                actionIds[action] = id;
        //                idActions[id] = action;
        //            }
        //        }
        //    }
        //}

        public string Save(SyncProfile sp)
        {
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

                            /////////use open transaction to insert or ignore all paths, and profile name.
                            int profileId = EnsureProfileId(sp.Name, cmd);

                            List<string> paths = sp.SyncMaps.AllPaths();

                            foreach (string path in paths)
                            {
                                InsertOrIgnorePath(path, cmd);
                            }

                            /////////use open transaction to get path ids for path values
                            Dictionary<string, int> pathIds = new Dictionary<string, int>();

                            //store all paths
                            foreach (string path in paths)
                            {
                                pathIds[path] = -1;
                            }

                            RefreshPathIds(pathIds, cmd);

                            foreach (SyncMap map in sp.SyncMaps)
                            {
                                int destId = pathIds[map.Destination];
                                int srcId = pathIds[map.Source];
                                //int directionId = directionIds[map.SyncDirection];
                                //int actionId = actionIds[map.DefaultSyncAction];
                                int directionId = db.GetDirectionId(map.SyncDirection);
                                int actionId = db.GetActionId(map.DefaultSyncAction);

                                UpsertSyncMap(profileId, srcId, destId, directionId, actionId, cmd);
                            }

                            transaction.Commit();

                            sw.Stop();
                            time = sw.Elapsed.ToString("mm\\:ss\\.ff");
                        }
                    }

                    conn.Close();
                }

                outputMsg = "Save Sync Profile Finished: " + time;
            }
            catch (Exception ex)
            {
                outputMsg = "error: " + ex.Message;
            }

            return outputMsg;
        }

        private void UpsertSyncMap(int profileId, int srcId, int destId, int directionId, int actionId, SQLiteCommand cmd)
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

        private void DeleteSyncMap(SyncMap sm, SQLiteCommand cmd)
        {
            Dictionary<string, int> pathIds =
                new Dictionary<string, int>();

            pathIds.Add(sm.Source, -1);
            pathIds.Add(sm.Destination, -1);
            db.RefreshPathIds(pathIds, cmd);
            
            cmd.Parameters.Clear();

            cmd.CommandText =
                //"DELETE FROM SyncMap " +
                //"WHERE SyncProfileId = @profileId " +
                //"AND PathIdSource = @srcId " +
                //"AND PathIdDestination = @destId " +
                //"AND SyncDirectionId = @directionId";
                "DELETE FROM " + NwdContract.TABLE_SYNC_MAP + " " +
                "WHERE " + NwdContract.COLUMN_SYNC_PROFILE_ID + " = @profileId " +
                "AND " + NwdContract.COLUMN_PATH_ID_SOURCE + " = @srcId " +
                "AND " + NwdContract.COLUMN_PATH_ID_DESTINATION + " = @destId " +
                "AND " + NwdContract.COLUMN_SYNC_DIRECTION_ID + " = @directionId";

            //cmd.Parameters.AddWithValue("@profileId", nameIds[sm.Profile.Name]);
            //cmd.Parameters.AddWithValue("@srcId", pathIds[sm.Source]);
            //cmd.Parameters.AddWithValue("@destId", pathIds[sm.Destination]);
            //cmd.Parameters.AddWithValue("@directionId", directionIds[sm.SyncDirection]);

            cmd.Parameters.AddWithValue("@profileId", db.GetNameId(sm.Profile.Name));
            cmd.Parameters.AddWithValue("@srcId", pathIds[sm.Source]);
            cmd.Parameters.AddWithValue("@destId", pathIds[sm.Destination]);
            cmd.Parameters.AddWithValue("@directionId", db.GetDirectionId(sm.SyncDirection));

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// will insert a profile name if it doesn't exist already,
        /// will ignore on duplicate
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns>completion status string</returns>
        public string EnsureProfile(string profileName)
        {
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

                            EnsureProfileId(profileName, cmd);

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

        private int EnsureProfileId(string profileName, SQLiteCommand cmd)
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
        /// refreshes pathIds with the ids associated with each path in keys collection
        /// any paths not in keys collection will be ignored
        /// </summary>
        /// <param name="pathIds"></param>
        /// <param name="cmd"></param>
        [Obsolete("use Db.SqliteDbAdapter")]
        private void RefreshPathIds(Dictionary<string, int> pathIds, SQLiteCommand cmd)
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

        /// <summary>
        /// returns -1 if not found, else found it
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [Obsolete("use Db.SqliteDbAdapter")]
        private int GetIdForPath(string path, SQLiteCommand cmd)
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

        [Obsolete("use Db.SqliteDbAdapter")]
        private void InsertOrIgnorePath(string path, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText =
                //"INSERT OR IGNORE INTO Path (PathValue) VALUES (@path)";
                "INSERT OR IGNORE INTO " + NwdContract.TABLE_PATH +
                " (" + NwdContract.COLUMN_PATH_VALUE + ") VALUES (@path)";

            cmd.Parameters.AddWithValue("@path", path);
            cmd.ExecuteNonQuery();
        }

        private void InsertOrIgnoreTag(string tag, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText =
                //"INSERT OR IGNORE INTO Tag (TagValue) VALUES (@tag)";
                "INSERT OR IGNORE INTO " + NwdContract.TABLE_TAG +
                " (" + NwdContract.COLUMN_TAG_VALUE + ") VALUES (@tag)";

            cmd.Parameters.AddWithValue("@tag", tag);
            cmd.ExecuteNonQuery();
        }

        //private void InsertOrIgnoreProfile(SyncProfile sp, SQLiteCommand cmd)
        //{
        //    string profileName = sp.Name;
        //    cmd.Parameters.Clear();
        //    cmd.CommandText =
        //        "INSERT OR IGNORE INTO SyncProfile (SyncProfileName) VALUES (@profileName)";
        //    cmd.Parameters.AddWithValue("@profileName", profileName);
        //    cmd.ExecuteNonQuery();
        //}

        private void InsertOrIgnoreTags(List<string> tags, SQLiteCommand cmd)
        {
            foreach (var tag in tags)
            {
                InsertOrIgnoreTag(tag, cmd);
            }
        }

        /////////////////////////////////////connection/transaction template        
        //public string Template()
        //{
        //    string outputMsg = "implementation in progress";
        //    string time = "";

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
        //                    Stopwatch sw = Stopwatch.StartNew();

        //                    ////////////////////////////////////////CODE HERE//////////////////////////////////////

        //                    transaction.Commit();

        //                    sw.Stop();
        //                    time = sw.Elapsed.ToString("mm\\:ss\\.ff");
        //                }
        //            }

        //            conn.Close();
        //        }

        //        outputMsg = "Finished: " + time;
        //    }
        //    catch (Exception ex)
        //    {
        //        outputMsg = "error: " + ex.Message;
        //    }

        //    return outputMsg;
        //}

    }
}