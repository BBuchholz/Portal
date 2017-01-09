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
    public class MediaV5SubsetDb
    {
        protected string DbName { get; private set; }
        public int LocalDeviceId { get; private set; }

        public MediaV5SubsetDb()
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
            cmd.CommandText = INSERT_MEDIA_PATH_X;
            
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

        private static string INSERT_MEDIA_PATH_X =
            
            "INSERT OR IGNORE INTO " + NwdContract.TABLE_MEDIA_PATH + " " + 
            "	(" + NwdContract.COLUMN_MEDIA_PATH_VALUE + ") " + 
            "VALUES " + 
            "	(?); ";

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
