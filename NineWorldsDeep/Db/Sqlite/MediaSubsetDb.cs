using NineWorldsDeep.Core;
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
    public class MediaSubsetDb
    {
        protected string DbName { get; private set; }
        public int LocalDeviceId { get; private set; }

        public MediaSubsetDb()
        {
            DbName = "nwd";
            LocalDeviceId = GetLocalMediaDeviceId();
        }

        #region "public methods"

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

        #endregion

        #region "private methods"

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

        #endregion
    }
}
