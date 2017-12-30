using NineWorldsDeep.Archivist;
using NineWorldsDeep.Core;
using NineWorldsDeep.Mnemosyne.V5;
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
        private MediaV5SubsetDb mediaDb;

        protected string DbName { get; private set; }

        public ArchivistSubsetDb()
        {
            DbName = "nwd";
            mediaDb = new MediaV5SubsetDb();
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

        public void LoadSourceExcerptsWithTags(ArchivistSource source)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {

                        LoadSourceExcerptsWithTags(source, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        private void LoadSourceExcerptsWithTags(ArchivistSource source, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_EXCERPTS_WITH_TAGGED_TAGS_FOR_SOURCE_ID_X;
            
            cmd.Parameters.Add(new SQLiteParameter() { Value = source.SourceId });

            using (var rdr = cmd.ExecuteReader())
            {
                Dictionary<int, ArchivistSourceExcerpt> idToExcerpt =
                    new Dictionary<int, ArchivistSourceExcerpt>();

                while (rdr.Read())
                {
                    int taggingId = DbV5Utils.GetNullableInt32(rdr, 0);
                    int excerptId = rdr.GetInt32(1);
                    int sourceId = rdr.GetInt32(2);
                    string exVal = rdr.GetString(3);
                    int mediaTagId = DbV5Utils.GetNullableInt32(rdr, 4);
                    string tagValue = DbV5Utils.GetNullableString(rdr, 5);

                    if (!idToExcerpt.ContainsKey(excerptId))
                    {
                        idToExcerpt[excerptId] = new ArchivistSourceExcerpt()
                        {
                            SourceExcerptId = excerptId,
                            SourceId = sourceId,
                            ExcerptValue = exVal
                        };
                        
                        source.Add(idToExcerpt[excerptId]);
                    }

                    var ase = idToExcerpt[excerptId];

                    if (taggingId > 0 &&
                        mediaTagId > 0 &&
                        !string.IsNullOrWhiteSpace(tagValue))
                    {
                        var mt = new MediaTag()
                        {
                            MediaTagId = mediaTagId,
                            MediaTagValue = tagValue
                        };

                        var tagging = new SourceExcerptTagging()
                        {
                            SourceExcerptTaggingId = taggingId,
                            Excerpt = ase,
                            MediaTag = mt
                        };

                        mt.Add(ase);
                        ase.Add(tagging);
                    }
                }
            }
        }

        public void LoadSourceExcerptsWithTags(MediaTag tag)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {

                        LoadSourceExcerptsWithTags(tag, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        private void LoadSourceExcerptsWithTags(MediaTag tag, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_EXCERPTS_WITH_SOURCE_FOR_TAG_ID_X;

            cmd.Parameters.Add(new SQLiteParameter() { Value = tag.MediaTagId });

            using (var rdr = cmd.ExecuteReader())
            {
                Dictionary<int, ArchivistSourceExcerpt> idToExcerpt =
                    new Dictionary<int, ArchivistSourceExcerpt>();

                while (rdr.Read())
                {                    
                    int taggingId = DbV5Utils.GetNullableInt32(rdr, 0);
                    int excerptId = rdr.GetInt32(1);
                    string exVal = rdr.GetString(2);
                    int mediaTagId = DbV5Utils.GetNullableInt32(rdr, 3);
                    string tagValue = DbV5Utils.GetNullableString(rdr, 4);
                    int sourceId = rdr.GetInt32(5);
                    string sourceAuthor = DbV5Utils.GetNullableString(rdr, 6);
                    string sourceTitle = DbV5Utils.GetNullableString(rdr, 7);
                    string sourceUrl = DbV5Utils.GetNullableString(rdr, 8);
                    string sourceTypeValue = rdr.GetString(9);

                    if (!idToExcerpt.ContainsKey(excerptId))
                    {
                        idToExcerpt[excerptId] = new ArchivistSourceExcerpt()
                        {
                            SourceExcerptId = excerptId,
                            SourceId = sourceId,
                            ExcerptValue = exVal
                        };

                        tag.Add(idToExcerpt[excerptId]);
                    }
                    
                    var ase = idToExcerpt[excerptId];

                    if (taggingId > 0 &&
                        mediaTagId > 0 &&
                        !string.IsNullOrWhiteSpace(tagValue))
                    {
                        var mt = new MediaTag()
                        {
                            MediaTagId = mediaTagId,
                            MediaTagValue = tagValue
                        };

                        var tagging = new SourceExcerptTagging()
                        {
                            SourceExcerptTaggingId = taggingId,
                            Excerpt = ase,
                            MediaTag = mt
                        };

                        mt.Add(ase);
                        ase.Add(tagging);
                    }
                }
            }
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
        internal List<ArchivistSource> GetSourceCoresForSourceTypeId(int sourceTypeId)
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
                        lst = SelectSourceCoresForSourceTypeId(sourceTypeId, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }

        public void SaveExcerptTaggings(ArchivistSourceExcerpt ase)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        SaveExcerptTaggings(ase, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        public void SaveExcerptTaggings(IEnumerable<ArchivistSourceExcerpt> archivistSourceExcerpts)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (ArchivistSourceExcerpt ase in archivistSourceExcerpts)
                        {
                            SaveExcerptTaggings(ase, cmd);
                        }

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        private void SaveExcerptTaggings(ArchivistSourceExcerpt ase, SQLiteCommand cmd)
        {
            foreach(SourceExcerptTagging set in ase.ExcerptTaggings)
            {
                SaveExcerptTagging(set, cmd);
            }
        }

        private void SaveExcerptTagging(SourceExcerptTagging set, SQLiteCommand cmd)
        {
            if (set.MediaTag.MediaTagId < 1)
            {                
                set.MediaTag.MediaTagId = mediaDb.EnsureMediaTag(set.MediaTag.MediaTagValue, cmd);
            }

            set.SourceExcerptTaggingId = GetExcerptTaggingId(set, cmd);
            
            if (set.SourceExcerptTaggingId < 1)
            {
                //no need to update, just insert it fresh
                InsertOrIgnoreExcerptTagging(set, cmd); //mimic MediaV5SubsetDb.InsertMediaTagging(...)
            }
            else
            {
                UpdateExcerptTaggingTimestampsById(set, cmd);
            }
                        
        }

        private void UpdateExcerptTaggingTimestampsById(SourceExcerptTagging set, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.UPDATE_SOURCE_EXCERPT_TAGGING_TIMESTAMPS_X_Y_Z;

            //// TAGGED PARAM ///////////////////////////////////////////////////
            SQLiteParameter taggedParam = new SQLiteParameter();

            String taggedAt = TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(set.TaggedAt);

            if (string.IsNullOrWhiteSpace(taggedAt))
            {
                taggedParam.Value = DBNull.Value;
            }
            else
            {
                taggedParam.Value = taggedAt;
            }
            cmd.Parameters.Add(taggedParam);

            //// UNTAGGED PARAM ////////////////////////////////////////////////
            SQLiteParameter untaggedParam = new SQLiteParameter();

            String untaggedAt =
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(set.UntaggedAt);

            if (string.IsNullOrWhiteSpace(untaggedAt))
            {
                untaggedParam.Value = DBNull.Value;
            }
            else
            {
                untaggedParam.Value = untaggedAt;
            }
            cmd.Parameters.Add(untaggedParam);

            //// ID PARAM /////////////////////////////////////////////////////
            cmd.Parameters.Add(new SQLiteParameter()
            {
                Value = set.SourceExcerptTaggingId
            });

            cmd.ExecuteNonQuery();
        }

        private void InsertOrIgnoreExcerptTagging(SourceExcerptTagging set, SQLiteCommand cmd)
        {
            int sourceExcerptId = set.Excerpt.SourceExcerptId;
            int mediaTagId = set.MediaTag.MediaTagId;

            if(sourceExcerptId < 1)
            {
                throw new Exception("sourceExcerptId not set");
            }

            if (mediaTagId < 1)
            {
                throw new Exception("mediaTagId not set");
            }

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.INSERT_OR_IGNORE_EXCERPT_TAGGING_X_Y;
            
            cmd.Parameters.Add(
                new SQLiteParameter() { Value = sourceExcerptId });
            
            cmd.Parameters.Add(
                new SQLiteParameter() { Value = mediaTagId });

            cmd.ExecuteNonQuery();

            //update timestamps (no need to replicate code)
            UpdateExcerptTaggingTimestampsById(set, cmd);
        }

        private int GetExcerptTaggingId(SourceExcerptTagging set, SQLiteCommand cmd)
        {
            int sourceExcerptId = set.Excerpt.SourceExcerptId;
            int mediaTagId = set.MediaTag.MediaTagId;

            if(sourceExcerptId < 1)
            {
                throw new Exception("undefined source excerpt id");
            }

            if (mediaTagId < 1)
            {
                throw new Exception("undefined media tag id");
            }

            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText = 
                NwdContract.SELECT_SOURCE_EXCERPT_TAGGING_ID_X_Y;  //query is done in NwdSql, needs conversion to NwdContract format

            cmd.Parameters.Add(
                new SQLiteParameter() { Value = sourceExcerptId });

            cmd.Parameters.Add(
                new SQLiteParameter() { Value = mediaTagId });

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        public int EnsureCore(ArchivistSourceExcerpt excerpt)
        {
            int excerptId = -1;

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        excerptId = EnsureCore(excerpt, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return excerptId;
        }

        public int EnsureCore(ArchivistSourceExcerpt excerpt, SQLiteCommand cmd)
        {
            int srcId = excerpt.SourceId;
            string exVal = excerpt.ExcerptValue;

            int excerptId = 
                GetExcerptId(srcId, exVal, cmd);

            if(excerptId < 1)
            {
                InsertOrIgnoreExcerpt(srcId, exVal, cmd);
                excerptId = GetExcerptId(srcId, exVal, cmd);
            }

            return excerptId;
        }

        private void InsertOrIgnoreExcerpt(int sourceId, 
            string excerptValue, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.INSERT_OR_IGNORE_SOURCE_EXCERPT_X_Y;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceId });
            cmd.Parameters.Add(new SQLiteParameter() { Value = excerptValue });

            cmd.ExecuteNonQuery();
        }

        private int GetExcerptId(int sourceId, 
            string excerptValue, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_SOURCE_EXCERPT_ID_X_Y;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceId });
            cmd.Parameters.Add(new SQLiteParameter() { Value = excerptValue });

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        private List<ArchivistSource> SelectSourceCoresForSourceTypeId(
            int sourceTypeId, SQLiteCommand cmd)
        {
            List<ArchivistSource> lst =
                new List<ArchivistSource>();

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_SOURCES_BY_TYPE_ID_X;

            cmd.Parameters.Add(ToParm(sourceTypeId));

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
                    //8:SourceTag
                    
                    int sourceId = rdr.GetInt32(0);
                    //int sourceTypeId = rdr.GetInt32(1);
                    string sourceTitle = DbV5Utils.GetNullableString(rdr, 2);
                    string sourceAuthor = DbV5Utils.GetNullableString(rdr, 3);
                    string sourceDirector = DbV5Utils.GetNullableString(rdr, 4);
                    string sourceYear = DbV5Utils.GetNullableString(rdr, 5);
                    string sourceUrl = DbV5Utils.GetNullableString(rdr, 6);
                    string sourceRetrievalDate = DbV5Utils.GetNullableString(rdr, 7);
                    string sourceTag = DbV5Utils.GetNullableString(rdr, 8);

                    lst.Add(new ArchivistSource()
                    {
                        SourceId = sourceId,
                        SourceTypeId = sourceTypeId,
                        Title = sourceTitle,
                        Author = sourceAuthor,
                        Director = sourceDirector,
                        Year = sourceYear,
                        Url = sourceUrl,
                        RetrievalDate = sourceRetrievalDate,
                        SourceTag = sourceTag
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
        /// tries to set source tag, will fail if unique constraint is violated
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="sourceTag"></param>
        /// <param name="cmd"></param>
        /// <returns>true if successful, false if fails for any reason</returns>
        internal bool SetSourceTag(int sourceId, string sourceTag)
        {
            bool success = false;

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        success = SetSourceTag(sourceId, sourceTag, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();

            }

            return success;
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

        private void PopulateSourceByTypeTitleYearAndUrl(ArchivistSource src, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            // W = SourceTypeId
            // X = SourceTitle
            // Y = SourceYear
            // Z = SourceUrl
            cmd.CommandText = NwdContract.SELECT_SOURCE_W_X_Y_Z;

            cmd.Parameters.Add(ToParm(src.SourceTypeId));
            cmd.Parameters.Add(ToNullableParm(src.Title));
            cmd.Parameters.Add(ToNullableParm(src.Year));
            cmd.Parameters.Add(ToNullableParm(src.Url));

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    // 0:SourceId
                    // 1:SourceTypeId
                    // 2:SourceTitle
                    // 3:SourceAuthor
                    // 4:SourceDirector
                    // 5:SourceYear
                    // 6:SourceUrl
                    // 7:SourceRetrievalDate 
                    // 8:SourceTag

                    int sourceId = rdr.GetInt32(0);
                    int sourceTypeId = rdr.GetInt32(1);
                    string sourceTitle = DbV5Utils.GetNullableString(rdr, 2);
                    string sourceAuthor = DbV5Utils.GetNullableString(rdr, 3);
                    string sourceDirector = DbV5Utils.GetNullableString(rdr, 4);
                    string sourceYear = DbV5Utils.GetNullableString(rdr, 5);
                    string sourceUrl = DbV5Utils.GetNullableString(rdr, 6);
                    string sourceRetrievalDate = DbV5Utils.GetNullableString(rdr, 7);
                    string sourceTag = DbV5Utils.GetNullableString(rdr, 8);

                    src.SourceId = sourceId;
                    src.SourceTypeId = sourceTypeId;
                    src.Title = sourceTitle;
                    src.Author = sourceAuthor;
                    src.Director = sourceDirector;
                    src.Year = sourceYear;
                    src.Url = sourceUrl;
                    src.RetrievalDate = sourceRetrievalDate;
                    src.SourceTag = sourceTag;
                }
            }
        }

        private int EnsureSourceIdByTypeTitleYearAndUrl(ArchivistSource src, SQLiteCommand cmd)
        {
            int sourceId = GetSourceIdByTypeTitleYearAndUrl(src, cmd);

            if(sourceId < 1)
            {
                InsertOrIgnoreTypeTitleYearAndUrlForSource(src, cmd);
                sourceId = GetSourceIdByTypeTitleYearAndUrl(src, cmd);
            }

            return sourceId;
        }

        private void InsertOrIgnoreTypeTitleYearAndUrlForSource(ArchivistSource src, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            // T = SourceTypeId
            // U = SourceTitle
            // V = SourceAuthor
            // W = SourceDirector
            // X = SourceYear
            // Y = SourceUrl
            // Z = SourceRetrievalDate
            cmd.CommandText = NwdContract.INSERT_SOURCE_T_U_V_W_X_Y_Z;

            cmd.Parameters.Add(ToParm(src.SourceTypeId));
            cmd.Parameters.Add(ToNullableParm(src.Title));
            cmd.Parameters.Add(ToNullableParm(src.Author));
            cmd.Parameters.Add(ToNullableParm(src.Director));
            cmd.Parameters.Add(ToNullableParm(src.Year));
            cmd.Parameters.Add(ToNullableParm(src.Url));
            cmd.Parameters.Add(ToNullableParm(src.RetrievalDate));

            cmd.ExecuteNonQuery();
            
        }

        /// <summary>
        /// tries to set source tag, will fail if unique constraint is violated
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="sourceTag"></param>
        /// <param name="cmd"></param>
        /// <returns>true if successful, false if fails for any reason</returns>
        private bool SetSourceTag(int sourceId, string sourceTag, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            
            cmd.CommandText = NwdContract.UPDATE_SOURCE_TAG_WHERE_SOURCE_ID;

            cmd.Parameters.Add(ToNullableParm(sourceTag));
            cmd.Parameters.Add(ToParm(sourceId));

            //in case unique constraint fails

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// just stores the value inside a parameter which it returns.
        /// convenience method.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private SQLiteParameter ToParm(int value)
        {
            return new SQLiteParameter()
            {
                Value = value
            };
        }

        /// <summary>
        /// null or whitespace values will be converted to DBNull, any
        /// other values will be passed through as is.
        /// convenience method.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private SQLiteParameter ToNullableParm(string value)
        {
            return new SQLiteParameter()
            {
                Value = GetNullableValue(value)
            };
        }

        private int GetSourceIdByTypeTitleYearAndUrl(ArchivistSource src, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();

            // W = SourceTypeId
            // X = SourceTitle
            // Y = SourceYear
            // Z = SourceUrl
            cmd.CommandText = NwdContract.SELECT_SOURCE_W_X_Y_Z;

            cmd.Parameters.Add(ToParm(src.SourceTypeId));
            cmd.Parameters.Add(ToNullableParm(src.Title));
            cmd.Parameters.Add(ToNullableParm(src.Year));
            cmd.Parameters.Add(ToNullableParm(src.Url));

            //cmd.Parameters.Add(new SQLiteParameter()
            //{
            //    Value = src.SourceTypeId
            //});

            //cmd.Parameters.Add(new SQLiteParameter()
            //{
            //    Value = GetNullableValue(src.Title)
            //});

            //cmd.Parameters.Add(new SQLiteParameter()
            //{
            //    Value = GetNullableValue(src.Year)
            //});

            //cmd.Parameters.Add(new SQLiteParameter()
            //{
            //    Value = GetNullableValue(src.Url)
            //});

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    // query returns entire source record, 
                    // we just need the id for this method
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        private object GetNullableValue(string valueToCheck)
        {
            object outputValue;

            if (string.IsNullOrWhiteSpace(valueToCheck))
            {
                outputValue = DBNull.Value;
            }
            else
            {
                outputValue = valueToCheck;
            }

            return outputValue;
        }
    }
}
