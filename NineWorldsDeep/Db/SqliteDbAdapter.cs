using NineWorldsDeep.Core;
using NineWorldsDeep.Mtp;
using NineWorldsDeep.Parser;
using NineWorldsDeep.Sqlite;
using NineWorldsDeep.UI;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Db
{
    public class SqliteDbAdapter
    {        
        private Dictionary<NwdDeviceKey, int> deviceIds =
            new Dictionary<NwdDeviceKey, int>();
        private Dictionary<string, int> hashIds =
            new Dictionary<string, int>();
        private Dictionary<string, int> pathIds =
            new Dictionary<string, int>();

        //TODO: consolidate all db logic from all of NWD into one class with a private constructor (singleton)
        //TODO: in the db singleton, enable the foreign key pragma when opening sqlite db

        public SqliteDbAdapter()
        {
        }

        ///// <summary>
        ///// assumes conn opened and closed outside this method
        ///// </summary>
        ///// <param name="lst"></param>
        ///// <param name="conn"></param>
        //public void StoreHashes(List<string> lst, SQLiteConnection conn)
        //{
        //    //INSERT OR IGNORE            
        //    using (var cmd = new SQLiteCommand(conn))
        //    {
        //        using (var transaction = conn.BeginTransaction())
        //        {
        //            foreach (var hash in lst)
        //            {
        //                if (!string.IsNullOrWhiteSpace(hash))
        //                {
        //                    cmd.CommandText =
        //                        "INSERT OR IGNORE INTO Hash (HashValue) VALUES (@hashValue)";
        //                    cmd.Parameters.AddWithValue("@hashValue", hash);
        //                    cmd.ExecuteNonQuery();
        //                }
        //            }

        //            transaction.Commit();
        //        }
        //    }
        //}

        /// <summary>
        /// assumes conn opened and closed outside this method
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="conn"></param>
        //public void StoreTags(List<string> lst, SQLiteConnection conn)
        //{
        //    //INSERT OR IGNORE            
        //    using (var cmd = new SQLiteCommand(conn))
        //    {
        //        using (var transaction = conn.BeginTransaction())
        //        {
        //            foreach (var tag in lst)
        //            {
        //                if (!string.IsNullOrWhiteSpace(tag))
        //                {
        //                    throw new NotImplementedException("create tag table in db then implement");
        //                    //cmd.CommandText =
        //                    //    "INSERT OR IGNORE INTO Hash (HashValue) VALUES (@hashValue)";
        //                    //cmd.Parameters.AddWithValue("@hashValue", hash);
        //                    //cmd.ExecuteNonQuery();
        //                }
        //            }

        //            transaction.Commit();
        //        }
        //    }
        //}

        /// <summary>
        /// returns -1 if not found, or id if found
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public int GetIdForHash(string hash, SQLiteCommand cmd)
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

        public void InsertOrIgnoreHash(string hash, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText =
                //"INSERT OR IGNORE INTO Hash (HashValue) VALUES (@hash)";
                "INSERT OR IGNORE INTO " + NwdContract.TABLE_HASH +
                    " (" + NwdContract.COLUMN_HASH_VALUE + ") VALUES (@hash)";

            cmd.Parameters.AddWithValue("@hash", hash);
            cmd.ExecuteNonQuery();
        }

        public void StoreTags(List<string> lst)
        {
            //INSERT OR IGNORE
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
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

        public void SaveToDb()
        {

            //ensure tags
            //ensure paths
            //link all
        }

        //public void StoreHashes(List<NwdUriProcessEntry> lst)
        //{
        //    //INSERT OR IGNORE
        //    using (var conn = new SQLiteConnection(
        //        @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
        //    {
        //        conn.Open();

        //        var stopwatch = new Stopwatch();
        //        stopwatch.Start();

        //        using (var cmd = new SQLiteCommand(conn))
        //        {
        //            using (var transaction = conn.BeginTransaction())
        //            {
        //                foreach (var pe in lst)
        //                {
        //                    if (!string.IsNullOrWhiteSpace(pe.Hash))
        //                    {
        //                        cmd.CommandText =
        //                            "INSERT OR IGNORE INTO Hash (HashValue) VALUES (@hashValue)";
        //                        cmd.Parameters.AddWithValue("@hashValue", pe.NwdUri.Hash);
        //                        cmd.ExecuteNonQuery();
        //                    }
        //                }

        //                transaction.Commit();
        //            }
        //        }

        //        Display.Message(stopwatch.Elapsed.TotalSeconds +
        //            " seconds with one transaction.");

        //        conn.Close();
        //    }
        //}

        ///// <summary>
        ///// assumes conn opened and closed outside of this method
        ///// </summary>
        ///// <param name="lst"></param>
        ///// <param name="conn"></param>
        //public void StorePaths(List<string> lst, SQLiteConnection conn)
        //{
        //    //INSERT OR IGNORE            
        //    using (var cmd = new SQLiteCommand(conn))
        //    {
        //        using (var transaction = conn.BeginTransaction())
        //        {
        //            foreach (var path in lst)
        //            {
        //                if (!string.IsNullOrWhiteSpace(path))
        //                {
        //                    cmd.CommandText =
        //                        "INSERT OR IGNORE INTO Path (PathValue) VALUES (@pathValue)";
        //                    cmd.Parameters.AddWithValue("@pathValue", path);
        //                    cmd.ExecuteNonQuery();
        //                }
        //            }

        //            transaction.Commit();
        //        }
        //    }
        //}

        public void StorePaths(List<string> lst)
        {
            //INSERT OR IGNORE
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
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
        
        public string GetTagsForHash(string sha1Hash, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText =
                //"SELECT t.TagValue " +
                //"FROM Tag t " +
                //"JOIN junction_File_Tag jft " +
                //"ON t.TagId = jft.TagId " +
                //"JOIN File f " +
                //"ON f.FileId = jft.FileId " +
                //"JOIN Hash h " +
                //"ON h.HashId = f.HashId " +
                //"WHERE h.HashValue = @hash ";
                "SELECT t." + NwdContract.COLUMN_TAG_VALUE + " " +
                "FROM " + NwdContract.TABLE_TAG + " t " +
                "JOIN " + NwdContract.TABLE_JUNCTION_FILE_TAG + " jft " +
                "ON t." + NwdContract.COLUMN_TAG_ID + " = jft." + NwdContract.COLUMN_TAG_ID + " " +
                "JOIN " + NwdContract.TABLE_FILE + " f " +
                "ON f." + NwdContract.COLUMN_FILE_ID + " = jft." + NwdContract.COLUMN_FILE_ID + " " +
                "JOIN " + NwdContract.TABLE_HASH + " h " +
                "ON h." + NwdContract.COLUMN_HASH_ID + " = f." + NwdContract.COLUMN_HASH_ID + " " +
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


        //public void StorePaths(List<NwdUriProcessEntry> lst)
        //{
        //    //INSERT OR IGNORE
        //    using (var conn = new SQLiteConnection(
        //        @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
        //    {
        //        conn.Open();

        //        var stopwatch = new Stopwatch();
        //        stopwatch.Start();

        //        using (var cmd = new SQLiteCommand(conn))
        //        {
        //            using (var transaction = conn.BeginTransaction())
        //            {
        //                foreach (var pe in lst)
        //                {
        //                    if (!string.IsNullOrWhiteSpace(pe.NwdUri.Hash))
        //                    {
        //                        cmd.CommandText =
        //                            "INSERT OR IGNORE INTO Path (PathValue) VALUES (@pathValue)";
        //                        cmd.Parameters.AddWithValue("@pathValue", pe.Path);
        //                        cmd.ExecuteNonQuery();
        //                    }
        //                }

        //                transaction.Commit();
        //            }
        //        }

        //        Display.Message(stopwatch.Elapsed.TotalSeconds +
        //            " seconds with one transaction.");

        //        conn.Close();
        //    }
        //}

        /// <summary>
        /// uses conn for chaining multiple id refreshes with one 
        /// connection. requires conn be opened before 
        /// passing to method, and leaves conn open when
        /// done (to be used by other refresh methods).
        /// 
        /// Be sure to close connection when finished (including
        /// all refresh statements in a single using block
        /// is the intended usage)
        /// </summary>
        /// <param name="conn"></param>
        //public void RefreshHashIds(SQLiteConnection conn)
        //{
        //    string cmdStr = "SELECT HashValue, HashId FROM Hash";

        //    using (var cmd = new SQLiteCommand(cmdStr, conn))
        //    {
        //        using (var rdr = cmd.ExecuteReader())
        //        {
        //            while (rdr.Read())
        //            {
        //                hashIds.Add(rdr.GetString(0), rdr.GetInt32(1));
        //            }
        //        }
        //    }
        //}

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
        public List<PathTagLink> GetPathTagLinks(string filePathTopFolder)
        {
            //TODO: this should take device into account (overload method to default to this device if one is not supplied)
            List<PathTagLink> lst = new List<PathTagLink>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
            {
                conn.Open();
                
                //TODO: may be able to move path filter into a LIKE clause
                //but should test which is faster, for now just
                //filtering in code
                string cmdStr =
                    //"SELECT PathValue, " +
                    //        "TagValue " +
                    //"FROM Path " +
                    //"JOIN File " +
                    //"ON Path.PathId = File.PathId " +
                    //"JOIN junction_File_Tag " +
                    //"ON File.FileId = junction_File_Tag.FileId " +
                    //"JOIN Tag " +
                    //"ON junction_File_Tag.TagId = Tag.TagId";
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
                    "JOIN " + NwdContract.TABLE_JUNCTION_FILE_TAG + " " +
                    "ON " + 
                        NwdContract.TABLE_FILE + "." + 
                            NwdContract.COLUMN_FILE_ID + 
                        " = " + 
                        NwdContract.TABLE_JUNCTION_FILE_TAG + "." + 
                            NwdContract.COLUMN_FILE_ID + " " +
                    "JOIN " + NwdContract.TABLE_TAG + " " +
                    "ON " + 
                        NwdContract.TABLE_JUNCTION_FILE_TAG + "." + 
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
                                lst.Add(new PathTagLink() {
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

        public string DeleteFile(String device, String path)
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
                        Configuration.GetSqliteDbPath("nwd")))
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

        public int GetFileIdForDevicePath(String device, 
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

        public int GetIdForFile(int deviceId, int pathId, SQLiteCommand cmd)
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

        public int UpsertFile(int deviceId, int pathId, int hashId, string hashedAtTimeStamp, SQLiteCommand cmd)
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
                //"UPDATE OR IGNORE File " +
                //"SET HashId = @hashId, " +
                //    "FileHashedAt = @hashedAt " +
                //"WHERE DeviceId = @deviceId " +
                //"AND PathId = @pathId ";
                "UPDATE OR IGNORE " + NwdContract.TABLE_FILE + " " +
                "SET " + NwdContract.COLUMN_HASH_ID + " = @hashId, " +
                    "" + NwdContract.COLUMN_FILE_HASHED_AT + " = @hashedAt " +
                "WHERE " + NwdContract.COLUMN_DEVICE_ID + " = @deviceId " +
                "AND " + NwdContract.COLUMN_PATH_ID + " = @pathId ";

            cmd.Parameters.AddWithValue("@hashId", hashId);
            cmd.Parameters.AddWithValue("@hashedAt", hashedAtTimeStamp);
            cmd.Parameters.AddWithValue("@deviceId", deviceId);
            cmd.Parameters.AddWithValue("@pathId", pathId);
            cmd.ExecuteNonQuery();

            //INSERT or IGNORE
            cmd.Parameters.Clear();
            cmd.CommandText =
                //"INSERT OR IGNORE INTO File (DeviceId, " +
                //                             "PathId, " +
                //                             "HashId, " +
                //                             "FileHashedAt) " +
                //"VALUES (@deviceId, " +
                //        "@pathId, " +
                //        "@hashId, " +
                //        "@hashedAt)";
                "INSERT OR IGNORE INTO " + NwdContract.TABLE_FILE +
                " (" + NwdContract.COLUMN_DEVICE_ID + ", " +
                  "" + NwdContract.COLUMN_PATH_ID + ", " +
                  "" + NwdContract.COLUMN_HASH_ID + ", " +
                  "" + NwdContract.COLUMN_FILE_HASHED_AT + ") " +
                "VALUES (@deviceId, " +
                        "@pathId, " +
                        "@hashId, " +
                        "@hashedAt)";

            cmd.Parameters.AddWithValue("@deviceId", deviceId);
            cmd.Parameters.AddWithValue("@pathId", pathId);
            cmd.Parameters.AddWithValue("@hashId", hashId);
            cmd.Parameters.AddWithValue("@hashedAt", hashedAtTimeStamp);
            cmd.ExecuteNonQuery();

            return GetIdForFile(deviceId, pathId, cmd);
        }

        public void LinkFileIdToTagId(int fileId, int tagId, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText =
                //"INSERT OR IGNORE INTO junction_File_Tag (FileId, TagId) VALUES (@fileId, @tagId) ";
                "INSERT OR IGNORE INTO " +
                    NwdContract.TABLE_JUNCTION_FILE_TAG +
                    " (" +
                        NwdContract.COLUMN_FILE_ID + ", " +
                        NwdContract.COLUMN_TAG_ID +
                     ") " +
                "VALUES (@fileId, @tagId) ";

            cmd.Parameters.AddWithValue("@fileId", fileId);
            cmd.Parameters.AddWithValue("@tagId", tagId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteFileTagsForFileId(int fileId, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                "DELETE FROM " + NwdContract.TABLE_JUNCTION_FILE_TAG + 
                " WHERE " + NwdContract.COLUMN_FILE_ID + " = ? ";
            
            SQLiteParameter fileIdParam = new SQLiteParameter();
            fileIdParam.Value = fileId;
            cmd.Parameters.Add(fileIdParam);
            
            cmd.ExecuteNonQuery();
        }

        public void DeleteFileForFileId(int fileId, SQLiteCommand cmd)
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

        public string DeleteFile(String path)
        {
            return DeleteFile(null, path);
        }

        public void PopulatePathIds(Dictionary<string, int> pathsToIds)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
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

        public void StoreFileTags(List<PathToTagMapping> mappings)
        {
            //INSERT OR IGNORE
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (var mapping in mappings)
                        {
                            cmd.CommandText =
                                //"INSERT OR IGNORE INTO junction_File_Tag (FileId, TagId) VALUES (@fileId, @tagId)";
                                "INSERT OR IGNORE INTO " + 
                                    NwdContract.TABLE_JUNCTION_FILE_TAG + 
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

        public void StoreDeviceFiles(List<PathToTagMapping> mappings)
        {
            //INSERT OR IGNORE
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
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

        public void StorePathToTagMappings(List<PathToTagMapping> mappings)
        {
            StoreDeviceFiles(mappings);
            PopulateFileIds(mappings);
            StoreFileTags(mappings);
        }

        private void PopulateFileIds(List<PathToTagMapping> mappings)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
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
                            foreach(var m in mappings)
                            {
                                if(m.DeviceId == deviceId &&
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
                new NwdDeviceKey(description,
                                 friendlyName,
                                 model,
                                 deviceType);

            if (deviceIds.ContainsKey(deviceKey))
            {
                return deviceIds[deviceKey];
            }

            return -1;
        }

        public void PopulateTagIds(Dictionary<string, int> tagsToIds)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
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

        /// <summary>
        /// uses conn for chaining multiple id refreshes with one 
        /// connection. requires conn be opened before 
        /// passing to method, and leaves conn open when
        /// done (to be used by other refresh methods).
        /// 
        /// Be sure to close connection when finished (including
        /// all refresh statements in a single using block
        /// is the intended usage)
        /// </summary>
        /// <param name="conn"></param>
        //public void RefreshPathIds(SQLiteConnection conn)
        //{
        //    string cmdStr = "SELECT PathValue, PathId FROM Path";

        //    using (var cmd = new SQLiteCommand(cmdStr, conn))
        //    {
        //        using (var rdr = cmd.ExecuteReader())
        //        {
        //            while (rdr.Read())
        //            {
        //                pathIds.Add(rdr.GetString(0), rdr.GetInt32(1));
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// uses conn for chaining multiple id refreshes with one 
        ///// connection. requires conn be opened before 
        ///// passing to method, and leaves conn open when
        ///// done (to be used by other refresh methods).
        ///// 
        ///// Be sure to close connection when finished (including
        ///// all refresh statements in a single using block
        ///// is the intended usage)
        ///// </summary>
        ///// <param name="conn"></param>
        //public void PopulateIds(List<NwdUriProcessEntry> lst, 
        //                        SQLiteConnection conn)
        //{
        //    RefreshHashIds(conn);

        //    RefreshPathIds(conn);

        //    foreach (var pe in lst)
        //    {
        //        EnsureAndPopulateDeviceId(pe);

        //        if (!string.IsNullOrWhiteSpace(pe.Path) &&
        //            pathIds.ContainsKey(pe.Path))
        //        {
        //            pe.PathId = pathIds[pe.Path];
        //        }

        //        if (!string.IsNullOrWhiteSpace(pe.Hash) &&
        //            hashIds.ContainsKey(pe.Hash))
        //        {
        //            pe.HashId = hashIds[pe.Hash];
        //        }
        //    }
        //}
        
        //[Obsolete("use PopulateIds(List<NwdUriProcessEntry, SQLiteConnection)")]
        //public void PopulateIds(List<NwdUriProcessEntry> lst)
        //{
        //    Dictionary<string, int> hashes =
        //        new Dictionary<string, int>();
            
        //    Dictionary<string, int> paths =
        //        new Dictionary<string, int>();

        //    //foreach entry, get id for hash and path from db
        //    using (var conn = new SQLiteConnection(
        //        @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
        //    {
        //        conn.Open();

        //        var stopwatch = new Stopwatch();
        //        stopwatch.Start();

        //        string cmdStr = "SELECT HashValue, HashId FROM Hash";

        //        using (var cmd = new SQLiteCommand(cmdStr, conn))
        //        {
        //            using (var rdr = cmd.ExecuteReader())
        //            {
        //                while (rdr.Read())
        //                {
        //                    hashes.Add(rdr.GetString(0), rdr.GetInt32(1));
        //                }
        //            }
        //        }

        //        Display.Message(stopwatch.Elapsed.TotalSeconds +
        //            " seconds for hash ids.");

        //        stopwatch = new Stopwatch();
        //        stopwatch.Start();

        //        cmdStr = "SELECT PathValue, PathId FROM Path";

        //        using (var cmd = new SQLiteCommand(cmdStr, conn))
        //        {
        //            using (var rdr = cmd.ExecuteReader())
        //            {
        //                while (rdr.Read())
        //                {
        //                    paths.Add(rdr.GetString(0), rdr.GetInt32(1));
        //                }
        //            }
        //        }

        //        Display.Message(stopwatch.Elapsed.TotalSeconds +
        //            " seconds for path ids.");
                
        //        conn.Close();
        //    }

        //    foreach (var pe in lst)
        //    {
        //        EnsureAndPopulateDeviceId(pe);
                
        //        if (!string.IsNullOrWhiteSpace(pe.Path) &&
        //            paths.ContainsKey(pe.Path))
        //        {
        //            pe.PathId = paths[pe.Path];
        //        }

        //        if (!string.IsNullOrWhiteSpace(pe.Hash) &&
        //            hashes.ContainsKey(pe.Hash))
        //        {
        //            pe.HashId = hashes[pe.Hash];
        //        }
        //    }
        //}

        /// <summary>
        /// used for chaining multiple id refreshes with one 
        /// connection. requires conn be opened before 
        /// passing to method, and leaves conn open when
        /// done (to be used by other refresh methods).
        /// 
        /// Be sure to close connection when finished (including
        /// all refresh statements in a single using block
        /// is the intended usage)
        /// </summary>
        /// <param name="conn"></param>
        //private void RefreshDeviceIds(SQLiteConnection conn)
        //{            
        //    string cmdStr = "SELECT DeviceDescription, " +
        //                            "DeviceFriendlyName, " +
        //                            "DeviceModel, " +
        //                            "DeviceType, " +
        //                            "DeviceId " +
        //                    "FROM Device";

        //    using (var cmd = new SQLiteCommand(cmdStr, conn))
        //    {
        //        using (var rdr = cmd.ExecuteReader())
        //        {
        //            while (rdr.Read())
        //            {
        //                string desc = rdr.GetString(0);
        //                string friendlyName = rdr.GetString(1);
        //                string model = rdr.GetString(2);
        //                string deviceType = rdr.GetString(3);

        //                deviceIds.Add(new NwdDeviceKey()
        //                {
        //                    Description = desc,
        //                    FriendlyName = friendlyName,
        //                    Model = model,
        //                    DeviceType = deviceType
        //                }, rdr.GetInt32(4));
        //            }
        //        }
        //    }                
        //}

        private void RefreshDeviceIds()
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
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
                    "" + NwdContract.COLUMN_DEVICE_FRIENDLY_NAME + ", " +
                    "" + NwdContract.COLUMN_DEVICE_MODEL + ", " +
                    "" + NwdContract.COLUMN_DEVICE_TYPE + ", " +
                    "" + NwdContract.COLUMN_DEVICE_ID + " " +
                "FROM " + NwdContract.TABLE_DEVICE + "";

                using (var cmd = new SQLiteCommand(cmdStr, conn))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string desc = rdr.GetString(0);
                            string friendlyName = rdr.GetString(1);
                            string model = rdr.GetString(2);
                            string deviceType = rdr.GetString(3);

                            deviceIds.Add(new NwdDeviceKey() {
                                Description = desc,
                                FriendlyName = friendlyName,
                                Model = model,
                                DeviceType = deviceType
                            }, rdr.GetInt32(4));                                                        
                        }
                    }
                }

                conn.Close();
            }
        }

        private NwdDeviceKey ToDeviceKey(NwdPortableDevice device)
        {
            if(device == null)
            {
                throw new Exception("NwdPortableDevice null in method ToDeviceKey()");
            }
        
            return new NwdDeviceKey()
            {
                Description = device.Description,
                FriendlyName = device.FriendlyName,
                Model = device.Model,
                DeviceType = device.DeviceType
            };
        }

        //private void EnsureAndPopulateDeviceId(NwdUriProcessEntry pe)
        //{
        //    var key = ToDeviceKey(pe.PortableDevice);

        //    if (!deviceIds.ContainsKey(key))
        //    {
        //        StoreDevice(pe.PortableDevice);
        //        RefreshDeviceIds();
        //    }

        //    pe.DeviceId = deviceIds[key];
        //}
        
        //public void StoreHashPathJunctions(List<NwdUriProcessEntry> lst)
        //{
        //    //if both hash and path ids are populated, INSERT OR IGNORE each
        //    using (var conn = new SQLiteConnection(
        //        @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
        //    {
        //        conn.Open();

        //        var stopwatch = new Stopwatch();
        //        stopwatch.Start();

        //        using (var cmd = new SQLiteCommand(conn))
        //        {
        //            using (var transaction = conn.BeginTransaction())
        //            {
        //                foreach (var pe in lst)
        //                {
        //                    if (!string.IsNullOrWhiteSpace(pe.Hash) &&
        //                        !string.IsNullOrWhiteSpace(pe.Path))
        //                    {
        //                        cmd.CommandText =
        //                            "INSERT OR IGNORE INTO File (DeviceId, PathId, HashId) VALUES (@deviceId, @pathId, @hashId)";
        //                        cmd.Parameters.AddWithValue("@deviceId", pe.DeviceId);
        //                        cmd.Parameters.AddWithValue("@pathId", pe.PathId);
        //                        cmd.Parameters.AddWithValue("@hashId", pe.HashId);
        //                        cmd.ExecuteNonQuery();
        //                    }
        //                }

        //                transaction.Commit();
        //            }
        //        }

        //        Display.Message(stopwatch.Elapsed.TotalSeconds +
        //            " seconds with one transaction.");

        //        conn.Close();
        //    }
        //}

        ///// <summary>
        ///// executes an INSERT OR IGNORE statement for supplied device info,
        ///// intended for use with device nodes not meeting the standard NwdPortableDevice
        ///// format (laptops, desktops, remote servers, &c.)
        ///// 
        ///// Assumes conn opened and closed outside of method for bulk chaining
        ///// 
        ///// Any null or whitespace values will result in no database changes
        ///// </summary>
        ///// <param name="device"></param>
        //public void StoreDevice(string description,
        //                        string friendlyName,
        //                        string model,
        //                        string deviceType,
        //                        SQLiteConnection conn)
        //{
        //    using (var cmd = new SQLiteCommand(conn))
        //    {
        //        using (var transaction = conn.BeginTransaction())
        //        {
        //            //TODO: change this to default each to "" if null or whitespace, change method summary when you do so (it currently mentions this limitation)
        //            if (!string.IsNullOrWhiteSpace(description) &&
        //                !string.IsNullOrWhiteSpace(friendlyName) &&
        //                !string.IsNullOrWhiteSpace(model) &&
        //                !string.IsNullOrWhiteSpace(deviceType))
        //            {                        
        //                cmd.CommandText =
        //                    "INSERT OR IGNORE INTO Device " +
        //                        "(DeviceDescription, " +
        //                        "DeviceFriendlyName, " +
        //                        "DeviceModel, " +
        //                        "DeviceType) " +
        //                    "VALUES (@deviceDescription, " +
        //                            "@deviceFriendlyName, " +
        //                            "@deviceModel, " +
        //                            "@deviceType)";

        //                cmd.Parameters.AddWithValue("@deviceDescription", description);
        //                cmd.Parameters.AddWithValue("@deviceFriendlyName", friendlyName);
        //                cmd.Parameters.AddWithValue("@deviceModel", model);
        //                cmd.Parameters.AddWithValue("@deviceType", deviceType);
        //                cmd.ExecuteNonQuery();
        //            }

        //            transaction.Commit();
        //        }

        //    }
        //}

        /// <summary>
        /// executes an INSERT OR IGNORE statement for supplied device info,
        /// intended for use with device nodes not meeting the standard NwdPortableDevice
        /// format (laptops, desktops, remote servers, &c.)
        /// 
        /// Any null or whitespace values will result in no database changes
        /// </summary>
        /// <param name="device"></param>
        public void StoreDevice(string description,
                                string friendlyName,
                                string model,
                                string deviceType)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        //TODO: change this to default each to "" if null or whitespace, change method summary when you do so (it currently mentions this limitation)
                        if (!string.IsNullOrWhiteSpace(description) &&
                            !string.IsNullOrWhiteSpace(friendlyName) &&
                            !string.IsNullOrWhiteSpace(model) &&
                            !string.IsNullOrWhiteSpace(deviceType))
                        {
                            cmd.CommandText =
                                //"INSERT OR IGNORE INTO Device " +
                                //    "(DeviceDescription, " +
                                //    "DeviceFriendlyName, " +
                                //    "DeviceModel, " +
                                //    "DeviceType) " +
                                //"VALUES (@deviceDescription, " +
                                //        "@deviceFriendlyName, " +
                                //        "@deviceModel, " +
                                //        "@deviceType)";

                                "INSERT OR IGNORE INTO " + 
                                    NwdContract.TABLE_DEVICE + " " +
                                        "(" + 
                                            NwdContract.COLUMN_DEVICE_DESCRIPTION + ", " +
                                            NwdContract.COLUMN_DEVICE_FRIENDLY_NAME + ", " +
                                            NwdContract.COLUMN_DEVICE_MODEL + ", " +
                                            NwdContract.COLUMN_DEVICE_TYPE + 
                                        ") " +
                                "VALUES (@deviceDescription, " +
                                        "@deviceFriendlyName, " +
                                        "@deviceModel, " +
                                        "@deviceType)";

                            cmd.Parameters.AddWithValue("@deviceDescription", description);
                            cmd.Parameters.AddWithValue("@deviceFriendlyName", friendlyName);
                            cmd.Parameters.AddWithValue("@deviceModel", model);
                            cmd.Parameters.AddWithValue("@deviceType", deviceType);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }
            }
        }

        ///// <summary>
        ///// executes an INSERT OR IGNORE statement for supplied device
        ///// </summary>
        ///// <param name="device"></param>
        //public void StoreDevice(NwdPortableDevice device)
        //{
        //    using (var conn = new SQLiteConnection(
        //        @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
        //    {
        //        conn.Open();

        //        var stopwatch = new Stopwatch();
        //        stopwatch.Start();

        //        using (var cmd = new SQLiteCommand(conn))
        //        {
        //            using (var transaction = conn.BeginTransaction())
        //            {
        //                if (device != null)
        //                {
        //                    //convert null values to empty strings
        //                    ConvertNullsToEmptyStrings(device);

        //                    cmd.CommandText =
        //                        "INSERT OR IGNORE INTO Device " +
        //                            "(DeviceDescription, " +
        //                            "DeviceFriendlyName, " +
        //                            "DeviceModel, " +
        //                            "DeviceType) " +
        //                        "VALUES (@deviceDescription, " +
        //                                "@deviceFriendlyName, " +
        //                                "@deviceModel, " +
        //                                "@deviceType)";

        //                    cmd.Parameters.AddWithValue("@deviceDescription", device.Description);
        //                    cmd.Parameters.AddWithValue("@deviceFriendlyName", device.FriendlyName);
        //                    cmd.Parameters.AddWithValue("@deviceModel", device.Model);
        //                    cmd.Parameters.AddWithValue("@deviceType", device.DeviceType);
        //                    cmd.ExecuteNonQuery();
        //                }
                        
        //                transaction.Commit();
        //            }
        //        }

        //        Display.Message(stopwatch.Elapsed.TotalSeconds +
        //            " seconds to insert one device.");

        //        conn.Close();
        //    }
        //}

        private void ConvertNullsToEmptyStrings(NwdPortableDevice device)
        {
            if(device.Description == null)
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


        /////////////////////////////////////connection/transaction template        
        //
        //private void ExecuteNonQueryTemplate(string xxx, SQLiteCommand cmd)
        //{
        //    cmd.Parameters.Clear();
        //    cmd.CommandText =
        //        "INSERT OR IGNORE INTO XXX (XXXValue) VALUES (?)";
        //    SQLiteParameter param = new SQLiteParameter();
        //    param.Value = xxx;
        //    cmd.Parameters.Add(param);
        //    cmd.ExecuteNonQuery();
        //}
        //
        //public string TransactionTemplate()
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
