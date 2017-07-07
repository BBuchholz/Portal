using NineWorldsDeep.Core;
using NineWorldsDeep.Mnemosyne.V5;
using NineWorldsDeep.Model;
using NineWorldsDeep.Sqlite;
using NineWorldsDeep.Tapestry.NodeUI;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Archivist;

namespace NineWorldsDeep.Db.Sqlite
{
    public class MediaV5SubsetDb
    {
        protected string DbName { get; private set; }
        public int LocalDeviceId { get; private set; }

        public MediaV5SubsetDb()
        {
            DbName = "nwd";
            LocalDeviceId = GetLocalMediaDeviceId();
        }

        #region "methods"

        public List<MediaDeviceModelItem> GetAllMediaDevices()
        {
            List<MediaDeviceModelItem> lst =
                new List<MediaDeviceModelItem>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        lst = SelectMediaDevices(cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }

        public List<MediaTagging> GetMediaTaggingsForHash(string hash)
        {
            List<MediaTagging> lst =
                new List<MediaTagging>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        lst = GetMediaTaggingsForHash(hash, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }

        public TaggingMatrix RetrieveLocalDeviceTaggingMatrix()
        {
            TaggingMatrix tm = null;

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
                            tm = RetrieveLocalDeviceTaggingMatrix(cmd);
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

            return tm;
        }

        private TaggingMatrix RetrieveLocalDeviceTaggingMatrix(SQLiteCommand cmd)
        {
            TaggingMatrix tm = new TaggingMatrix();

            cmd.Parameters.Clear();
            cmd.CommandText = NwdContract.GET_PATH_TAGS_FOR_DEVICE_NAME_X;

            cmd.Parameters.Add(new SQLiteParameter() {
                Value = Configuration.GetLocalDeviceDescription()
            });            

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string tag = DbV5Utils.GetNullableString(rdr, 0);
                    string path = DbV5Utils.GetNullableString(rdr, 1);
                    tm.Link(tag, path);
                }
            }

            return tm;
        }

        internal void LoadMediaWithDevicePathsForTag(MediaTag mediaTag)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {

                        LoadMediaWithDevicePathsForTag(mediaTag, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        private void LoadMediaWithDevicePathsForTag(MediaTag tag, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_MEDIA_WITH_DEVICE_PATHS_FOR_TAG_ID_X;

            cmd.Parameters.Add(new SQLiteParameter() { Value = tag.MediaTagId });

            using (var rdr = cmd.ExecuteReader())
            {
                Dictionary<string, Media> hashToMedia = 
                    new Dictionary<string, Media>();

                while (rdr.Read())
                {
                    string mediaHash = DbV5Utils.GetNullableString(rdr, 0);
                    string deviceName = DbV5Utils.GetNullableString(rdr, 1);
                    string pathValue = DbV5Utils.GetNullableString(rdr, 2);

                    if (!string.IsNullOrWhiteSpace(mediaHash))
                    {
                        if (!hashToMedia.ContainsKey(mediaHash))
                        {
                            hashToMedia[mediaHash] = new Media()
                            {
                                MediaHash = mediaHash
                            };

                            tag.Add(hashToMedia[mediaHash]);
                        }

                        var media = hashToMedia[mediaHash];

                        if (!string.IsNullOrWhiteSpace(deviceName) &&
                            !string.IsNullOrWhiteSpace(pathValue))
                        {
                            media.Add(new DevicePath()
                            {
                                DeviceName = deviceName,
                                DevicePathValue = pathValue
                            });
                        }
                    }
                }
            }
        }

        internal void PopulateTagIds(List<Tag> tags)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (Tag tag in tags)
                        {
                            tag.TagId = EnsureMediaTag(tag.TagValue, cmd);
                        }

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        public void PopulateTagIdForTagValue(MediaTag tag)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        tag.MediaTagId = EnsureMediaTag(tag.MediaTagValue, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        private List<MediaTagging> GetMediaTaggingsForHash(
            string hash, SQLiteCommand cmd)
        {
            List<MediaTagging> lst =
                new List<MediaTagging>();

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_TAGS_FOR_HASH_X;

            cmd.Parameters.Add(new SQLiteParameter { Value = hash });

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    // column indexes
                    // 0: MediaTagId
                    // 1: MediaTagValue
                    // 2: MediaTaggingId
                    // 3: MediaId
                    // 4: MediaTaggingTaggedAt
                    // 5: MediaTaggingUntaggedAt
                    // 6: MediaHash

                    int mediaTagId = rdr.GetInt32(0);
                    string mediaTagValue = rdr.GetString(1);
                    int mediaTaggingId = rdr.GetInt32(2);
                    int mediaId = rdr.GetInt32(3);

                    string mediaTaggingTaggedAtString = 
                        DbV5Utils.GetNullableString(rdr, 4);

                    string mediaTaggingUntaggedAtString = 
                        DbV5Utils.GetNullableString(rdr, 5);

                    string mediaHash = DbV5Utils.GetNullableString(rdr, 6);

                    DateTime? taggedAt =
                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(mediaTaggingTaggedAtString);

                    DateTime? untaggedAt =
                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(mediaTaggingUntaggedAtString);

                    var mt = new MediaTagging()
                    {
                        MediaTagId = mediaTagId,
                        MediaTagValue = mediaTagValue,
                        MediaTaggingId = mediaTaggingId,
                        MediaId = mediaId,
                        MediaHash = mediaHash
                    };

                    mt.SetTimeStamps(taggedAt, untaggedAt);

                    lst.Add(mt);
                }
            }

            return lst;
        }

        public List<string> GetAllFilePathsForDeviceRoot(
            int mediaDeviceId,
            string rootPath)
        {
            List<string> lst =
                new List<string>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        lst = SelectPathsForDeviceRoot(mediaDeviceId, rootPath, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }
        
        private List<string> SelectPathsForDeviceRoot(int mediaDeviceId, string rootPath, SQLiteCommand cmd)
        {
            List<string> lst =
                new List<string>();

            cmd.Parameters.Clear();
            cmd.CommandText =
                SELECT_PATH_FOR_DEVICE_ID_LIKE_ROOT_PATH_X_Y;

            SQLiteParameter mediaDeviceIdParam = new SQLiteParameter();
            mediaDeviceIdParam.Value = mediaDeviceId;
            cmd.Parameters.Add(mediaDeviceIdParam);

            SQLiteParameter rootPathParam = new SQLiteParameter();
            rootPathParam.Value = rootPath;
            cmd.Parameters.Add(rootPathParam);

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string path = DbV5Utils.GetNullableString(rdr, 0);

                    lst.Add(path);
                }
            }

            return lst;
        }

        public List<MediaRootModelItem> GetMediaRootsForDeviceId(int id)
        {
            List<MediaRootModelItem> lst =
                new List<MediaRootModelItem>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        lst = SelectMediaRoots(id, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }

        internal void StoreHashForPath(int mediaDeviceId, string path, string hash)
        {
            StoreHashForPath(-1, -1, mediaDeviceId, path, hash);
        }

        internal void StoreHashForPath(int mediaDevicePathId, 
            int mediaId, int mediaDeviceId, string path, string hash)
        {
            TestIdIsSet(mediaDeviceId, "MediaDeviceId");
            
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
                            if (mediaId < 1)
                            {
                                mediaId = GetMediaIdForHash(hash, cmd);

                                if(mediaId < 1)
                                {
                                    mediaId = EnsureMediaIdForHash(hash, cmd);
                                }
                                else
                                {
                                    UpdateHashForMediaId(hash, mediaId, cmd);
                                }
                            }
                            
                            int mediaPathId = EnsureMediaPath(path, cmd);                                
                            
                            StoreMediaAtDevicePath(
                                mediaId, mediaDeviceId, mediaPathId, cmd);

                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            //handle exception here
                            transaction.Rollback();
                        }
                    }
                }

                conn.Close();
            }
        }

        private int EnsureMediaPath(string path, SQLiteCommand cmd)
        {
            InsertMediaPath(path, cmd);
            return GetMediaPathId(path, cmd);
        }

        private int GetMediaPathId(string path, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_MEDIA_PATH_ID_FOR_PATH_X;

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

        private void StoreMediaAtDevicePath(
            int mediaId, int mediaDeviceId, int mediaPathId, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = NwdContract.INSERT_MEDIA_DEVICE_PATH_MID_DID_PID;

            SQLiteParameter mediaIdParam = new SQLiteParameter();
            mediaIdParam.Value = mediaId;
            cmd.Parameters.Add(mediaIdParam);

            SQLiteParameter mediaDeviceIdParam = new SQLiteParameter();
            mediaDeviceIdParam.Value = mediaDeviceId;
            cmd.Parameters.Add(mediaDeviceIdParam);

            SQLiteParameter mediaPathIdParam = new SQLiteParameter();
            mediaPathIdParam.Value = mediaPathId;
            cmd.Parameters.Add(mediaPathIdParam);

            cmd.ExecuteNonQuery();
        }

        private void UpdateHashForMediaId(string hash, int mediaId, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = NwdContract.UPDATE_HASH_FOR_MEDIA_ID_X_Y;

            SQLiteParameter hashParam = new SQLiteParameter();
            hashParam.Value = hash;
            cmd.Parameters.Add(hashParam);

            SQLiteParameter mediaIdParam = new SQLiteParameter();
            mediaIdParam.Value = mediaId;
            cmd.Parameters.Add(mediaIdParam);
            
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// only updates if either file name or description is set, in which case
        /// it will update both (empty value in either will overwrite whatever is 
        /// in the db, but only if one or the other is not an empty value)
        /// </summary>
        /// <param name="media"></param>
        /// <param name="cmd"></param>
        private void UpdateOrIgnoreMediaRecordByHash(Media media, SQLiteCommand cmd)
        {
            if (!string.IsNullOrWhiteSpace(media.MediaFileName) ||
                !string.IsNullOrWhiteSpace(media.MediaDescription))
            {
                cmd.Parameters.Clear();
                cmd.CommandText = NwdContract.UPDATE_MEDIA_FILE_DESC_FOR_HASH_X_Y_Z;

                cmd.Parameters.Add(new SQLiteParameter { Value = media.MediaFileName });
                cmd.Parameters.Add(new SQLiteParameter { Value = media.MediaDescription });
                cmd.Parameters.Add(new SQLiteParameter { Value = media.MediaHash });

                cmd.ExecuteNonQuery();
            }
        }

        private int EnsureMediaIdForHash(string hash, SQLiteCommand cmd)
        {
            //need to get media first so COLLATE NOCASE can be used in 
            //the select query (otherwise the select is case sensitive and
            //duplicate hashes occur)
            int mediaId = GetMediaIdForHash(hash, cmd);

            if (mediaId < 1)
            {
                InsertOrIgnoreHashForMedia(hash, cmd);
                mediaId = GetMediaIdForHash(hash, cmd);
            }

            return mediaId;
        }

        /// <summary>
        /// only ensures media table fields (ignores taggings and device paths),
        /// will return a media object with all fields populated from db after
        /// updates from original media object
        /// </summary>
        /// <param name="media"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        //private Media EnsureMediaRecord(Media media, SQLiteCommand cmd)
        //{            
        //    EnsureMediaIdForHash(media.MediaHash, cmd);
        //    UpdateOrIgnoreMediaRecordByHash(media, cmd);
        //    return GetMediaForHash(media.MediaHash, cmd);
        //}

        public int EnsureMediaHash(string hash)
        {
            int id = -1;

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        id = EnsureMediaIdForHash(hash, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return id;
        }

        private void PopulateMediaByHash(Media media, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_MEDIA_FOR_HASH_X;

            //SQLiteParameter hashParam = new SQLiteParameter();
            //hashParam.Value = media.MediaHash;
            //cmd.Parameters.Add(hashParam);

            cmd.Parameters.Add(new SQLiteParameter() { Value = media.MediaHash });

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    //0:MediaId
                    //1:MediaFileName
                    //2:MediaDescription
                    //3:MediaHash

                    int id = rdr.GetInt32(0);
                    string fileName = DbV5Utils.GetNullableString(rdr, 1);
                    string description = DbV5Utils.GetNullableString(rdr, 2);

                    media.MediaId = id;
                    media.MediaFileName = fileName;
                    media.MediaDescription = description;
                }
            }
        }

        /// <summary>
        /// requires that the hash already exists in the database,
        /// will return empty media if it does not
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        //private Media GetMediaForHash(string hash, SQLiteCommand cmd)
        //{
        //    //cmd.Parameters.Clear();
        //    //cmd.CommandText =
        //    //    NwdContract.SELECT_MEDIA_FOR_HASH_X;

        //    //SQLiteParameter hashParam = new SQLiteParameter();
        //    //hashParam.Value = hash;
        //    //cmd.Parameters.Add(hashParam);

        //    //using (var rdr = cmd.ExecuteReader())
        //    //{
        //    //    if (rdr.Read())
        //    //    {
        //    //        //0:MediaId
        //    //        //1:MediaFileName
        //    //        //2:MediaDescription
        //    //        //3:MediaHash

        //    //        int id = rdr.GetInt32(0);
        //    //        string fileName = DbV5Utils.GetNullableString(rdr, 1);
        //    //        string description = DbV5Utils.GetNullableString(rdr, 2);

        //    //        media = new Media()
        //    //        {
        //    //            MediaId = id,
        //    //            MediaFileName = fileName,
        //    //            MediaDescription = description,
        //    //            MediaHash = hash
        //    //        };
        //    //    }
        //    //}

        //    Media m = new Media() { MediaHash = hash };
        //    PopulateMediaByHash(m, cmd);
        //    return m;
        //}

        /// <summary>
        /// uses COLLATE NOCASE to perform a case insensitive query
        /// on MediaHash column
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private int GetMediaIdForHash(string hash, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_MEDIA_ID_FOR_HASH_X;

            SQLiteParameter hashParam = new SQLiteParameter();
            hashParam.Value = hash;
            cmd.Parameters.Add(hashParam);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            //Media media = GetMediaForHash(hash, cmd);

            //if(media != null)
            //{
            //    id = media.MediaId;
            //}

            return id;
        }

        private void InsertOrIgnoreHashForMedia(string hash, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText =
                NwdContract.INSERT_MEDIA_HASH_X;

            SQLiteParameter hashParam = new SQLiteParameter();
            hashParam.Value = hash;
            cmd.Parameters.Add(hashParam);

            cmd.ExecuteNonQuery();
        }

        private void TestIdIsSet(int id, string idName)
        {
            if(id < 1)
            {
                throw new Exception("invalid " + idName + ": " + id);
            }
        }

        public void InsertMediaRoot(int deviceId, string rootPath)
        {
            using (var conn = new SQLiteConnection(
                 @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        InsertMediaRoot(deviceId, rootPath, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }
        
        public void InsertPathsForDeviceId(
            int mediaDeviceId, List<string> paths, IAsyncStatusResponsive ui)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        InsertPathsForDeviceId(mediaDeviceId, paths, ui, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        private List<MediaDeviceModelItem> SelectMediaDevices(SQLiteCommand cmd)
        {
            List<MediaDeviceModelItem> lst =
                new List<MediaDeviceModelItem>();

            cmd.Parameters.Clear();
            cmd.CommandText =
                SELECT_ID_DESCRIPTION_FROM_MEDIA_DEVICE;

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    int id = rdr.GetInt32(0);
                    string description = rdr.GetString(1);

                    lst.Add(new MediaDeviceModelItem()
                    {
                        MediaDeviceId = id,
                        MediaDeviceDescription = description
                    });
                }
            }

            return lst;
        }

        private List<MediaRootModelItem> SelectMediaRoots(
            int deviceId, 
            SQLiteCommand cmd)
        {
            List<MediaRootModelItem> lst =
                new List<MediaRootModelItem>();

            cmd.Parameters.Clear();
            cmd.CommandText =
                SELECT_ID_AND_PATH_FROM_MEDIA_ROOT_FOR_DEVICE_ID;

            SQLiteParameter deviceIdParam = new SQLiteParameter();
            deviceIdParam.Value = deviceId;
            cmd.Parameters.Add(deviceIdParam);

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    int rootId = rdr.GetInt32(0);
                    string rootPath = rdr.GetString(1);

                    lst.Add(new MediaRootModelItem()
                    {
                        MediaRootId = rootId,
                        MediaDeviceId = deviceId,
                        MediaRootPath = rootPath
                    });
                }
            }

            return lst;
        }

        private int EnsureMediaDevice(string deviceDescription, SQLiteCommand cmd)
        {
            int id = -1;

            id = SelectMediaDeviceId(deviceDescription, cmd);

            if (id < 0)
            {
                InsertMediaDevice(deviceDescription, cmd);
                id = SelectMediaDeviceId(deviceDescription, cmd);
            }

            return id;
        }

        private int GetLocalMediaDeviceId()
        {
            int id = -1;
            string deviceDescription = Configuration.GetLocalDeviceDescription();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        //id = SelectMediaDeviceId(deviceDesription, cmd);

                        //if(id < 0)
                        //{
                        //    InsertMediaDevice(deviceDesription, cmd);
                        //    id = SelectMediaDeviceId(deviceDesription, cmd);
                        //}

                        id = EnsureMediaDevice(deviceDescription, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            if(id < 0)
            {
                throw new Exception("Error retrieving id for Media Device: " +
                    deviceDescription);
            }

            return id;
        }

        private void InsertMediaDevice(string deviceDescription, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = INSERT_INTO_MEDIA_DEVICE;                

            SQLiteParameter deviceDescriptionParam = new SQLiteParameter();
            deviceDescriptionParam.Value = deviceDescription;
            cmd.Parameters.Add(deviceDescriptionParam);
            
            cmd.ExecuteNonQuery();
        }

        private void InsertMediaRoot(int deviceId, string rootPath, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = INSERT_DEVICE_ID_PATH_INTO_MEDIA_ROOT;

            SQLiteParameter deviceIdParam = new SQLiteParameter();
            deviceIdParam.Value = deviceId;
            cmd.Parameters.Add(deviceIdParam);

            SQLiteParameter rootPathParam = new SQLiteParameter();
            rootPathParam.Value = rootPath;
            cmd.Parameters.Add(rootPathParam);

            cmd.ExecuteNonQuery();
        }

        internal void EnsureMediaTags(HashSet<string> tagValues)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach(string tag in tagValues)
                        {
                            EnsureMediaTag(tag, cmd);
                        }

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        internal void Sync(Media media, SQLiteCommand cmd)
        {
            media.MediaId = EnsureMediaIdForHash(media.MediaHash, cmd);
            PopulateMediaByHash(media, cmd);

            foreach (MediaTagging tagging in media.MediaTaggings)
            {
                tagging.MediaHash = media.MediaHash;
                tagging.MediaId = media.MediaId;
                
                PopulateTagByValue(tagging, cmd);
                UpsertTaggingTimeStamps(tagging, cmd);
            }

            RefreshMediaTaggingsByMediaHash(media, cmd);

            foreach (string deviceName in media.DevicePaths.Keys)
            {
                int deviceId = EnsureMediaDevice(deviceName, cmd);

                foreach (DevicePath dp in media.DevicePaths[deviceName])
                {
                    int pathId = EnsureMediaPath(dp.DevicePathValue, cmd);

                    dp.MediaDeviceId = deviceId;
                    dp.MediaPathId = pathId;
                    dp.MediaId = media.MediaId;

                    UpsertMediaDevicePathTimeStamps(dp, cmd);
                }
            }

            RefreshDevicePathsByMediaHash(media, cmd);
        }

        internal void SyncAsync(IEnumerable<Media> multipleMediaItems, 
                                IAsyncStatusResponsive ui,
                                string asyncStatusDetailPrefix = "")
        {
            string detail;
            int total = multipleMediaItems.Count();
            int count = 0;

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (Media media in multipleMediaItems)
                        {
                            count++;

                            detail = asyncStatusDetailPrefix +
                                "syncing media " + count + " of " + 
                                total + ": " + media.MediaHash;

                            ui.StatusDetailUpdate(detail);

                            Sync(media, cmd);
                        }

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        internal void Sync(IEnumerable<Media> multipleMediaItems)
        {

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach(Media media in multipleMediaItems)
                        {
                            Sync(media, cmd);
                        }

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        internal void Sync(Media media)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {                        
                        Sync(media, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        /// <summary>
        /// will refresh all DevicePaths for the MediaHash of the 
        /// Media object parameter. any existing DevicePaths will
        /// be cleared and freshly populated from the db. 
        /// Any DevicePaths that are not in the db already, will be lost
        /// </summary>
        /// <param name="media"></param>
        /// <param name="cmd"></param>
        private void RefreshDevicePathsByMediaHash(Media media, SQLiteCommand cmd)
        {
            media.DevicePaths.ClearAll();

            var devPaths = GetDevicePaths(media.MediaHash, cmd);

            foreach(DevicePath dp in devPaths.AllValues())
            {
                media.Add(dp);
            }
        }

        /// <summary>
        /// will update or insert the device path timestamps for VerifiedPresent 
        /// and VerifiedMissing using the MediaId, MediaDeviceId, and MediaPathId
        /// in the DevicePath parameter
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="cmd"></param>
        private void UpsertMediaDevicePathTimeStamps(DevicePath dp, SQLiteCommand cmd)
        {
            UpdateMediaDevicePath(dp, cmd);
            InsertMediaDevicePath(dp, cmd);
        }

        /// <summary>
        /// will refresh all MediaTaggings for the MediaId of the 
        /// Media object parameter. any existing MediaTaggings will
        /// be cleared and freshly populated from the db. 
        /// Any MediaTaggings that are not in the db already, will be lost
        /// </summary>
        /// <param name="media"></param>
        /// <param name="cmd"></param>
        private void RefreshMediaTaggingsByMediaHash(Media media, SQLiteCommand cmd)
        {
            media.MediaTaggings.Clear();

            var taggings = GetMediaTaggingsForHash(media.MediaHash, cmd);

            foreach(MediaTagging mt in taggings)
            {
                media.Add(mt);
            }
        }

        /// <summary>
        /// will update the TaggedAt and UntaggedAt timestamps for a 
        /// MediaTagging, using the TagId and MediaId set in the tagging 
        /// object parameter
        /// </summary>
        /// <param name="tagging"></param>
        /// <param name="cmd"></param>
        private void UpsertTaggingTimeStamps(MediaTagging tagging, SQLiteCommand cmd)
        {
            UpdateMediaTagging(tagging, cmd);
            InsertMediaTagging(tagging, cmd);
        }

        /// <summary>
        /// will populate the tagId value of the tagging object based on 
        /// the tag value set in the tagging object
        /// </summary>
        /// <param name="tagging"></param>
        /// <param name="cmd"></param>
        private void PopulateTagByValue(MediaTagging tagging, SQLiteCommand cmd)
        {
            tagging.MediaTagId = EnsureMediaTag(tagging.MediaTagValue, cmd);
        }

        internal MultiMap<string, DevicePath> GetDevicePaths(string hash)
        {
            MultiMap<string, DevicePath> map = new MultiMap<string, DevicePath>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        map = GetDevicePaths(hash, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return map;
        }

        private MultiMap<string, DevicePath> GetDevicePaths(string hash, SQLiteCommand cmd)
        {
            //mimic GetTaggedMediaTaggingsForHash
            MultiMap<string, DevicePath> map = new MultiMap<string, DevicePath>();

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_DEVICE_PATHS_FOR_HASH_X;

            cmd.Parameters.Add(new SQLiteParameter { Value = hash });

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    // column indexes
                    //0: MediaPathValue
                    //1: MediaDeviceDescription
                    //2: MediaDevicePathId
                    //3: MediaId
                    //4: MediaDeviceId
                    //5: MediaPathId
                    //6: MediaDevicePathVerifiedPresent
                    //7: MediaDevicePathVerifiedMissing

                    string mediaPathValue = rdr.GetString(0);
                    string mediaDeviceDescription = rdr.GetString(1);
                    int mediaDevicePathId = rdr.GetInt32(2);
                    int mediaId = rdr.GetInt32(3);
                    int mediaDeviceId = rdr.GetInt32(4);
                    int mediaPathId = rdr.GetInt32(5);

                    string mediaDevicePathVerifiedPresent = 
                        DbV5Utils.GetNullableString(rdr, 6);

                    string mediaDevicePathVerifiedMissing = 
                        DbV5Utils.GetNullableString(rdr, 7);

                    DateTime? verifiedPresent =
                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(
                            mediaDevicePathVerifiedPresent);
                    
                    DateTime? verifiedMissing =
                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(
                            mediaDevicePathVerifiedMissing);

                    var devicePath = new DevicePath()
                    {
                        DevicePathValue = mediaPathValue,
                        DeviceName = mediaDeviceDescription,
                        DevicePathId = mediaDevicePathId,
                        MediaId = mediaId,
                        MediaDeviceId = mediaDeviceId,
                        MediaPathId = mediaPathId
                    };

                    devicePath.SetTimeStamps(verifiedPresent, verifiedMissing);

                    map.Add(mediaDeviceDescription, devicePath);
                }
            }

            return map;
        }

        public int EnsureMediaTag(string tag, SQLiteCommand cmd)
        {
            int mediaTagId = GetMediaTagId(tag, cmd);

            if(mediaTagId < 1)
            {
                InsertOrIgnoreMediaTag(tag, cmd);
                mediaTagId = GetMediaTagId(tag, cmd);
            }

            return mediaTagId;
        }
        
        private int GetMediaTagId(string tag, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_MEDIA_TAG_ID_FOR_VALUE_X;

            SQLiteParameter tagParam = new SQLiteParameter();
            tagParam.Value = tag;
            cmd.Parameters.Add(tagParam);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        private void InsertOrIgnoreMediaTag(string tag, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText =
                NwdContract.INSERT_MEDIA_TAG_X;

            SQLiteParameter tagParam = new SQLiteParameter();
            tagParam.Value = tag;
            cmd.Parameters.Add(tagParam);

            cmd.ExecuteNonQuery();
        }

        internal void EnsureMediaTaggings(List<MediaTagging> taggings)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        EnsureMediaTaggings(taggings, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        private void EnsureMediaTaggings(List<MediaTagging> taggings, SQLiteCommand cmd)
        {
            foreach(MediaTagging mt in taggings)
            {
                if(mt.MediaId < 1 || mt.MediaTagId < 1)
                {
                    throw new Exception("Unable to ensure MediaTagging: " +
                        "MediaId and/or MediaTagId not set.");
                }

                InsertMediaTagging(mt, cmd); //just inserts ids, not timestamps
                UpdateMediaTagging(mt, cmd); //updates timestamps
            }
        }

        private void UpdateMediaTagging(MediaTagging mt, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.UPDATE_MEDIA_TAGGING_TAGGED_UNTAGGED_WHERE_MEDIA_ID_AND_TAG_ID_W_X_Y_Z;
            
            SQLiteParameter taggedParam = new SQLiteParameter();

            String taggedAt = TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(mt.TaggedAt);

            if (string.IsNullOrWhiteSpace(taggedAt))
            {
                taggedParam.Value = DBNull.Value;
            }
            else
            {
                taggedParam.Value = taggedAt;
            }                
            cmd.Parameters.Add(taggedParam);


            SQLiteParameter untaggedParam = new SQLiteParameter();

            String untaggedAt = 
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(mt.UntaggedAt);

            if (string.IsNullOrWhiteSpace(untaggedAt))
            {
                untaggedParam.Value = DBNull.Value;
            }
            else
            {
                untaggedParam.Value = untaggedAt;
            }
            cmd.Parameters.Add(untaggedParam);


            SQLiteParameter mediaIdParam = new SQLiteParameter();
            mediaIdParam.Value = mt.MediaId;
            cmd.Parameters.Add(mediaIdParam);

            SQLiteParameter mediaTagIdParam = new SQLiteParameter();
            mediaTagIdParam.Value = mt.MediaTagId;
            cmd.Parameters.Add(mediaTagIdParam);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// updates media device path timestamps using MediaId, 
        /// MediaPathId, and MediaDeviceId
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="cmd"></param>
        private void UpdateMediaDevicePath(DevicePath dp, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.UPDATE_MEDIA_DEVICE_PATH_V_W_X_Y_Z;

            string verifiedPresent =
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(dp.VerifiedPresent);
            string verifiedMissing =
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(dp.VerifiedMissing);

            cmd.Parameters.Add(new SQLiteParameter() { Value = verifiedPresent});
            cmd.Parameters.Add(new SQLiteParameter() { Value = verifiedMissing});
            cmd.Parameters.Add(new SQLiteParameter() { Value = dp.MediaId});
            cmd.Parameters.Add(new SQLiteParameter() { Value = dp.MediaDeviceId});
            cmd.Parameters.Add(new SQLiteParameter() { Value = dp.MediaPathId});

            cmd.ExecuteNonQuery();
        }

        private void InsertMediaTagging(MediaTagging mt, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.INSERT_OR_IGNORE_MEDIA_TAGGING_X_Y;

            SQLiteParameter mediaIdParam = new SQLiteParameter();
            mediaIdParam.Value = mt.MediaId;
            cmd.Parameters.Add(mediaIdParam);

            SQLiteParameter mediaTagIdParam = new SQLiteParameter();
            mediaTagIdParam.Value = mt.MediaTagId;
            cmd.Parameters.Add(mediaTagIdParam);
            
            cmd.ExecuteNonQuery();

            //added 2017-06-01 (just noticed, working on something else)
            //remove is something breaks :P
            //use update to set timestamps
            UpdateMediaTagging(mt, cmd);
        }

        internal Dictionary<string, Tag> GetAllMediaTags()
        {
            Dictionary<string, Tag> allTags =
                new Dictionary<string, Tag>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        allTags = SelectMediaTags(cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return allTags;
        }

        /// <summary>
        /// gets a dictionary of all media keyed to hash
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, Media> GetAllMedia()
        {
            Dictionary<string, Media> allMedia =
                new Dictionary<string, Media>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        allMedia = SelectMediaByHash(cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return allMedia;
        }

        /// <summary>
        /// gets a dictionary of all media keyed to hash. media with null or empty hash values in the db will be excluded
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Media> SelectMediaByHash(SQLiteCommand cmd)
        {
            Dictionary<string, Media> allMedia =
                new Dictionary<string, Media>();

            cmd.Parameters.Clear();
            cmd.CommandText = 
                NwdContract.SELECT_MEDIA_WHERE_HASH_NOT_NULL_OR_WHITESPACE;

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    // query column indexes
                    // 0 : MediaId
                    // 1 : MediaFileName
                    // 2 : MediaDescription
                    // 3 : MediaHash

                    int mediaId = rdr.GetInt32(0);
                    string mediaFileName = DbV5Utils.GetNullableString(rdr, 1);
                    string mediaDescription = DbV5Utils.GetNullableString(rdr, 2);
                    string mediaHash = DbV5Utils.GetNullableString(rdr, 3);
                    
                    if (!string.IsNullOrWhiteSpace(mediaHash))
                    {
                        allMedia.Add(mediaHash, new Media
                        {
                            MediaId = mediaId,
                            MediaFileName = mediaFileName,
                            MediaDescription = mediaDescription,
                            MediaHash = mediaHash
                        });
                    }
                }
            }

            return allMedia;
        }

        private Dictionary<string, Tag> SelectMediaTags(SQLiteCommand cmd)
        {
            Dictionary<string, Tag> allTags =
                new Dictionary<string, Tag>();

            cmd.Parameters.Clear();
            cmd.CommandText = NwdContract.SELECT_MEDIA_TAG_ID_VALUE;

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    int tagId = rdr.GetInt32(0);
                    string tagValue = rdr.GetString(1);

                    allTags.Add(tagValue, new Tag
                    {
                        TagId = tagId,
                        TagValue = tagValue
                    });
                }
            }

            return allTags;
        }

        private void InsertPathsForDeviceId(
            int mediaDeviceId, 
            List<string> paths, 
            IAsyncStatusResponsive ui, 
            SQLiteCommand cmd)
        {            
            foreach(string path in paths)
            {
                ui.StatusDetailUpdate("inserting path: " + path);

                string fileName = System.IO.Path.GetFileName(path);

                InsertMediaPath(path, cmd);
                InsertMediaFileName(fileName, cmd);

                InsertMediaDevicePath(fileName, mediaDeviceId, path, cmd);
            }
        }

        private void InsertMediaDevicePath(
            string fileName, int mediaDeviceId, string path, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = 
                INSERT_MEDIA_DEVICE_PATH_FILENAME_DEVICEID_PATH_X_Y_Z;

            SQLiteParameter fileNameParam = new SQLiteParameter();
            fileNameParam.Value = fileName;
            cmd.Parameters.Add(fileNameParam);

            SQLiteParameter mediaDeviceIdParam = new SQLiteParameter();
            mediaDeviceIdParam.Value = mediaDeviceId;
            cmd.Parameters.Add(mediaDeviceIdParam);

            SQLiteParameter pathParam = new SQLiteParameter();
            pathParam.Value = path;
            cmd.Parameters.Add(pathParam);

            cmd.ExecuteNonQuery();
        }

        private void InsertMediaDevicePath(DevicePath dp, SQLiteCommand cmd)
        {
            //fields: MediaId, MediaDeviceId, MediaPathId, verified present and missing
            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.INSERT_MEDIA_DEVICE_PATH_V_W_X_Y_Z;

            string verifiedPresent =
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(dp.VerifiedPresent);
            string verifiedMissing =
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(dp.VerifiedMissing);

            cmd.Parameters.Add(new SQLiteParameter() { Value = dp.MediaId });
            cmd.Parameters.Add(new SQLiteParameter() { Value = dp.MediaDeviceId });
            cmd.Parameters.Add(new SQLiteParameter() { Value = dp.MediaPathId });
            cmd.Parameters.Add(new SQLiteParameter() { Value = verifiedPresent });
            cmd.Parameters.Add(new SQLiteParameter() { Value = verifiedMissing });

            cmd.ExecuteNonQuery();
        }

        private void InsertMediaFileName(string fileName, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = INSERT_MEDIA_FILE_NAME_X;

            SQLiteParameter fileNameParam = new SQLiteParameter();
            fileNameParam.Value = fileName;
            cmd.Parameters.Add(fileNameParam);

            cmd.ExecuteNonQuery();
        }

        private void InsertMediaPath(string path, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = NwdContract.INSERT_MEDIA_PATH_X;
            
            SQLiteParameter pathParam = new SQLiteParameter();
            pathParam.Value = path;
            cmd.Parameters.Add(pathParam);

            cmd.ExecuteNonQuery();
        }

        private int SelectMediaDeviceId(string deviceDescription, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText = SELECT_MEDIA_DEVICE_ID;

            SQLiteParameter devDescriptionParam = new SQLiteParameter();
            devDescriptionParam.Value = deviceDescription;
            cmd.Parameters.Add(devDescriptionParam);
            
            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        #endregion

        #region "queries"

        private static string INSERT_INTO_MEDIA_DEVICE = 
            "INSERT OR IGNORE INTO " + NwdContract.TABLE_MEDIA_DEVICE + " "
            + "	(" + NwdContract.COLUMN_MEDIA_DEVICE_DESCRIPTION + ") "
            + "VALUES "
            + "	(?); ";

        private static string SELECT_MEDIA_DEVICE_ID =
            "SELECT " + NwdContract.COLUMN_MEDIA_DEVICE_ID + " "
            + "FROM " + NwdContract.TABLE_MEDIA_DEVICE + " "
            + "WHERE " + NwdContract.COLUMN_MEDIA_DEVICE_DESCRIPTION + " = ? ; ";

        private static string INSERT_DEVICE_ID_PATH_INTO_MEDIA_ROOT =
            "INSERT OR IGNORE INTO " + NwdContract.TABLE_MEDIA_ROOT + " "
            + "	(" + NwdContract.COLUMN_MEDIA_DEVICE_ID + ", " + NwdContract.COLUMN_MEDIA_ROOT_PATH + ") "
            + "VALUES "
            + "	(?, ?); ";

        private static string SELECT_ID_DESCRIPTION_FROM_MEDIA_DEVICE =
            "SELECT " + NwdContract.COLUMN_MEDIA_DEVICE_ID + ", "
                      + NwdContract.COLUMN_MEDIA_DEVICE_DESCRIPTION + " "
            + "FROM " + NwdContract.TABLE_MEDIA_DEVICE + "; ";

        private static string
            SELECT_ID_AND_PATH_FROM_MEDIA_ROOT_FOR_DEVICE_ID =
                "SELECT " + NwdContract.COLUMN_MEDIA_ROOT_ID + ", "
                          + NwdContract.COLUMN_MEDIA_ROOT_PATH + " "
                + "FROM " + NwdContract.TABLE_MEDIA_ROOT + " "
                + "WHERE " + NwdContract.COLUMN_MEDIA_DEVICE_ID + " = ? ; ";

        private static string SELECT_PATH_FOR_DEVICE_ID_LIKE_ROOT_PATH_X_Y =

            "SELECT mp." + NwdContract.COLUMN_MEDIA_PATH_VALUE + "  " +
            "FROM " + NwdContract.TABLE_MEDIA_PATH + " mp  " +
            "JOIN " + NwdContract.TABLE_MEDIA_DEVICE_PATH + " mdp  " +
            "ON mp." + NwdContract.COLUMN_MEDIA_PATH_ID + " = mdp." + NwdContract.COLUMN_MEDIA_PATH_ID + " " +
            "WHERE mdp." + NwdContract.COLUMN_MEDIA_DEVICE_ID + " = ? " +
            "AND mp." + NwdContract.COLUMN_MEDIA_PATH_VALUE + " LIKE ? || '%'; ";
        
        private static string INSERT_MEDIA_FILE_NAME_X =

            "INSERT OR IGNORE INTO " + NwdContract.TABLE_MEDIA + " " +
            "	(" + NwdContract.COLUMN_MEDIA_FILE_NAME + ") " +
            "VALUES " +
            "	(?); ";

        private static string INSERT_MEDIA_DEVICE_PATH_FILENAME_DEVICEID_PATH_X_Y_Z =

            "INSERT OR IGNORE INTO " + NwdContract.TABLE_MEDIA_DEVICE_PATH + " " +
            "	(" + NwdContract.COLUMN_MEDIA_ID + ", " + NwdContract.COLUMN_MEDIA_DEVICE_ID + ", " + NwdContract.COLUMN_MEDIA_PATH_ID + ") " +
            "VALUES " +
            "	( " +
            "		(SELECT m." + NwdContract.COLUMN_MEDIA_ID + " FROM " + NwdContract.TABLE_MEDIA + " m LEFT JOIN " + NwdContract.TABLE_MEDIA_DEVICE_PATH + " mdp ON m." + NwdContract.COLUMN_MEDIA_ID + " = mdp." + NwdContract.COLUMN_MEDIA_ID + " WHERE " + NwdContract.COLUMN_MEDIA_FILE_NAME + " = ? AND m." + NwdContract.COLUMN_MEDIA_HASH + " IS NULL AND mdp." + NwdContract.COLUMN_MEDIA_DEVICE_ID + " IS NULL LIMIT 1), " +
            "		?, " +
            "		(SELECT " + NwdContract.COLUMN_MEDIA_PATH_ID + " FROM " + NwdContract.TABLE_MEDIA_PATH + " WHERE " + NwdContract.COLUMN_MEDIA_PATH_VALUE + " = ? LIMIT 1) " +
            "	) ";

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
                        catch(Exception ex)
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
            
            cmd.Parameters.Add(new SQLiteParameter() { Value = 1 });
            cmd.Parameters.Add(new SQLiteParameter() { Value = 2 });
            
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
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
