using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Hive;
using System.Data.SQLite;
using NineWorldsDeep.Core;
using NineWorldsDeep.Sqlite;

namespace NineWorldsDeep.Db.Sqlite
{
    class HiveSubsetDb
    {
        protected string DbName { get; private set; }

        public HiveSubsetDb()
        {
            DbName = "nwd";
        }
        
        internal List<HiveRoot> GetDeactivatedRoots()
        {
            List<HiveRoot> lst =
                new List<HiveRoot>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        lst = GetDeactivatedRoots(cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }

        private List<HiveRoot> GetDeactivatedRoots(SQLiteCommand cmd)
        {
            List<HiveRoot> lst =
                new List<HiveRoot>();

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_DEACTIVATED_HIVE_ROOTS;

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    int hiveRootId = rdr.GetInt32(0);
                    string hiveRootName = rdr.GetString(1);

                    string activatedAtString = 
                        DbV5Utils.GetNullableString(rdr, 2);

                    string deactivatedAtString = DbV5Utils.
                        GetNullableString(rdr, 3);

                    DateTime? activatedAt =
                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(activatedAtString);

                    DateTime? deactivatedAt =
                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(deactivatedAtString);

                    var hr = new HiveRoot()
                    {
                        HiveRootId = hiveRootId,
                        HiveRootName = hiveRootName
                    };

                    hr.SetTimeStamps(activatedAt, deactivatedAt);

                    lst.Add(hr);
                }
            }

            return lst;
        }

        internal List<HiveRoot> GetActiveRoots()
        {
            List<HiveRoot> lst =
                new List<HiveRoot>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        lst = GetActiveRoots(cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }

        private List<HiveRoot> GetActiveRoots(SQLiteCommand cmd)
        {
            List<HiveRoot> lst =
                new List<HiveRoot>();

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_ACTIVE_HIVE_ROOTS;

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    int hiveRootId = rdr.GetInt32(0);
                    string hiveRootName = rdr.GetString(1);

                    string activatedAtString =
                        DbV5Utils.GetNullableString(rdr, 2);

                    string deactivatedAtString = DbV5Utils.
                        GetNullableString(rdr, 3);

                    DateTime? activatedAt =
                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(activatedAtString);

                    DateTime? deactivatedAt =
                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(deactivatedAtString);

                    var hr = new HiveRoot()
                    {
                        HiveRootId = hiveRootId,
                        HiveRootName = hiveRootName
                    };

                    hr.SetTimeStamps(activatedAt, deactivatedAt);

                    lst.Add(hr);
                }
            }

            return lst;
        }

        internal int EnsureHiveRoot(string hiveRootName)
        {
            int hiveRootId = -1;

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        hiveRootId = EnsureHiveRoot(hiveRootName, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return hiveRootId;
        }

        private int EnsureHiveRoot(string hiveRootName, SQLiteCommand cmd)
        {            
            int hiveRootId = GetHiveRootId(hiveRootName, cmd);

            if (hiveRootId < 1)
            {
                InsertOrIgnoreHiveRootName(hiveRootName, cmd);
                hiveRootId = GetHiveRootId(hiveRootName, cmd);
            }

            return hiveRootId;
        }

        private void InsertOrIgnoreHiveRootName(string hiveRootName, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.INSERT_HIVE_ROOT_NAME_X;

            cmd.Parameters.Add(new SQLiteParameter() { Value = hiveRootName });

            cmd.ExecuteNonQuery();
        }

        private int GetHiveRootId(string hiveRootName, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_HIVE_ROOT_ID_FOR_NAME_X;
            
            cmd.Parameters.Add(new SQLiteParameter() { Value = hiveRootName });

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }
    }
}
