using NineWorldsDeep.Core;
using NineWorldsDeep.Mtp;
using NineWorldsDeep.Sqlite;
using NineWorldsDeep.Synergy;
using NineWorldsDeep.Warehouse;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Db
{
    public abstract class DbAdapterBase
    {
        protected Dictionary<NwdDeviceKey, int> deviceIds =
            new Dictionary<NwdDeviceKey, int>();
        protected Dictionary<string, int> hashIds =
            new Dictionary<string, int>();
        protected Dictionary<string, int> pathIds =
            new Dictionary<string, int>();

        protected Dictionary<SyncDirection, int> directionIds =
            new Dictionary<SyncDirection, int>();
        protected Dictionary<SyncAction, int> actionIds =
            new Dictionary<SyncAction, int>();
        protected Dictionary<string, int> nameIds =
            new Dictionary<string, int>();

        protected Dictionary<int, SyncDirection> idDirections =
            new Dictionary<int, SyncDirection>();
        protected Dictionary<int, SyncAction> idActions =
            new Dictionary<int, SyncAction>();
        protected Dictionary<int, string> idNames =
            new Dictionary<int, string>();
        
        public abstract void UpsertFragment(int listId, int itemId, SynergyItem si, SQLiteCommand cmd);
        public abstract IEnumerable<SynergyList> GetLists(bool active);
        internal abstract int EnsureIdForValue(string tableName,
                                             string idColumnName,
                                             string valueColumnName,
                                             string valueToEnsure,
                                             SQLiteCommand cmd);
        public abstract void StoreTags(List<string> lst);
        public abstract void SetActive(string listName, bool active, SQLiteCommand cmd);
        public abstract void StorePaths(List<string> lst);
        internal abstract void PopulateFileIds(List<PathToTagMapping> mappings);
        internal abstract void RefreshDeviceIds();
        public abstract void InsertOrIgnorePath(string path, SQLiteCommand cmd);
        public abstract void InsertOrIgnoreHash(string hash, SQLiteCommand cmd);
        public abstract int GetIdForPath(string path, SQLiteCommand cmd);
        public abstract int GetIdForHash(string hash, SQLiteCommand cmd);
        public abstract string GetTagsForHash(string sha1Hash, SQLiteCommand cmd);
        public abstract List<PathTagLink> GetPathTagLinks(string filePathTopFolder);
        public abstract string DeleteFile(String device, String path);
        public abstract int GetFileIdForDevicePath(String device,
                                          String path,
                                          SQLiteCommand cmd);
        public abstract int GetIdForFile(int deviceId, int pathId, SQLiteCommand cmd);
        public abstract int UpsertFile(int deviceId, int pathId, int hashId, string hashedAtTimeStamp, SQLiteCommand cmd);
        public abstract void LinkFileIdToTagId(int fileId, int tagId, SQLiteCommand cmd);
        public abstract void DeleteFileTagsForFileId(int fileId, SQLiteCommand cmd);
        public abstract void DeleteFileForFileId(int fileId, SQLiteCommand cmd);
        public abstract void RefreshPathIds(Dictionary<string, int> pathIds, SQLiteCommand cmd);
        public abstract void PopulateSyncMaps(SyncProfile sp, SQLiteCommand cmd);
        public abstract void RefreshProfileIds(SQLiteCommand cmd);
        public abstract void RefreshActionIds(SQLiteCommand cmd);
        public abstract void RefreshDirectionIds(SQLiteCommand cmd);
        public abstract void InsertOrIgnoreAction(SyncAction action, SQLiteCommand cmd);
        public abstract void InsertOrIgnoreDirection(SyncDirection direction, SQLiteCommand cmd);
        public abstract void InsertOrIgnoreTag(string tag, SQLiteCommand cmd);
        public abstract Dictionary<string, int> GetIdsForTags(List<string> tags, SQLiteCommand cmd);
        public abstract int EnsureProfileId(string profileName, SQLiteCommand cmd);
        public abstract int GetExtDeviceIdForProfileId(int profileId, SQLiteCommand cmd);
        public abstract int GetHostDeviceIdForProfileId(int profileId, SQLiteCommand cmd);
        public abstract void PopulatePathIds(Dictionary<string, int> pathsToIds);
        public abstract void StoreFileTags(List<PathToTagMapping> mappings);
        public abstract void StoreDeviceFiles(List<PathToTagMapping> mappings);
        public abstract void PopulateSyncProfiles(List<SyncProfile> lst, SQLiteCommand cmd);
        public abstract void PopulateTagIds(Dictionary<string, int> tagsToIds);
        public abstract void StoreDevice(string description,
                                         string friendlyName,
                                         string model,
                                         string deviceType);
        public abstract List<string> GetTableNames(SQLiteCommand cmd);
        public abstract void DeleteSyncMap(SyncMap sm, SQLiteCommand cmd);
        public abstract List<string> GetColumnNames(string tableName, SQLiteCommand cmd);
        public abstract void UpsertSyncMap(int profileId, int srcId, int destId, int directionId, int actionId, SQLiteCommand cmd);
        public abstract string GetDbName();

        public DbAdapterBase()
        {
            InitializeIds();
        }

        public string GetErdRawSource()
        {
            string outputMsg = "";

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

        public string SaveSyncProfile(SyncProfile sp)
        {
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
                                int directionId = GetDirectionId(map.SyncDirection);
                                int actionId = GetActionId(map.DefaultSyncAction);

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

        /// <summary>
        /// will insert a profile name if it doesn't exist already,
        /// will ignore on duplicate
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns>completion status string</returns>
        public string EnsureSyncProfile(string profileName)
        {
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

        public string DeleteSyncMap(SyncMap sm)
        {
            string outputMsg = "implementation in progress";
            string time = "";

            try
            {
                //we need to make sure our id dictionaries are refreshed
                RefreshIds();

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

        public string GetTagsForSHA1Hash(string sha1Hash)
        {
            string tags = "";

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
                            tags = GetTagsForHash(sha1Hash, cmd);
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

        public string LoadSyncProfile(SyncProfile sp)
        {
            string outputMsg = "implementation in progress";
            string time = "";

            try
            {
                //we need to make sure our id dictionaries are refreshed
                RefreshIds();

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

                            PopulateSyncMaps(sp, cmd);

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

        public IEnumerable<SyncProfile> GetAllSyncProfiles()
        {
            List<SyncProfile> lst = new List<SyncProfile>();

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

        /// <summary>
        /// returns device database id if found, -1 if not found
        /// </summary>
        /// <param name="description"></param>
        /// <param name="friendlyName"></param>
        /// <param name="model"></param>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public int GetDeviceId(string description,
                               string friendlyName,
                               string model,
                               string deviceType)
        {
            RefreshDeviceIds();

            NwdDeviceKey deviceKey =
                new NwdDeviceKey(description
                //,
                //                 friendlyName,
                //                 model,
                //                 deviceType
                                 );

            if (deviceIds.ContainsKey(deviceKey))
            {
                return deviceIds[deviceKey];
            }

            return -1;
        }

        public void StorePathToTagMappings(List<PathToTagMapping> mappings)
        {
            StoreDeviceFiles(mappings);
            PopulateFileIds(mappings);
            StoreFileTags(mappings);
        }

        public string StoreImport(string profileName, string extPath, string hostPath, string hash, List<string> tags)
        {
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
                                int fileId = UpsertFile(extDeviceId, extPathId, hashId, timeStamp, cmd);

                                //foreach tag, link File to Tag (junction table entry)
                                LinkFileIdToTagIds(fileId, tagIds, cmd);
                            }

                            //get host deviceId for profile
                            int hostDeviceId = GetHostDeviceIdForProfileId(profileId, cmd);

                            if (hostDeviceId != -1)
                            {
                                //upsert file (to update hashedAt timestamp if already exists)
                                string timeStamp = NwdUtils.GetTimeStamp_yyyyMMddHHmmss();
                                int fileId = UpsertFile(hostDeviceId, hostPathId, hashId, timeStamp, cmd);

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
                LinkFileIdToTagId(fileId, tagId, cmd);
            }
        }

        private void InsertOrIgnoreTags(List<string> tags, SQLiteCommand cmd)
        {
            foreach (var tag in tags)
            {
                InsertOrIgnoreTag(tag, cmd);
            }
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

        public void InitializeIds()
        {
            RefreshIds();

            bool actionsMissing = false;
            bool directionsMissing = false;

            //check dictionaries, if any are not stored
            //insert or ignore all (quicker and just a couple of values)
            foreach (SyncAction action in Enum.GetValues(typeof(SyncAction)))
            {
                if (!actionIds.ContainsKey(action))
                {
                    actionsMissing = true;
                }
            }

            foreach (SyncDirection direction in Enum.GetValues(typeof(SyncDirection)))
            {
                if (!directionIds.ContainsKey(direction))
                {
                    directionsMissing = true;
                }
            }

            if (actionsMissing || directionsMissing)
            {
                InsertOrIgnoreAllDirectionsAndActions();
            }
        }

        public void InsertOrIgnoreAllDirectionsAndActions()
        {
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
                            foreach (SyncAction action in Enum.GetValues(typeof(SyncAction)))
                            {
                                InsertOrIgnoreAction(action, cmd);
                            }

                            foreach (SyncDirection direction in Enum.GetValues(typeof(SyncDirection)))
                            {
                                if (!directionIds.ContainsKey(direction))
                                {
                                    InsertOrIgnoreDirection(direction, cmd);
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
                //just throw for now
                throw ex;
            }
        }
        
        public int GetNameId(string name)
        {
            return nameIds[name];
        }

        public int GetDirectionId(SyncDirection direction)
        {
            return directionIds[direction];
        }

        public int GetActionId(SyncAction action)
        {
            return actionIds[action];
        }

        public void RefreshIds()
        {
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
                            RefreshProfileIds(cmd);
                            RefreshDirectionIds(cmd);
                            RefreshActionIds(cmd);

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
        }

        public IEnumerable<SynergyList> GetActiveLists()
        {
            return GetLists(true);
        }

        public string DeleteFile(String path)
        {
            return DeleteFile(null, path);
        }

        protected void ConvertNullsToEmptyStrings(NwdPortableDevice device)
        {
            if (device.Description == null)
            {
                device.Description = "";
            }

            if (device.Model == null)
            {
                device.Model = "";
            }

            if (device.DeviceType == null)
            {
                device.DeviceType = "";
            }

            if (device.FriendlyName == null)
            {
                device.FriendlyName = "";
            }
        }

        //protected NwdDeviceKey ToDeviceKey(NwdPortableDevice device)
        //{
        //    if (device == null)
        //    {
        //        throw new Exception("NwdPortableDevice null in method ToDeviceKey()");
        //    }

        //    return new NwdDeviceKey()
        //    {
        //        Description = device.Description,
        //        FriendlyName = device.FriendlyName,
        //        Model = device.Model,
        //        DeviceType = device.DeviceType
        //    };
        //}

        protected int EnsurePath(string path, SQLiteCommand cmd)
        {
            InsertOrIgnorePath(path, cmd);
            return GetIdForPath(path, cmd);
        }

        protected int EnsureHash(string hash, SQLiteCommand cmd)
        {
            InsertOrIgnoreHash(hash, cmd);
            return GetIdForHash(hash, cmd);
        }

        public void UpdateActiveInactive(IEnumerable<SynergyList> setToActive, IEnumerable<SynergyList> setToInactive)
        {
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
                            foreach (SynergyList sl in setToActive)
                            {
                                SetActive(sl.Name, true, cmd);
                            }

                            foreach (SynergyList sl in setToInactive)
                            {
                                SetActive(sl.Name, false, cmd);
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

        public int EnsureIdForItemValue(string item, SQLiteCommand cmd)
        {
            return EnsureIdForValue(NwdContract.TABLE_ITEM,
                                    NwdContract.COLUMN_ITEM_ID,
                                    NwdContract.COLUMN_ITEM_VALUE,
                                    item,
                                    cmd);
        }

        public int EnsureIdForListName(string name, SQLiteCommand cmd)
        {
            return EnsureIdForValue(NwdContract.TABLE_LIST,
                                    NwdContract.COLUMN_LIST_ID,
                                    NwdContract.COLUMN_LIST_NAME,
                                    name,
                                    cmd);
        }

        public void SaveSynergyLists(IEnumerable<SynergyList> _lists)
        {
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

                            foreach (var lst in _lists)
                            {
                                int listId = EnsureIdForListName(lst.Name, cmd);

                                foreach (var si in lst.Items)
                                {
                                    int itemId = EnsureIdForItemValue(si.Item, cmd);

                                    UpsertFragment(listId, itemId, si, cmd);
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
        }

    }
}
