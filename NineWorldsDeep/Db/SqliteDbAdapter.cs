using NineWorldsDeep.Core;
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
                    " seconds with one transaction.");

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
                    " seconds with one transaction.");

                conn.Close();
            }

            foreach (var pe in lst)
            {
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
                                    "INSERT OR IGNORE INTO File (DeviceId, PathId, HashId) VALUES (1, @pathId, @hashId)";
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
    }
}
