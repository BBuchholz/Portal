using NineWorldsDeep.Core;
using NineWorldsDeep.Mnemosyne.V5;
using NineWorldsDeep.Model;
using NineWorldsDeep.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<MediaTagging> GetTaggedMediaTaggingsForHash(string hash)
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
                        lst = GetTaggedMediaTaggingsForHash(hash, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
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
        
        private List<MediaTagging> GetTaggedMediaTaggingsForHash(
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
                                    mediaId = EnsureMediaHash(hash, cmd);
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

        private int EnsureMediaHash(string hash, SQLiteCommand cmd)
        {
            InsertOrIgnoreHashForMedia(hash, cmd);
            return GetMediaIdForHash(hash, cmd);
        }

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
                        id = EnsureMediaHash(hash, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return id;
        }

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
        
        public void InsertPathsForDeviceId(int mediaDeviceId, List<string> paths)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        InsertPathsForDeviceId(mediaDeviceId, paths, cmd);

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

        private int GetLocalMediaDeviceId()
        {
            int id = -1;
            string deviceDesription = Configuration.GetLocalDeviceDescription();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        id = SelectMediaDeviceId(deviceDesription, cmd);

                        if(id < 0)
                        {
                            InsertMediaDevice(deviceDesription, cmd);
                            id = SelectMediaDeviceId(deviceDesription, cmd);
                        }

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            if(id < 0)
            {
                throw new Exception("Error retrieving id for Media Device: " +
                    deviceDesription);
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

        private int EnsureMediaTag(string tag, SQLiteCommand cmd)
        {
            InsertOrIgnoreMediaTag(tag, cmd);
            return GetMediaTagId(tag, cmd);
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

                InsertMediaTagging(mt, cmd);
                UpdateMediaTagging(mt, cmd);
            }
        }

        private void UpdateMediaTagging(MediaTagging mt, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.UPDATE_MEDIA_TAGGING_TAGGED_UNTAGGED_WHERE_MEDIA_ID_AND_TAG_ID_W_X_Y_Z;

            SQLiteParameter taggedParam = new SQLiteParameter();
            taggedParam.Value = 
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(mt.TaggedAt);
            cmd.Parameters.Add(taggedParam);

            SQLiteParameter untaggedParam = new SQLiteParameter();
            untaggedParam.Value = 
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(mt.UntaggedAt);
            cmd.Parameters.Add(untaggedParam);

            SQLiteParameter mediaIdParam = new SQLiteParameter();
            mediaIdParam.Value = mt.MediaId;
            cmd.Parameters.Add(mediaIdParam);

            SQLiteParameter mediaTagIdParam = new SQLiteParameter();
            mediaTagIdParam.Value = mt.MediaTagId;
            cmd.Parameters.Add(mediaTagIdParam);

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
            int mediaDeviceId, List<string> paths, SQLiteCommand cmd)
        {            
            foreach(string path in paths)
            {
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
