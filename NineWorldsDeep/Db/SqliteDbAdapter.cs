using NineWorldsDeep.Core;
using NineWorldsDeep.Mtp;
using NineWorldsDeep.Parser;
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

        /// <summary>
        /// assumes conn opened and closed outside this method
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="conn"></param>
        public void StoreHashes(List<string> lst, SQLiteConnection conn)
        {
            //INSERT OR IGNORE            
            using (var cmd = new SQLiteCommand(conn))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var hash in lst)
                    {
                        if (!string.IsNullOrWhiteSpace(hash))
                        {
                            cmd.CommandText =
                                "INSERT OR IGNORE INTO Hash (HashValue) VALUES (@hashValue)";
                            cmd.Parameters.AddWithValue("@hashValue", hash);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// assumes conn opened and closed outside this method
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="conn"></param>
        public void StoreTags(List<string> lst, SQLiteConnection conn)
        {
            //INSERT OR IGNORE            
            using (var cmd = new SQLiteCommand(conn))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var tag in lst)
                    {
                        if (!string.IsNullOrWhiteSpace(tag))
                        {
                            throw new NotImplementedException("create tag table in db then implement");
                            //cmd.CommandText =
                            //    "INSERT OR IGNORE INTO Hash (HashValue) VALUES (@hashValue)";
                            //cmd.Parameters.AddWithValue("@hashValue", hash);
                            //cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }
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
                                    "INSERT OR IGNORE INTO Tag (TagValue) VALUES (@tagValue)";
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

        public void StoreHashes(List<NwdUriProcessEntry> lst)
        {
            //INSERT OR IGNORE
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
            {
                conn.Open();

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (var pe in lst)
                        {
                            if (!string.IsNullOrWhiteSpace(pe.Hash))
                            {
                                cmd.CommandText =
                                    "INSERT OR IGNORE INTO Hash (HashValue) VALUES (@hashValue)";
                                cmd.Parameters.AddWithValue("@hashValue", pe.NwdUri.Hash);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                }

                Display.Message(stopwatch.Elapsed.TotalSeconds +
                    " seconds with one transaction.");

                conn.Close();
            }
        }

        /// <summary>
        /// assumes conn opened and closed outside of this method
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="conn"></param>
        public void StorePaths(List<string> lst, SQLiteConnection conn)
        {
            //INSERT OR IGNORE            
            using (var cmd = new SQLiteCommand(conn))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var path in lst)
                    {
                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            cmd.CommandText =
                                "INSERT OR IGNORE INTO Path (PathValue) VALUES (@pathValue)";
                            cmd.Parameters.AddWithValue("@pathValue", path);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }
        }

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
                                    "INSERT OR IGNORE INTO Path (PathValue) VALUES (@pathValue)";
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

        public void StorePaths(List<NwdUriProcessEntry> lst)
        {
            //INSERT OR IGNORE
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
            {
                conn.Open();

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (var pe in lst)
                        {
                            if (!string.IsNullOrWhiteSpace(pe.NwdUri.Hash))
                            {
                                cmd.CommandText =
                                    "INSERT OR IGNORE INTO Path (PathValue) VALUES (@pathValue)";
                                cmd.Parameters.AddWithValue("@pathValue", pe.Path);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                }

                Display.Message(stopwatch.Elapsed.TotalSeconds +
                    " seconds with one transaction.");

                conn.Close();
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
        public void RefreshHashIds(SQLiteConnection conn)
        {
            string cmdStr = "SELECT HashValue, HashId FROM Hash";

            using (var cmd = new SQLiteCommand(cmdStr, conn))
            {
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        hashIds.Add(rdr.GetString(0), rdr.GetInt32(1));
                    }
                }
            }
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
        public List<PathTagLink> GetPathTagLinks(string filePathTopFolder)
        {
            List<PathTagLink> lst = new List<PathTagLink>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
            {
                conn.Open();
                
                //TODO: may be able to move path filter into a LIKE clause
                //but should test which is faster, for now just
                //filtering in code
                string cmdStr = "SELECT PathValue, " +
                                       "TagValue " +
                                "FROM Path " +
                                "JOIN File " +
                                "ON Path.PathId = File.PathId " +
                                "JOIN junction_File_Tag " +
                                "ON File.FileId = junction_File_Tag.FileId " +
                                "JOIN Tag " +
                                "ON junction_File_Tag.TagId = Tag.TagId";

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

        public void PopulatePathIds(Dictionary<string, int> pathsToIds)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
            {
                conn.Open();

                string cmdStr = "SELECT PathValue, PathId FROM Path";

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
                                "INSERT OR IGNORE INTO junction_File_Tag (FileId, TagId) VALUES (@fileId, @tagId)";
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
                                "INSERT OR IGNORE INTO File (DeviceId, PathId) VALUES (@deviceId, @pathId)";
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

                string cmdStr = "SELECT FileId, DeviceId, PathId FROM File";

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
                
                string cmdStr = "SELECT TagValue, TagId FROM Tag";

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
        public void RefreshPathIds(SQLiteConnection conn)
        {
            string cmdStr = "SELECT PathValue, PathId FROM Path";

            using (var cmd = new SQLiteCommand(cmdStr, conn))
            {
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        pathIds.Add(rdr.GetString(0), rdr.GetInt32(1));
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
        public void PopulateIds(List<NwdUriProcessEntry> lst, 
                                SQLiteConnection conn)
        {
            RefreshHashIds(conn);

            RefreshPathIds(conn);

            foreach (var pe in lst)
            {
                EnsureAndPopulateDeviceId(pe);

                if (!string.IsNullOrWhiteSpace(pe.Path) &&
                    pathIds.ContainsKey(pe.Path))
                {
                    pe.PathId = pathIds[pe.Path];
                }

                if (!string.IsNullOrWhiteSpace(pe.Hash) &&
                    hashIds.ContainsKey(pe.Hash))
                {
                    pe.HashId = hashIds[pe.Hash];
                }
            }
        }
        
        [Obsolete("use PopulateIds(List<NwdUriProcessEntry, SQLiteConnection)")]
        public void PopulateIds(List<NwdUriProcessEntry> lst)
        {
            Dictionary<string, int> hashes =
                new Dictionary<string, int>();
            
            Dictionary<string, int> paths =
                new Dictionary<string, int>();

            //foreach entry, get id for hash and path from db
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
            {
                conn.Open();

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                string cmdStr = "SELECT HashValue, HashId FROM Hash";

                using (var cmd = new SQLiteCommand(cmdStr, conn))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            hashes.Add(rdr.GetString(0), rdr.GetInt32(1));
                        }
                    }
                }

                Display.Message(stopwatch.Elapsed.TotalSeconds +
                    " seconds for hash ids.");

                stopwatch = new Stopwatch();
                stopwatch.Start();

                cmdStr = "SELECT PathValue, PathId FROM Path";

                using (var cmd = new SQLiteCommand(cmdStr, conn))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            paths.Add(rdr.GetString(0), rdr.GetInt32(1));
                        }
                    }
                }

                Display.Message(stopwatch.Elapsed.TotalSeconds +
                    " seconds for path ids.");
                
                conn.Close();
            }

            foreach (var pe in lst)
            {
                EnsureAndPopulateDeviceId(pe);
                
                if (!string.IsNullOrWhiteSpace(pe.Path) &&
                    paths.ContainsKey(pe.Path))
                {
                    pe.PathId = paths[pe.Path];
                }

                if (!string.IsNullOrWhiteSpace(pe.Hash) &&
                    hashes.ContainsKey(pe.Hash))
                {
                    pe.HashId = hashes[pe.Hash];
                }
            }
        }

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
        private void RefreshDeviceIds(SQLiteConnection conn)
        {            
            string cmdStr = "SELECT DeviceDescription, " +
                                    "DeviceFriendlyName, " +
                                    "DeviceModel, " +
                                    "DeviceType, " +
                                    "DeviceId " +
                            "FROM Device";

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

                        deviceIds.Add(new NwdDeviceKey()
                        {
                            Description = desc,
                            FriendlyName = friendlyName,
                            Model = model,
                            DeviceType = deviceType
                        }, rdr.GetInt32(4));
                    }
                }
            }                
        }

        private void RefreshDeviceIds()
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
            {
                conn.Open();

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                string cmdStr = "SELECT DeviceDescription, " +
                                       "DeviceFriendlyName, " +
                                       "DeviceModel, " +
                                       "DeviceType, " +
                                       "DeviceId " +
                                "FROM Device";

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

        private void EnsureAndPopulateDeviceId(NwdUriProcessEntry pe)
        {
            var key = ToDeviceKey(pe.PortableDevice);

            if (!deviceIds.ContainsKey(key))
            {
                StoreDevice(pe.PortableDevice);
                RefreshDeviceIds();
            }

            pe.DeviceId = deviceIds[key];
        }
        
        public void StoreHashPathJunctions(List<NwdUriProcessEntry> lst)
        {
            //if both hash and path ids are populated, INSERT OR IGNORE each
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
            {
                conn.Open();

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (var pe in lst)
                        {
                            if (!string.IsNullOrWhiteSpace(pe.Hash) &&
                                !string.IsNullOrWhiteSpace(pe.Path))
                            {
                                cmd.CommandText =
                                    "INSERT OR IGNORE INTO File (DeviceId, PathId, HashId) VALUES (@deviceId, @pathId, @hashId)";
                                cmd.Parameters.AddWithValue("@deviceId", pe.DeviceId);
                                cmd.Parameters.AddWithValue("@pathId", pe.PathId);
                                cmd.Parameters.AddWithValue("@hashId", pe.HashId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                }

                Display.Message(stopwatch.Elapsed.TotalSeconds +
                    " seconds with one transaction.");

                conn.Close();
            }
        }

        /// <summary>
        /// executes an INSERT OR IGNORE statement for supplied device info,
        /// intended for use with device nodes not meeting the standard NwdPortableDevice
        /// format (laptops, desktops, remote servers, &c.)
        /// 
        /// Assumes conn opened and closed outside of method for bulk chaining
        /// 
        /// Any null or whitespace values will result in no database changes
        /// </summary>
        /// <param name="device"></param>
        public void StoreDevice(string description,
                                string friendlyName,
                                string model,
                                string deviceType,
                                SQLiteConnection conn)
        {
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
                            "INSERT OR IGNORE INTO Device " +
                                "(DeviceDescription, " +
                                "DeviceFriendlyName, " +
                                "DeviceModel, " +
                                "DeviceType) " +
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
                                "INSERT OR IGNORE INTO Device " +
                                    "(DeviceDescription, " +
                                    "DeviceFriendlyName, " +
                                    "DeviceModel, " +
                                    "DeviceType) " +
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

        /// <summary>
        /// executes an INSERT OR IGNORE statement for supplied device
        /// </summary>
        /// <param name="device"></param>
        public void StoreDevice(NwdPortableDevice device)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath("nwd")))
            {
                conn.Open();

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        if (device != null)
                        {
                            //convert null values to empty strings
                            ConvertNullsToEmptyStrings(device);

                            cmd.CommandText =
                                "INSERT OR IGNORE INTO Device " +
                                    "(DeviceDescription, " +
                                    "DeviceFriendlyName, " +
                                    "DeviceModel, " +
                                    "DeviceType) " +
                                "VALUES (@deviceDescription, " +
                                        "@deviceFriendlyName, " +
                                        "@deviceModel, " +
                                        "@deviceType)";

                            cmd.Parameters.AddWithValue("@deviceDescription", device.Description);
                            cmd.Parameters.AddWithValue("@deviceFriendlyName", device.FriendlyName);
                            cmd.Parameters.AddWithValue("@deviceModel", device.Model);
                            cmd.Parameters.AddWithValue("@deviceType", device.DeviceType);
                            cmd.ExecuteNonQuery();
                        }
                        
                        transaction.Commit();
                    }
                }

                Display.Message(stopwatch.Elapsed.TotalSeconds +
                    " seconds to insert one device.");

                conn.Close();
            }
        }

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
    }
}
