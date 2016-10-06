using NineWorldsDeep.Core;
using NineWorldsDeep.Synergy;
using NineWorldsDeep.Warehouse;
using System.Collections.Generic;
using System.Data.SQLite;

namespace NineWorldsDeep.Db.Sqlite
{
    public interface IDbAdapter
    {
        string DeleteFile(string device, string path);
        void DeleteFileForFileId(int fileId, SQLiteCommand cmd);
        void DeleteFileTagsForFileId(int fileId, SQLiteCommand cmd);
        void DeleteSyncMap(SyncMap sm, SQLiteCommand cmd);
        int EnsureProfileId(string profileName, SQLiteCommand cmd);
        List<string> GetColumnNames(string tableName, SQLiteCommand cmd);
        int GetExtDeviceIdForProfileId(int profileId, SQLiteCommand cmd);
        int GetFileIdForDevicePath(string device, string path, SQLiteCommand cmd);
        int GetHostDeviceIdForProfileId(int profileId, SQLiteCommand cmd);
        int GetIdForFile(int deviceId, int pathId, SQLiteCommand cmd);
        int GetIdForHash(string hash, SQLiteCommand cmd);
        int GetIdForPath(string path, SQLiteCommand cmd);
        Dictionary<string, int> GetIdsForTags(List<string> tags, SQLiteCommand cmd);
        IEnumerable<SynergyList> GetLists(bool active);
        List<PathTagLink> GetPathTagLinks(string filePathTopFolder);
        List<string> GetTableNames(SQLiteCommand cmd);
        string LoadSyncProfile(SyncProfile _syncProfile);
        string GetTagStringForHash(string sha1Hash, SQLiteCommand cmd);
        void InsertOrIgnoreAction(SyncAction action, SQLiteCommand cmd);
        void InsertOrIgnoreDirection(SyncDirection direction, SQLiteCommand cmd);
        void InsertOrIgnoreHash(string hash, SQLiteCommand cmd);
        void InsertOrIgnorePath(string path, SQLiteCommand cmd);
        void InsertOrIgnoreTag(string tag, SQLiteCommand cmd);
        void LinkFileIdToTagId(int fileId, int tagId, SQLiteCommand cmd);
        void PopulatePathIds(Dictionary<string, int> pathsToIds);
        void UpdateTagStringForCurrentDevicePath(string path, string tagString);
        void PopulateSyncMaps(SyncProfile sp, SQLiteCommand cmd);
        void PopulateSyncProfiles(List<SyncProfile> lst, SQLiteCommand cmd);
        void PopulateTagIds(Dictionary<string, int> tagsToIds);
        void RefreshActionIds(SQLiteCommand cmd);
        void RefreshDirectionIds(SQLiteCommand cmd);
        void RefreshPathIds(Dictionary<string, int> pathIds, SQLiteCommand cmd);
        void RefreshProfileIds(SQLiteCommand cmd);
        void SetActive(string listName, bool active, SQLiteCommand cmd);
        void StoreDevice(string description, string friendlyName, string model, string deviceType);
        void StoreDeviceFiles(List<PathToTagMapping> mappings);
        void StoreFileTags(List<PathToTagMapping> mappings);
        void StorePaths(List<string> lst);
        void StoreTags(List<string> lst);
        int UpsertFile(int deviceId, int pathId, int hashId, string hashedAtTimeStamp, SQLiteCommand cmd);
        IEnumerable<SyncProfile> GetAllSyncProfiles();
        void UpsertFragment(int listId, int itemId, SynergyItem si, SQLiteCommand cmd);
        string DeleteFile(string path);
        void UpsertSyncMap(int profileId, int srcId, int destId, int directionId, int actionId, SQLiteCommand cmd);
        void PopulateFileIds(List<PathToTagMapping> mappings);
        void RefreshDeviceIds();
        int EnsureIdForValue(string tableName, string idColumnName, string valueColumnName, string valueToEnsure, SQLiteCommand cmd);
        string GetDbName();
        Dictionary<string, int> GetPathIds();
        string SaveSyncProfile(SyncProfile _syncProfile);
        string DeleteSyncMap(SyncMap sm);
        IEnumerable<SynergyList> GetActiveLists();
        string GetTagStringForSHA1Hash(string sha1Hash);
        void StorePathToTagMappings(List<PathToTagMapping> mappings);
        string StoreImport(string profileName, string extPath, string hostPath, string hash, List<string> tags);
        string GetErdRawSource();
        int GetDeviceId(string description, string friendlyName, string model, string deviceType);
        string EnsureSyncProfile(string profileName);
        void SaveSynergyLists(IEnumerable<SynergyList> lists);
        void UpdateActiveInactive(IEnumerable<SynergyList> setToActive, IEnumerable<SynergyList> setToInactive);
        string GetTagsForDevicePath(string deviceDescription, string path);
        void StoreHashForPath(string hostHash, string hostPath);
    }
}