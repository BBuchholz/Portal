using NineWorldsDeep.Archivist;
using NineWorldsDeep.Core;
using NineWorldsDeep.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Db.Sqlite
{
    public class ArchivistSubsetDb
    {
        protected string DbName { get; private set; }

        public ArchivistSubsetDb()
        {
            DbName = "nwd";
        }

        public List<ArchivistSourceType> GetAllSourceTypes()
        {
            List<ArchivistSourceType> lst =
                new List<ArchivistSourceType>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        lst = SelectSourceTypes(cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }

        private List<ArchivistSourceType> SelectSourceTypes(SQLiteCommand cmd)
        {
            List<ArchivistSourceType> lst =
                new List<ArchivistSourceType>();

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_TYPE_ID_TYPE_VALUE_FROM_SOURCE_TYPE;

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    int id = rdr.GetInt32(0);
                    string typeValue = rdr.GetString(1);

                    lst.Add(new ArchivistSourceType()
                    {
                        SourceTypeId = id,
                        SourceTypeValue = typeValue
                    });
                }
            }

            return lst;
        }

        /// <summary>
        /// will just retrieve the core properties of all sources
        /// (SourceId, SourceTypeId, Title, Author, Director, Year,
        /// Url, and RetrievalDate)
        /// </summary>
        /// <returns></returns>
        internal List<ArchivistSource> GetAllSourceCores()
        {
            List<ArchivistSource> lst =
                new List<ArchivistSource>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        lst = SelectSourceCores(cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }

        private List<ArchivistSource> SelectSourceCores(SQLiteCommand cmd)
        {
            List<ArchivistSource> lst =
                new List<ArchivistSource>();

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_SOURCES;

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    //query field order
                    //0:SourceId
                    //1:SourceTypeId
                    //2:SourceTitle
                    //3:SourceAuthor
                    //4:SourceDirector
                    //5:SourceYear
                    //6:SourceUrl
                    //7:SourceRetrievalDate
                    
                    int sourceId = rdr.GetInt32(0);
                    int sourceTypeId = rdr.GetInt32(1);
                    string sourceTitle = DbV5Utils.GetNullableString(rdr, 2);
                    string sourceAuthor = DbV5Utils.GetNullableString(rdr, 3);
                    string sourceDirector = DbV5Utils.GetNullableString(rdr, 4);
                    string sourceYear = DbV5Utils.GetNullableString(rdr, 5);
                    string sourceUrl = DbV5Utils.GetNullableString(rdr, 6);
                    string sourceRetrievalDate = DbV5Utils.GetNullableString(rdr, 7);

                    lst.Add(new ArchivistSource()
                    {
                        SourceId = sourceId,
                        SourceTypeId = sourceTypeId,
                        Title = sourceTitle,
                        Author = sourceAuthor,
                        Director = sourceDirector,
                        Year = sourceYear,
                        Url = sourceUrl,
                        RetrievalDate = sourceRetrievalDate
                    });
                }
            }

            return lst;
        }

        public void EnsureSourceType(string typeName)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        EnsureSourceType(typeName, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        /// <summary>
        /// ensures that the type exists in the db, ignoring if it already exists. 
        /// returns the SourceTypeId for the type, whether found or new.
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private int EnsureSourceType(string typeName, SQLiteCommand cmd)
        {
            int typeId = GetSourceTypeId(typeName, cmd);

            if(typeId < 1)
            {
                InsertOrIgnoreSourceType(typeName, cmd);
                typeId = GetSourceTypeId(typeName, cmd);
            }

            return typeId;
        }

        private void InsertOrIgnoreSourceType(string typeName, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.INSERT_OR_IGNORE_SOURCE_TYPE_VALUE;

            cmd.Parameters.Add(new SQLiteParameter() { Value = typeName });

            cmd.ExecuteNonQuery();
        }

        private int GetSourceTypeId(string typeName, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_SOURCE_TYPE_ID_FOR_VALUE_X;
            
            cmd.Parameters.Add(new SQLiteParameter() { Value = typeName });

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        /// <summary>
        /// As opposed to Sync(ArchivistSource), this will only sync
        /// properties specific to the Source table (SourceTypeId, SourceId, 
        /// Title, Author, Director, Year, Url, and RetrievalDate). The 
        /// rest of the object's members will be left as is, neither pushed to
        /// the db nor pulled from the db.
        /// 
        /// Lookup will be by SourceTypeId, Title, Year, and Url
        /// </summary>
        /// <param name="src"></param>
        internal void SyncCore(ArchivistSource src)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        SyncCore(src, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }
        
        /// <summary>
        /// As opposed to Sync(), this will only sync
        /// properties specific to the Source table (SourceTypeId, SourceId, 
        /// Title, Author, Director, Year, Url, and RetrievalDate). The 
        /// rest of the object's members will be left as is, neither pushed to
        /// the db nor pulled from the db.
        /// 
        /// Lookup will be by SourceTypeId, Title, Year, and Url
        /// </summary>
        /// <param name="src"></param>
        private void SyncCore(ArchivistSource src, SQLiteCommand cmd)
        {
            //mimic Sync(Media)
            src.SourceId = EnsureSourceIdByTypeTitleYearAndUrl(src, cmd);
            PopulateSourceByTypeTitleYearAndUrl(src, cmd);
        }


    }
}
