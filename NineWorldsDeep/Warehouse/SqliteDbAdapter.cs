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
        private Db.SqliteDbAdapter db =
            new Db.SqliteDbAdapter();

        public SqliteDbAdapter()
        {
            db.InitializeIds();
        }
        
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
                            db.PopulateSyncProfiles(lst, cmd);

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
            return db.GetIdsForTags(tags, cmd);
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
                            int profileId = db.EnsureProfileId(profileName, cmd);

                            //get external deviceId for profile
                            int extDeviceId = db.GetExtDeviceIdForProfileId(profileId, cmd);

                            if (extDeviceId != -1)
                            {
                                //upsert file (to update hashedAt timestamp if already exists)
                                string timeStamp = NwdUtils.GetTimeStamp_yyyyMMddHHmmss();
                                int fileId = db.UpsertFile(extDeviceId, extPathId, hashId, timeStamp, cmd);

                                //foreach tag, link File to Tag (junction table entry)
                                LinkFileIdToTagIds(fileId, tagIds, cmd);
                            }

                            //get host deviceId for profile
                            int hostDeviceId = db.GetHostDeviceIdForProfileId(profileId, cmd);

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
            return db.GetErdRawSource();
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

                            db.DeleteSyncMap(sm, cmd);

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
                            int profileId = db.EnsureProfileId(sp.Name, cmd);

                            List<string> paths = sp.SyncMaps.AllPaths();

                            foreach (string path in paths)
                            {
                                db.InsertOrIgnorePath(path, cmd);
                            }

                            /////////use open transaction to get path ids for path values
                            Dictionary<string, int> pathIds = new Dictionary<string, int>();

                            //store all paths
                            foreach (string path in paths)
                            {
                                pathIds[path] = -1;
                            }

                            db.RefreshPathIds(pathIds, cmd);

                            foreach (SyncMap map in sp.SyncMaps)
                            {
                                int destId = pathIds[map.Destination];
                                int srcId = pathIds[map.Source];
                                //int directionId = directionIds[map.SyncDirection];
                                //int actionId = actionIds[map.DefaultSyncAction];
                                int directionId = db.GetDirectionId(map.SyncDirection);
                                int actionId = db.GetActionId(map.DefaultSyncAction);

                                db.UpsertSyncMap(profileId, srcId, destId, directionId, actionId, cmd);
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

                            db.EnsureProfileId(profileName, cmd);

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
        
        private void InsertOrIgnoreTags(List<string> tags, SQLiteCommand cmd)
        {
            foreach (var tag in tags)
            {
                db.InsertOrIgnoreTag(tag, cmd);
            }
        }

    }
}