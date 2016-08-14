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
    public class DbAdapterSwitch : DbAdapterBase
    {
        //switch upon this variable
        private DbAdapterBase db;

        public DbAdapterSwitch(DbAdapterBase db)
        {
            this.db = db;
        }

        public DbAdapterSwitch()
            : this(new DbAdapterV4c())
        {
            //chained constructor, leave empty
        }

        public override string DeleteFile(string device, string path)
        {
            return db.DeleteFile(device, path);
        }

        public override void DeleteFileForFileId(int fileId, SQLiteCommand cmd)
        {
            db.DeleteFileForFileId(fileId, cmd);
        }

        public override void DeleteFileTagsForFileId(int fileId, SQLiteCommand cmd)
        {
            db.DeleteFileTagsForFileId(fileId, cmd);
        }

        public override void DeleteSyncMap(SyncMap sm, SQLiteCommand cmd)
        {
            db.DeleteSyncMap(sm, cmd);
        }

        public override int EnsureProfileId(string profileName, SQLiteCommand cmd)
        {
            return db.EnsureProfileId(profileName, cmd);
        }

        public override List<string> GetColumnNames(string tableName, SQLiteCommand cmd)
        {
            return db.GetColumnNames(tableName, cmd);
        }

        public override int GetExtDeviceIdForProfileId(int profileId, SQLiteCommand cmd)
        {
            return db.GetExtDeviceIdForProfileId(profileId, cmd);
        }

        public override int GetFileIdForDevicePath(string device, string path, SQLiteCommand cmd)
        {
            return db.GetFileIdForDevicePath(device, path, cmd);
        }

        public override int GetHostDeviceIdForProfileId(int profileId, SQLiteCommand cmd)
        {
            return db.GetHostDeviceIdForProfileId(profileId, cmd);
        }

        public override int GetIdForFile(int deviceId, int pathId, SQLiteCommand cmd)
        {
            return db.GetIdForFile(deviceId, pathId, cmd);
        }

        public override int GetIdForHash(string hash, SQLiteCommand cmd)
        {
            return db.GetIdForHash(hash, cmd);
        }

        public override int GetIdForPath(string path, SQLiteCommand cmd)
        {
            return db.GetIdForPath(path, cmd);
        }

        public override Dictionary<string, int> GetIdsForTags(List<string> tags, SQLiteCommand cmd)
        {
            return db.GetIdsForTags(tags, cmd);
        }

        public override IEnumerable<SynergyList> GetLists(bool active)
        {
            return db.GetLists(active);
        }

        public override List<PathTagLink> GetPathTagLinks(string filePathTopFolder)
        {
            return db.GetPathTagLinks(filePathTopFolder);
        }

        public override List<string> GetTableNames(SQLiteCommand cmd)
        {
            return db.GetTableNames(cmd);
        }

        public override string GetTagsForHash(string sha1Hash, SQLiteCommand cmd)
        {
            return db.GetTagsForHash(sha1Hash, cmd);
        }

        public override void InsertOrIgnoreAction(SyncAction action, SQLiteCommand cmd)
        {
            db.InsertOrIgnoreAction(action, cmd);
        }

        public override void InsertOrIgnoreDirection(SyncDirection direction, SQLiteCommand cmd)
        {
            db.InsertOrIgnoreDirection(direction, cmd);
        }

        public override void InsertOrIgnoreHash(string hash, SQLiteCommand cmd)
        {
            db.InsertOrIgnoreHash(hash, cmd);
        }

        public override void InsertOrIgnorePath(string path, SQLiteCommand cmd)
        {
            db.InsertOrIgnorePath(path, cmd);
        }

        public override void InsertOrIgnoreTag(string tag, SQLiteCommand cmd)
        {
            db.InsertOrIgnoreTag(tag, cmd);
        }

        public override void LinkFileIdToTagId(int fileId, int tagId, SQLiteCommand cmd)
        {
            db.LinkFileIdToTagId(fileId, tagId, cmd);
        }

        public override void PopulatePathIds(Dictionary<string, int> pathsToIds)
        {
            db.PopulatePathIds(pathIds);
        }

        public override void PopulateSyncMaps(SyncProfile sp, SQLiteCommand cmd)
        {
            db.PopulateSyncMaps(sp, cmd);
        }

        public override void PopulateSyncProfiles(List<SyncProfile> lst, SQLiteCommand cmd)
        {
            db.PopulateSyncProfiles(lst, cmd);
        }

        public override void PopulateTagIds(Dictionary<string, int> tagsToIds)
        {
            db.PopulateTagIds(tagsToIds);
        }

        public override void RefreshActionIds(SQLiteCommand cmd)
        {
            db.RefreshActionIds(cmd);
        }

        public override void RefreshDirectionIds(SQLiteCommand cmd)
        {
            db.RefreshDirectionIds(cmd);
        }

        public override void RefreshPathIds(Dictionary<string, int> pathIds, SQLiteCommand cmd)
        {
            db.RefreshPathIds(pathIds, cmd);
        }

        public override void RefreshProfileIds(SQLiteCommand cmd)
        {
            db.RefreshProfileIds(cmd);
        }

        public override void SetActive(string listName, bool active, SQLiteCommand cmd)
        {
            db.SetActive(listName, active, cmd);
        }

        public override void StoreDevice(string description, string friendlyName, string model, string deviceType)
        {
            db.StoreDevice(description, friendlyName, model, deviceType);
        }

        public override void StoreDeviceFiles(List<PathToTagMapping> mappings)
        {
            db.StoreDeviceFiles(mappings);
        }

        public override void StoreFileTags(List<PathToTagMapping> mappings)
        {
            db.StoreFileTags(mappings);
        }

        public override void StorePaths(List<string> lst)
        {
            db.StorePaths(lst);
        }

        public override void StoreTags(List<string> lst)
        {
            db.StoreTags(lst);
        }

        public override int UpsertFile(int deviceId, int pathId, int hashId, string hashedAtTimeStamp, SQLiteCommand cmd)
        {
            return db.UpsertFile(deviceId, pathId, hashId, hashedAtTimeStamp, cmd);
        }

        public override void UpsertFragment(int listId, int itemId, SynergyItem si, SQLiteCommand cmd)
        {
            db.UpsertFragment(listId, itemId, si, cmd);
        }

        public override void UpsertSyncMap(int profileId, int srcId, int destId, int directionId, int actionId, SQLiteCommand cmd)
        {
            db.UpsertSyncMap(profileId, srcId, destId, directionId, actionId, cmd);
        }

        internal override void PopulateFileIds(List<PathToTagMapping> mappings)
        {
            db.PopulateFileIds(mappings);
        }

        internal override void RefreshDeviceIds()
        {
            db.RefreshDeviceIds();
        }

        internal override int EnsureIdForValue(string tableName, string idColumnName, string valueColumnName, string valueToEnsure, SQLiteCommand cmd)
        {
            return db.EnsureIdForValue(tableName, idColumnName, valueColumnName, valueToEnsure, cmd);
        }
    }
}
