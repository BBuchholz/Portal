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
        private Dictionary<NwdPortableDeviceKey, int> deviceIds =
            new Dictionary<NwdPortableDeviceKey, int>();

        public SqliteDbAdapter()
        {
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

                            deviceIds.Add(new NwdPortableDeviceKey() {
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

        private NwdPortableDeviceKey ToDeviceKey(NwdPortableDevice device)
        {
            if(device == null)
            {
                throw new Exception("NwdPortableDevice null in method ToDeviceKey()");
            }
        
            return new NwdPortableDeviceKey()
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
