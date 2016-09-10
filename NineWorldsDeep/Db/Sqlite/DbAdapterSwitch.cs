using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Synergy;
using NineWorldsDeep.Warehouse;

namespace NineWorldsDeep.Db.Sqlite
{
    public class DbAdapterSwitch : IDbAdapter
    {
        //switch upon this variable
        private IDbAdapter db;

        public DbAdapterSwitch(IDbAdapter db)
        {
            this.db = db;
        }

        public DbAdapterSwitch()
            : this(new DbAdapterV4c())
        {
            //chained constructor, leave empty
        }

        public string DeleteFile(string device, string path)
        {
            return db.DeleteFile(device, path);
        }

        public void DeleteFileForFileId(int fileId, SQLiteCommand cmd)
        {
            db.DeleteFileForFileId(fileId, cmd);
        }

        public void DeleteFileTagsForFileId(int fileId, SQLiteCommand cmd)
        {
            db.DeleteFileTagsForFileId(fileId, cmd);
        }

        public void DeleteSyncMap(SyncMap sm, SQLiteCommand cmd)
        {
            db.DeleteSyncMap(sm, cmd);
        }

        public string LoadSyncProfile(SyncProfile _syncProfile)
        {
            return db.LoadSyncProfile(_syncProfile);
        }

        public int EnsureProfileId(string profileName, SQLiteCommand cmd)
        {
            return db.EnsureProfileId(profileName, cmd);
        }

        public List<string> GetColumnNames(string tableName, SQLiteCommand cmd)
        {
            return db.GetColumnNames(tableName, cmd);
        }

        public int GetExtDeviceIdForProfileId(int profileId, SQLiteCommand cmd)
        {
            return db.GetExtDeviceIdForProfileId(profileId, cmd);
        }

        public void UpdateTagStringForCurrentDevicePath(string path, string tagString)
        {
            db.UpdateTagStringForCurrentDevicePath(path, tagString);
        }

        public int GetFileIdForDevicePath(string device, string path, SQLiteCommand cmd)
        {
            return db.GetFileIdForDevicePath(device, path, cmd);
        }

        public int GetHostDeviceIdForProfileId(int profileId, SQLiteCommand cmd)
        {
            return db.GetHostDeviceIdForProfileId(profileId, cmd);
        }

        public int GetIdForFile(int deviceId, int pathId, SQLiteCommand cmd)
        {
            return db.GetIdForFile(deviceId, pathId, cmd);
        }

        public int GetIdForHash(string hash, SQLiteCommand cmd)
        {
            return db.GetIdForHash(hash, cmd);
        }

        public int GetIdForPath(string path, SQLiteCommand cmd)
        {
            return db.GetIdForPath(path, cmd);
        }

        public Dictionary<string, int> GetIdsForTags(List<string> tags, SQLiteCommand cmd)
        {
            return db.GetIdsForTags(tags, cmd);
        }

        public IEnumerable<SyncProfile> GetAllSyncProfiles()
        {
            return db.GetAllSyncProfiles();
        }

        public string DeleteFile(string path)
        {
            return db.DeleteFile(path);
        }

        public IEnumerable<SynergyList> GetLists(bool active)
        {
            return db.GetLists(active);
        }

        public List<PathTagLink> GetPathTagLinks(string filePathTopFolder)
        {
            return db.GetPathTagLinks(filePathTopFolder);
        }

        public IEnumerable<SynergyList> GetActiveLists()
        {
            return db.GetActiveLists();
        }

        public List<string> GetTableNames(SQLiteCommand cmd)
        {
            return db.GetTableNames(cmd);
        }

        public string GetTagStringForHash(string sha1Hash, SQLiteCommand cmd)
        {
            return db.GetTagStringForHash(sha1Hash, cmd);
        }

        public void InsertOrIgnoreAction(SyncAction action, SQLiteCommand cmd)
        {
            db.InsertOrIgnoreAction(action, cmd);
        }

        public void InsertOrIgnoreDirection(SyncDirection direction, SQLiteCommand cmd)
        {
            db.InsertOrIgnoreDirection(direction, cmd);
        }

        public void InsertOrIgnoreHash(string hash, SQLiteCommand cmd)
        {
            db.InsertOrIgnoreHash(hash, cmd);
        }

        public void SaveSynergyLists(IEnumerable<SynergyList> lists)
        {
            db.SaveSynergyLists(lists);
        }

        public void InsertOrIgnorePath(string path, SQLiteCommand cmd)
        {
            db.InsertOrIgnorePath(path, cmd);
        }

        public void InsertOrIgnoreTag(string tag, SQLiteCommand cmd)
        {
            db.InsertOrIgnoreTag(tag, cmd);
        }

        public void LinkFileIdToTagId(int fileId, int tagId, SQLiteCommand cmd)
        {
            db.LinkFileIdToTagId(fileId, tagId, cmd);
        }

        public void PopulatePathIds(Dictionary<string, int> pathsToIds)
        {
            db.PopulatePathIds(db.GetPathIds());
        }

        public void PopulateSyncMaps(SyncProfile sp, SQLiteCommand cmd)
        {
            db.PopulateSyncMaps(sp, cmd);
        }

        public void PopulateSyncProfiles(List<SyncProfile> lst, SQLiteCommand cmd)
        {
            db.PopulateSyncProfiles(lst, cmd);
        }

        public void PopulateTagIds(Dictionary<string, int> tagsToIds)
        {
            db.PopulateTagIds(tagsToIds);
        }

        public void RefreshActionIds(SQLiteCommand cmd)
        {
            db.RefreshActionIds(cmd);
        }

        public void RefreshDirectionIds(SQLiteCommand cmd)
        {
            db.RefreshDirectionIds(cmd);
        }

        public void RefreshPathIds(Dictionary<string, int> pathIds, SQLiteCommand cmd)
        {
            db.RefreshPathIds(pathIds, cmd);
        }

        public void RefreshProfileIds(SQLiteCommand cmd)
        {
            db.RefreshProfileIds(cmd);
        }

        public void UpdateActiveInactive(IEnumerable<SynergyList> setToActive, IEnumerable<SynergyList> setToInactive)
        {
            db.UpdateActiveInactive(setToActive, setToInactive);
        }

        public void SetActive(string listName, bool active, SQLiteCommand cmd)
        {
            db.SetActive(listName, active, cmd);
        }

        public void StoreDevice(string description, string friendlyName, string model, string deviceType)
        {
            db.StoreDevice(description, friendlyName, model, deviceType);
        }

        public void StoreDeviceFiles(List<PathToTagMapping> mappings)
        {
            db.StoreDeviceFiles(mappings);
        }

        internal string SaveSyncProfile(SyncProfile _syncProfile)
        {
            return db.SaveSyncProfile(_syncProfile);
        }

        public void StoreFileTags(List<PathToTagMapping> mappings)
        {
            db.StoreFileTags(mappings);
        }

        public void StorePaths(List<string> lst)
        {
            db.StorePaths(lst);
        }

        public void StoreTags(List<string> lst)
        {
            db.StoreTags(lst);
        }

        public int UpsertFile(int deviceId, int pathId, int hashId, string hashedAtTimeStamp, SQLiteCommand cmd)
        {
            return db.UpsertFile(deviceId, pathId, hashId, hashedAtTimeStamp, cmd);
        }

        public void UpsertFragment(int listId, int itemId, SynergyItem si, SQLiteCommand cmd)
        {
            db.UpsertFragment(listId, itemId, si, cmd);
        }

        public void UpsertSyncMap(int profileId, int srcId, int destId, int directionId, int actionId, SQLiteCommand cmd)
        {
            db.UpsertSyncMap(profileId, srcId, destId, directionId, actionId, cmd);
        }

        public void PopulateFileIds(List<PathToTagMapping> mappings)
        {
            db.PopulateFileIds(mappings);
        }

        internal string DeleteSyncMap(SyncMap sm)
        {
            return db.DeleteSyncMap(sm);
        }

        public void RefreshDeviceIds()
        {
            db.RefreshDeviceIds();
        }

        public int EnsureIdForValue(string tableName, string idColumnName, string valueColumnName, string valueToEnsure, SQLiteCommand cmd)
        {
            return db.EnsureIdForValue(tableName, idColumnName, valueColumnName, valueToEnsure, cmd);
        }

        public string GetTagsForDevicePath(string deviceDescription, string path)
        {
            return db.GetTagsForDevicePath(deviceDescription, path);
        }

        public string GetDbName()
        {
            if (db != null)
            {
                return db.GetDbName();
            }
            else
            {
                return null;
            }
        }

        internal string GetTagsForSHA1Hash(string sha1Hash)
        {
            return db.GetTagStringForSHA1Hash(sha1Hash);
        }

        internal void StorePathToTagMappings(List<PathToTagMapping> mappings)
        {
            db.StorePathToTagMappings(mappings);
        }

        internal void StoreImport(string profileName, string extPath, string hostPath, string hash, List<string> tags)
        {
            db.StoreImport(profileName, extPath, hostPath, hash, tags);
        }

        internal string GetErdRawSource()
        {
            return db.GetErdRawSource();
        }

        internal int GetDeviceId(string description, string friendlyName, string model, string deviceType)
        {
            return db.GetDeviceId(description, friendlyName, model, deviceType);
        }

        internal void EnsureSyncProfile(string profileName)
        {
            db.EnsureSyncProfile(profileName);
        }

        public Dictionary<string, int> GetPathIds()
        {
            return db.GetPathIds();
        }

        string IDbAdapter.SaveSyncProfile(SyncProfile _syncProfile)
        {
            return db.SaveSyncProfile(_syncProfile);
        }

        string IDbAdapter.DeleteSyncMap(SyncMap sm)
        {
            return db.DeleteSyncMap(sm);
        }

        string IDbAdapter.GetTagStringForSHA1Hash(string sha1Hash)
        {
            return db.GetTagStringForSHA1Hash(sha1Hash);
        }

        void IDbAdapter.StorePathToTagMappings(List<PathToTagMapping> mappings)
        {
            db.StorePathToTagMappings(mappings);
        }

        string IDbAdapter.StoreImport(string profileName, string extPath, string hostPath, string hash, List<string> tags)
        {
            return db.StoreImport(profileName, extPath, hostPath, hash, tags);
        }

        string IDbAdapter.GetErdRawSource()
        {
            return db.GetErdRawSource();
        }

        int IDbAdapter.GetDeviceId(string description, string friendlyName, string model, string deviceType)
        {
            return db.GetDeviceId(description, friendlyName, model, deviceType);
        }

        string IDbAdapter.EnsureSyncProfile(string profileName)
        {
            return db.EnsureSyncProfile(profileName);
        }

    }
}
