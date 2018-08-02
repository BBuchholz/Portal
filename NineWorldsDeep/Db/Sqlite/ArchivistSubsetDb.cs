using NineWorldsDeep.Archivist;
using NineWorldsDeep.Core;
using NineWorldsDeep.Mnemosyne.V5;
using NineWorldsDeep.Sqlite;
using NineWorldsDeep.Tapestry.NodeUI;
using NineWorldsDeep.Xml.Archivist;
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
        #region Fields

        private MediaV5SubsetDb mediaDb;

        protected string DbName { get; private set; }

        #endregion //Fields

        #region Creation

        public ArchivistSubsetDb()
        {
            DbName = "nwd";
            mediaDb = new MediaV5SubsetDb();
        }

        #endregion //Creation

        #region ArchivistSourceType

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

            if (typeId < 1)
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

        #endregion

        #region ArchivistXmlSource

        internal void SaveAsync(IEnumerable<ArchivistXmlSource> xmlSources,
                                IAsyncStatusResponsive ui,
                                string asyncStatusDetailPrefix = "")
        {
            string detail;
            int total = xmlSources.Count();
            int count = 0;

            using (var conn = new SQLiteConnection(
               @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (ArchivistXmlSource source in xmlSources)
                        {
                            count++;

                            detail = asyncStatusDetailPrefix +
                                "saving source " + count + " of " +
                                total + ", type is " + source.SourceType;

                            ui.StatusDetailUpdate(detail);

                            Save(source, cmd);
                        }

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        internal void Save(ArchivistXmlSource source, SQLiteCommand cmd)
        {
            int sourceTypeId = EnsureSourceType(source.SourceType, cmd);

            int sourceId = EnsureSource(sourceTypeId, source, cmd); // need to write this

            foreach(var slse in source.LocationEntries)
            {
                //ensure location
                int locationId = EnsureSourceLocation(slse.Location, cmd);

                //ensure subset
                int subsetId = EnsureSourceLocationSubset(locationId, slse.LocationSubset, cmd);

                //ensure entry
                int entryId = EnsureSourceLocationSubsetEntry(sourceId, subsetId, slse.LocationSubsetEntry, cmd);

                DateTime? verifiedPresent = TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(slse.VerifiedPresent);
                DateTime? verifiedMissing = TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(slse.VerifiedMissing);
                
                UpdateSourceLocationSubsetEntryTimeStamps(entryId, verifiedPresent, verifiedMissing, cmd);
            }

            foreach(var se in source.Excerpts)
            {
                //fill in values, already written
                int excerptId = EnsureSourceExcerpt(sourceId,
                                                    se.ExcerptValue,
                                                    se.BeginTime,
                                                    se.EndTime,
                                                    se.Pages,
                                                    cmd);

                foreach(var sea in se.Annotations)
                {
                    int annotationId = EnsureSourceAnnotation(sea.AnnotationValue, cmd);

                    InsertOrIgnoreSourceExcerptAnnotationRecord(excerptId, annotationId, cmd);
                }

                foreach(var tag in se.Tags)
                {
                    int tagId = mediaDb.EnsureMediaTag(tag.TagValue, cmd);

                    UpsertExcerptTagging(excerptId, tagId, tag.TaggedAt, tag.UntaggedAt, cmd);
                }
            }            
        }


        private int EnsureSource(int sourceTypeId, ArchivistXmlSource source, SQLiteCommand cmd)
        {
            int sourceId = GetSourceId(sourceTypeId, source, cmd);

            if(sourceId < 1)
            {
                InsertOrIgnoreSource(sourceTypeId, source, cmd);
                sourceId = GetSourceId(sourceTypeId, source, cmd);
            }

            return sourceId;
        }

        private void InsertOrIgnoreSource(int sourceTypeId, ArchivistXmlSource source, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.INSERT_SOURCE_TID_TTL_AUT_DIR_YR_URL_RDT_TG;

            cmd.Parameters.Add(ToParm(sourceTypeId));
            cmd.Parameters.Add(ToNullableParm(source.Title));
            cmd.Parameters.Add(ToNullableParm(source.Author));
            cmd.Parameters.Add(ToNullableParm(source.Director));
            cmd.Parameters.Add(ToNullableParm(source.Year));
            cmd.Parameters.Add(ToNullableParm(source.Url));
            cmd.Parameters.Add(ToNullableParm(source.RetrievalDate));
            cmd.Parameters.Add(ToNullableParm(source.SourceTag));

            cmd.ExecuteNonQuery();
        }

        private int GetSourceId(int sourceTypeId, ArchivistXmlSource source, SQLiteCommand cmd)
        {            
            int id = -1;

            cmd.Parameters.Clear();
            
            cmd.CommandText = 
                NwdContract.SELECT_SOURCE_FOR_TID_TTL_AUT_DIR_YR_URL_RDT_TG;

            cmd.Parameters.Add(ToParm(sourceTypeId));
            cmd.Parameters.Add(ToNullableParm(source.Title));
            cmd.Parameters.Add(ToNullableParm(source.Author));
            cmd.Parameters.Add(ToNullableParm(source.Director));
            cmd.Parameters.Add(ToNullableParm(source.Year));
            cmd.Parameters.Add(ToNullableParm(source.Url));
            cmd.Parameters.Add(ToNullableParm(source.RetrievalDate));
            cmd.Parameters.Add(ToNullableParm(source.SourceTag));
            
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


        #endregion
        
        #region Utility Methods

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

        #endregion //Utility Methods

        #region ArchivistSourceExcerpt

        public ArchivistSourceExcerpt GetSourceExcerptByIdWithSourceAndAnnotations(int sourceExcerptId)
        {
            ArchivistSourceExcerpt ase = null;

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        ase = GetSourceExcerptByIdWithSourceAndAnnotations(sourceExcerptId, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return ase;
        }

        private ArchivistSourceExcerpt GetSourceExcerptByIdWithSourceAndAnnotations(int sourceExcerptId, SQLiteCommand cmd)
        {
            //get excerpt by id
            ArchivistSourceExcerpt ase = GetSourceExcerptById(sourceExcerptId, cmd);

            if (ase != null)
            {
                //populate source
                if(ase.SourceId > 0)
                {
                    ase.Source = GetSourceById(ase.SourceId, cmd);
                }

                //populate annotations
                foreach(ArchivistSourceExcerptAnnotation annotation in GetSourceExcerptAnnotationsBySourceExcerptId(ase.SourceExcerptId, cmd))
                {
                    ase.Annotations.Add(annotation);
                }
            }

            return ase;
        }
        
        private ArchivistSourceExcerpt GetSourceExcerptById(int sourceExcerptId, SQLiteCommand cmd)
        {
            ArchivistSourceExcerpt ase = null;

            cmd.Parameters.Clear();
            
            cmd.CommandText = NwdContract.SELECT_SOURCE_EXCERPT_BY_ID;

            cmd.Parameters.Add(ToParm(sourceExcerptId));

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    // 0:SourceExcerptId
                    // 1:SourceId
                    // 2:SourceExcerptValue
                    // 3:SourceExcerptPages
                    // 4:SourceExcerptBeginTime
                    // 5:SourceExcerptEndTime 

                    sourceExcerptId = rdr.GetInt32(0);
                    int sourceId = rdr.GetInt32(1);
                    string sourceExcerptValue = DbV5Utils.GetNullableString(rdr, 2);
                    string sourceExcerptPages = DbV5Utils.GetNullableString(rdr, 3);
                    string sourceExcerptBeginTime = DbV5Utils.GetNullableString(rdr, 4);
                    string sourceExcerptEndTime = DbV5Utils.GetNullableString(rdr, 5);

                    ase = new ArchivistSourceExcerpt()
                    {
                        SourceExcerptId = sourceExcerptId,
                        SourceId = sourceId,
                        ExcerptValue = sourceExcerptValue,
                        ExcerptPages = sourceExcerptPages,
                        ExcerptBeginTime = sourceExcerptBeginTime,
                        ExcerptEndTime = sourceExcerptEndTime
                    };
                    
                }
            }

            return ase;
        }

        internal List<ArchivistSourceExcerpt> GetSourceExcerptsForSourceId(int sourceId)
        {
            List<ArchivistSourceExcerpt> lst = null;

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        lst = GetSourceExcerptsForSourceId(sourceId, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }

        private List<ArchivistSourceExcerpt> GetSourceExcerptsForSourceId(int sourceId, SQLiteCommand cmd)
        {
            List<ArchivistSourceExcerpt> lst =
                new List<ArchivistSourceExcerpt>();

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_SOURCE_EXCERPTS_FOR_SOURCE_ID_X;

            cmd.Parameters.Add(ToParm(sourceId));

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    // 0:SourceExcerptId
                    // 1:SourceId
                    // 2:SourceExcerptValue
                    // 3:SourceExcerptPages
                    // 4:SourceExcerptBeginTime
                    // 5:SourceExcerptEndTime 

                    int sourceExcerptId = rdr.GetInt32(0);
                    //int sourceId = rdr.GetInt32(1);
                    string sourceExcerptValue = DbV5Utils.GetNullableString(rdr, 2);
                    string sourceExcerptPages = DbV5Utils.GetNullableString(rdr, 3);
                    string sourceExcerptBeginTime = DbV5Utils.GetNullableString(rdr, 4);
                    string sourceExcerptEndTime = DbV5Utils.GetNullableString(rdr, 5);

                    var ase = new ArchivistSourceExcerpt()
                    {
                        SourceExcerptId = sourceExcerptId,
                        SourceId = sourceId,
                        ExcerptValue = sourceExcerptValue,
                        ExcerptPages = sourceExcerptPages,
                        ExcerptBeginTime = sourceExcerptBeginTime,
                        ExcerptEndTime = sourceExcerptEndTime
                    };

                    lst.Add(ase);
                }
            }

            return lst;
        }

        public void LoadSourceExcerptsWithTaggedTags(ArchivistSource source)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {

                        LoadSourceExcerptsWithTaggedTags(source, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }


        private void LoadSourceExcerptsWithTaggedTags(ArchivistSource source, SQLiteCommand cmd)
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

        public void LoadSourceExcerptsWithTagsAndSource(MediaTag tag)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {

                        LoadSourceExcerptsWithTagsAndSource(tag, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        private void LoadSourceExcerptsWithTagsAndSource(MediaTag tag, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_EXCERPTS_WITH_SOURCE_FOR_TAG_ID_X;

            cmd.Parameters.Add(new SQLiteParameter() { Value = tag.MediaTagId });

            using (var rdr = cmd.ExecuteReader())
            {
                Dictionary<int, ArchivistSourceExcerpt> idToExcerpt =
                    new Dictionary<int, ArchivistSourceExcerpt>();

                Dictionary<int, ArchivistSource> idToSource =
                    new Dictionary<int, ArchivistSource>();

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

                    if (!idToSource.ContainsKey(sourceId))
                    {
                        idToSource[sourceId] = new ArchivistSource()
                        {
                            Author = sourceAuthor,
                            Title = sourceTitle,
                            Url = sourceUrl,
                            SourceId = sourceId
                        };
                    }

                    if (!idToExcerpt.ContainsKey(excerptId))
                    {
                        idToExcerpt[excerptId] = new ArchivistSourceExcerpt()
                        {
                            SourceExcerptId = excerptId,
                            SourceId = sourceId,
                            Source = idToSource[sourceId],
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

        internal void DeleteArchivistSourceExcerptsBySourceId(int sourceId)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        DeleteArchivistSourceExcerptsBySourceId(sourceId, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        private void DeleteArchivistSourceExcerptsBySourceId(int sourceId,
                                                             SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.DELETE_ARCHIVIST_SOURCE_EXCERPTS_FOR_SOURCE_ID;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceId });

            cmd.ExecuteNonQuery();
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

        public int EnsureSourceExcerpt(int srcId,
                                       string exVal,
                                       string beginTime,
                                       string endTime,
                                       string pages,
                                       SQLiteCommand cmd)
        {
            int excerptId =
                GetExcerptId(srcId, exVal, beginTime, endTime, pages, cmd);

            if (excerptId < 1)
            {
                InsertOrIgnoreExcerpt(srcId, exVal, beginTime, endTime, pages, cmd);
                excerptId = GetExcerptId(srcId, exVal, beginTime, endTime, pages, cmd);
            }

            return excerptId;
        }

        public int EnsureCore(ArchivistSourceExcerpt excerpt, SQLiteCommand cmd)
        {
            //int srcId = excerpt.SourceId;
            //string exVal = excerpt.ExcerptValue;

            //int excerptId = 
            //    GetExcerptId(srcId, exVal, cmd);

            //if(excerptId < 1)
            //{
            //    InsertOrIgnoreExcerpt(srcId, exVal, cmd);
            //    excerptId = GetExcerptId(srcId, exVal, cmd);
            //}

            //return excerptId;

            int srcId = excerpt.SourceId;
            string exVal = excerpt.ExcerptValue;
            string beginTime = excerpt.ExcerptBeginTime;
            string endTime = excerpt.ExcerptEndTime;
            string pages = excerpt.ExcerptPages;

            int excerptId =
                GetExcerptId(srcId, exVal, beginTime, endTime, pages, cmd);

            if (excerptId < 1)
            {
                InsertOrIgnoreExcerpt(srcId, exVal, beginTime, endTime, pages, cmd);
                excerptId = GetExcerptId(srcId, exVal, beginTime, endTime, pages, cmd);
            }

            return excerptId;
        }

        private void InsertOrIgnoreExcerpt(int sourceId,
                                           string excerptValue,
                                           string beginTime,
                                           string endTime,
                                           string pages,
                                           SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.INSERT_OR_IGNORE_SOURCE_EXCERPT_SRCID_EXVAL_BTIME_ETIME_PGS_V_W_X_Y_Z;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceId });
            cmd.Parameters.Add(new SQLiteParameter() { Value = excerptValue });
            cmd.Parameters.Add(new SQLiteParameter() { Value = beginTime });
            cmd.Parameters.Add(new SQLiteParameter() { Value = endTime });
            cmd.Parameters.Add(new SQLiteParameter() { Value = pages });

            cmd.ExecuteNonQuery();
        }

        private int GetExcerptId(int sourceId,
                                 string excerptValue,
                                 string beginTime,
                                 string endTime,
                                 string pages,
                                 SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_SOURCE_EXCERPT_ID_FOR_SRCID_EXVAL_BTIME_ETIME_PGS_V_W_X_Y_Z;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceId });
            cmd.Parameters.Add(new SQLiteParameter() { Value = excerptValue });
            cmd.Parameters.Add(new SQLiteParameter() { Value = beginTime });
            cmd.Parameters.Add(new SQLiteParameter() { Value = endTime });
            cmd.Parameters.Add(new SQLiteParameter() { Value = pages });

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
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

        #endregion

        #region ArchivistSource


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



        internal void DeleteArchivistSourceBySourceId(int sourceId)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        DeleteArchivistSourceBySourceId(sourceId, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        private void DeleteArchivistSourceBySourceId(int sourceId,
                                                     SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.DELETE_ARCHIVIST_SOURCE_FOR_SOURCE_ID;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceId });

            cmd.ExecuteNonQuery();
        }

        internal List<ArchivistSource> GetAllSources()
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
                        lst = GetAllSources(cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }

        private List<ArchivistSource> GetAllSources(SQLiteCommand cmd)
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
                    //8:SourceTag

                    int sourceId = rdr.GetInt32(0);
                    int sourceTypeId = rdr.GetInt32(1);
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

        public ArchivistSource GetSourceById(int sourceId)
        {
            ArchivistSource src = null;

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        src = GetSourceById(sourceId, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return src;
        }

        private ArchivistSource GetSourceById(int sourceId, SQLiteCommand cmd)
        {
            ArchivistSource src = null;

            cmd.Parameters.Clear();

            // W = SourceTypeId
            // X = SourceTitle
            // Y = SourceYear
            // Z = SourceUrl
            cmd.CommandText = NwdContract.SELECT_SOURCE_BY_ID;

            cmd.Parameters.Add(ToParm(sourceId));

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

                    int sourceTypeId = rdr.GetInt32(1);
                    string sourceTitle = DbV5Utils.GetNullableString(rdr, 2);
                    string sourceAuthor = DbV5Utils.GetNullableString(rdr, 3);
                    string sourceDirector = DbV5Utils.GetNullableString(rdr, 4);
                    string sourceYear = DbV5Utils.GetNullableString(rdr, 5);
                    string sourceUrl = DbV5Utils.GetNullableString(rdr, 6);
                    string sourceRetrievalDate = DbV5Utils.GetNullableString(rdr, 7);
                    string sourceTag = DbV5Utils.GetNullableString(rdr, 8);

                    src = new ArchivistSource()
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
                    };

                    //src.SourceId = sourceId;
                    //src.SourceTypeId = sourceTypeId;
                    //src.Title = sourceTitle;
                    //src.Author = sourceAuthor;
                    //src.Director = sourceDirector;
                    //src.Year = sourceYear;
                    //src.Url = sourceUrl;
                    //src.RetrievalDate = sourceRetrievalDate;
                    //src.SourceTag = sourceTag;
                }
            }

            return src;
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

            if (sourceId < 1)
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

        #endregion

        #region ArchvistSourceExcerptAnnotation

        internal void DeleteArchivistSourceExcerptAnnotationsByExcerptId(int sourceExcerptId)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        DeleteArchivistSourceExcerptAnnotationsByExcerptId(sourceExcerptId, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }        

        private void DeleteArchivistSourceExcerptAnnotationsByExcerptId(int sourceExcerptId,
                                                                        SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.DELETE_ARCHIVIST_SOURCE_EXCERPT_ANNOTATIONS_FOR_EXID;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceExcerptId });

            cmd.ExecuteNonQuery();
        }

        public void InsertSourceExcerptAnnotation(int sourceExcerptId, string annotationValue)
        {            
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        InsertSourceExcerptAnnotation(sourceExcerptId, annotationValue, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        private void InsertSourceExcerptAnnotation(int sourceExcerptId, string annotationValue, SQLiteCommand cmd)
        {
            int annotationId = EnsureSourceAnnotation(annotationValue, cmd);

            InsertOrIgnoreSourceExcerptAnnotationRecord(sourceExcerptId, annotationId, cmd);
        }

        internal List<ArchivistSourceExcerptAnnotation> GetSourceExcerptAnnotationsBySourceExcerptId(int sourceExcerptId)
        {
            List<ArchivistSourceExcerptAnnotation> lst = null;

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        lst = GetSourceExcerptAnnotationsBySourceExcerptId(sourceExcerptId, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }

        private List<ArchivistSourceExcerptAnnotation> GetSourceExcerptAnnotationsBySourceExcerptId(int sourceExcerptId, SQLiteCommand cmd)
        {
            List<ArchivistSourceExcerptAnnotation> lst =
                new List<ArchivistSourceExcerptAnnotation>();

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_SOURCE_EXCERPT_ANNOTATIONS_BY_SOURCE_EXCERPT_ID;

            cmd.Parameters.Add(ToParm(sourceExcerptId));

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    //query field order
                    //0:SourceExcerptAnnotationId
                    //1:SourceExcerptId
                    //2:SourceAnnotationId
                    //3:SourceAnnotationValue 

                    int sourceExcerptAnnotationId = rdr.GetInt32(0);
                    sourceExcerptId = rdr.GetInt32(1);
                    int sourceAnnotationId = rdr.GetInt32(2);
                    string sourceAnnotationValue = DbV5Utils.GetNullableString(rdr, 3);
                    
                    lst.Add(new ArchivistSourceExcerptAnnotation()
                    {
                        SourceExcerptAnnotationId = sourceExcerptAnnotationId,
                        SourceExcerptId = sourceExcerptId,
                        SourceAnnotationId = sourceAnnotationId,
                        SourceAnnotationValue = sourceAnnotationValue
                    });
                }
            }

            return lst;
        }

        private int EnsureSourceAnnotation(string annotationValue, SQLiteCommand cmd)
        {
            int annotationId = GetSourceAnnotationId(annotationValue, cmd);

            if (annotationId < 1)
            {
                InsertOrIgnoreSourceAnnotation(annotationValue, cmd);
                annotationId = GetSourceAnnotationId(annotationValue, cmd);
            }

            return annotationId;
        }

        private int GetSourceAnnotationId(string annotationValue, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_SOURCE_ANNOTATION_ID_FOR_ANNOTATION_VALUE;

            cmd.Parameters.Add(new SQLiteParameter() { Value = annotationValue });

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        private void InsertOrIgnoreSourceAnnotation(string annotationValue, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.INSERT_OR_IGNORE_SOURCE_ANNOTATION_VALUE;

            cmd.Parameters.Add(new SQLiteParameter() { Value = annotationValue });

            cmd.ExecuteNonQuery();
        }

        private void InsertOrIgnoreSourceExcerptAnnotationRecord(int sourceExcerptId, int sourceAnnotationId, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.INSERT_OR_IGNORE_SOURCE_EXCERPT_ANNOTATION_EXCERPT_ID_ANNOTATION_ID_X_Y;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceExcerptId });
            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceAnnotationId });

            cmd.ExecuteNonQuery();
        }

        #endregion

        #region ArchivistSourceLocation

        internal void EnsureSourceLocation(string locationName)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        EnsureSourceLocation(locationName, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        private int EnsureSourceLocation(string locationName, SQLiteCommand cmd)
        {
            int typeId = GetSourceLocationId(locationName, cmd);

            if (typeId < 1)
            {
                InsertOrIgnoreSourceLocation(locationName, cmd);
                typeId = GetSourceLocationId(locationName, cmd);
            }

            return typeId;
        }

        private void InsertOrIgnoreSourceLocation(string locationName, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.INSERT_OR_IGNORE_SOURCE_LOCATION_VALUE;

            cmd.Parameters.Add(new SQLiteParameter() { Value = locationName });

            cmd.ExecuteNonQuery();
        }

        private int GetSourceLocationId(string locationName, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_SOURCE_LOCATION_ID_FOR_VALUE_X;

            cmd.Parameters.Add(new SQLiteParameter() { Value = locationName });

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        internal List<ArchivistSourceLocation> GetAllSourceLocations()
        {
            List<ArchivistSourceLocation> lst =
                new List<ArchivistSourceLocation>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        lst = SelectSourceLocations(cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }



        private List<ArchivistSourceLocation> SelectSourceLocations(SQLiteCommand cmd)
        {
            List<ArchivistSourceLocation> lst =
                new List<ArchivistSourceLocation>();

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_LOCATION_ID_LOCATION_VALUE_FROM_SOURCE_TYPE;
            
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    int id = rdr.GetInt32(0);
                    string typeValue = rdr.GetString(1);

                    lst.Add(new ArchivistSourceLocation()
                    {
                        SourceLocationId = id,
                        SourceLocationValue = typeValue
                    });
                }
            }

            return lst;
        }

        #endregion

        #region ArchivistSourceLocationSubset

        internal List<ArchivistSourceLocationSubset> GetSourceLocationSubsetsForLocationId(int locationId)
        {
            List<ArchivistSourceLocationSubset> lst =
                new List<ArchivistSourceLocationSubset>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        lst = SelectLocationSubsetsForSourceLocationId(locationId, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }

        private List<ArchivistSourceLocationSubset> SelectLocationSubsetsForSourceLocationId(int locationId, SQLiteCommand cmd)
        {
            List<ArchivistSourceLocationSubset> lst =
                new List<ArchivistSourceLocationSubset>();

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_SOURCE_LOCATION_SUBSETS_BY_LOCATION_ID_X;

            cmd.Parameters.Add(new SQLiteParameter() { Value = locationId });

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    //query field order
                    //0:SourceLocationSubsetId
                    //1:SourceLocationId
                    //2:SourceLocationSubsetValue

                    int sourceLocationSubsetId = rdr.GetInt32(0);
                    string sourceLocationSubsetValue = DbV5Utils.GetNullableString(rdr, 2);

                    lst.Add(new ArchivistSourceLocationSubset()
                    {
                        SourceLocationId = locationId,
                        SourceLocationSubsetId = sourceLocationSubsetId,
                        SourceLocationSubsetValue = sourceLocationSubsetValue
                    });
                }
            }

            return lst;
        }
        
        /// <summary>
        /// ensure location subset and returns subset id, whether found or created
        /// </summary>
        /// <param name="sourceLocationId"></param>
        /// <param name="subsetName"></param>
        /// <returns></returns>
        internal int EnsureSourceLocationSubset(int sourceLocationId, string subsetName)
        {
            int id = -1;

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        id = EnsureSourceLocationSubset(sourceLocationId, subsetName, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return id;
        }

        internal int EnsureSourceLocationSubset(int sourceLocationId, string subsetName, SQLiteCommand cmd)
        {
            int sourceLocationSubsetId =
                GetSourceLocationSubsetId(sourceLocationId, subsetName, cmd);

            if (sourceLocationSubsetId < 1)
            {
                InsertOrIgnoreSourceLocationSubset(sourceLocationId, subsetName, cmd);
                sourceLocationSubsetId =
                    GetSourceLocationSubsetId(sourceLocationId, subsetName, cmd);
            }

            return sourceLocationSubsetId;
        }

        private void InsertOrIgnoreSourceLocationSubset(int sourceLocationId, string subsetName, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.INSERT_OR_IGNORE_SOURCE_LOCATION_SUBSET_FOR_LOCATION_ID_AND_SUBSET_VALUE_X_Y;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceLocationId });
            cmd.Parameters.Add(new SQLiteParameter() { Value = subsetName });

            cmd.ExecuteNonQuery();
        }

        private int GetSourceLocationSubsetId(int sourceLocationId, string subsetName, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_SOURCE_LOCATION_SUBSET_ID_FOR_LOCATION_ID_AND_SUBSET_VALUE_X_Y;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceLocationId });
            cmd.Parameters.Add(new SQLiteParameter() { Value = subsetName });

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

        #region ArchivistSourceLocationSubsetEntry

        internal void DeleteArchivistSourceLocationSubsetEntriesBySourceId(int sourceId)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        DeleteArchivistSourceLocationSubsetEntriesBySourceId(sourceId, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        private void DeleteArchivistSourceLocationSubsetEntriesBySourceId(int sourceId,
                                                                          SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.DELETE_ARCHIVIST_SOURCE_LOCATION_SUBSET_ENTRIES_FOR_SOURCE_ID;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceId });

            cmd.ExecuteNonQuery();
        }

        internal List<ArchivistSourceLocationSubsetEntry> GetSourceLocationSubsetEntriesForSourceId(int sourceId, bool filterOutVerifiedMissing)
        {
            List<ArchivistSourceLocationSubsetEntry> lst =
                new List<ArchivistSourceLocationSubsetEntry>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        lst = SelectSourceLocationSubsetEntriesForSourceId(sourceId, filterOutVerifiedMissing, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }

        private List<ArchivistSourceLocationSubsetEntry> SelectSourceLocationSubsetEntriesForSourceId(int sourceId, bool filterOutVerifiedMissing, SQLiteCommand cmd)
        {
            List<ArchivistSourceLocationSubsetEntry> lst =
                new List<ArchivistSourceLocationSubsetEntry>();

            cmd.Parameters.Clear();

            //if (!filterOutVerifiedMissing)
            //{
            //    cmd.CommandText =
            //        NwdContract.SELECT_SOURCE_LOCATION_SUBSET_ENTRIES_FOR_SOURCE_ID_X;
            //}
            //else
            //{
            //    cmd.CommandText =
            //        NwdContract.SELECT_VERIFIED_PRESENT_SOURCE_LOCATION_SUBSET_ENTRIES_FOR_SOURCE_ID_X;
            //}

            cmd.CommandText =
                NwdContract.SELECT_SOURCE_LOCATION_SUBSET_ENTRIES_FOR_SOURCE_ID_X;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceId });

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    //0: sourceLocationSubsetEntryId
                    //1: sourceLocationSubsetId
                    //2: sourceLocationValue
                    //3: sourceLocationSubsetValue
                    //4: sourceLocationSubsetEntryValue
                    //5: sourceLocationSubsetEntryVerifiedPresent
                    //6: sourceLocationSubsetEntryVerifiedMissing

                    int sourceLocationSubsetEntryId = rdr.GetInt32(0);
                    int sourceLocationSubsetId = rdr.GetInt32(1);
                    string sourceLocationValue = rdr.GetString(2);
                    string sourceLocationSubsetValue = rdr.GetString(3);
                    string sourceLocationSubsetEntryValue = rdr.GetString(4);

                    string sourceLocationSubsetEntryVerifiedPresent =
                        DbV5Utils.GetNullableString(rdr, 5);

                    string sourceLocationSubsetEntryVerifiedMissing =
                        DbV5Utils.GetNullableString(rdr, 6);


                    DateTime? verifiedPresent =
                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(sourceLocationSubsetEntryVerifiedPresent);

                    DateTime? verifiedMissing =
                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(sourceLocationSubsetEntryVerifiedMissing);

                    var aslse = new ArchivistSourceLocationSubsetEntry()
                                    {
                                        SourceLocationSubsetEntryId = sourceLocationSubsetEntryId,
                                        SourceLocationSubsetId = sourceLocationSubsetId,
                                        SourceId = sourceId,
                                        SourceLocationValue = sourceLocationValue,
                                        SourceLocationSubsetValue = sourceLocationSubsetValue,
                                        SourceLocationSubsetEntryValue = sourceLocationSubsetEntryValue,
                                        VerifiedPresent = verifiedPresent,
                                        VerifiedMissing = verifiedMissing
                                    };

                    if (!filterOutVerifiedMissing || aslse.IsPresent())
                    {
                        lst.Add(aslse);
                    }
                }
            }

            return lst;
        }

        /// <summary>
        /// ensure location subset entry and returns subset location entry id, whether found or created
        /// </summary>
        internal int EnsureSourceLocationSubsetEntry(int sourceId, int sourceLocationSubsetId, string locationSubsetEntryValue)
        {
            int id = -1;

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        id = EnsureSourceLocationSubsetEntry(sourceId, sourceLocationSubsetId, locationSubsetEntryValue, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return id;
        }

        internal int EnsureSourceLocationSubsetEntry(int sourceId, int sourceLocationSubsetId, string locationSubsetEntryValue, SQLiteCommand cmd)
        {
            int sourceLocationSubsetEntryId =
                GetSourceLocationSubsetEntryId(sourceId, sourceLocationSubsetId, locationSubsetEntryValue, cmd);

            if (sourceLocationSubsetEntryId < 1)
            {
                InsertOrIgnoreSourceLocationSubsetEntry(sourceId, sourceLocationSubsetId, locationSubsetEntryValue, cmd);
                sourceLocationSubsetEntryId =
                    GetSourceLocationSubsetEntryId(sourceId, sourceLocationSubsetId, locationSubsetEntryValue, cmd);
            }

            return sourceLocationSubsetEntryId;
        }

        private void InsertOrIgnoreSourceLocationSubsetEntry(int sourceId, int sourceLocationSubsetId, string locationSubsetEntryValue, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.INSERT_OR_IGNORE_INTO_SOURCE_LOCATION_SUBSET_ENTRY_VALUES_SUBSET_ID_SOURCE_ID_ENTRY_VALUE_X_Y_Z;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceLocationSubsetId });
            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceId });
            cmd.Parameters.Add(new SQLiteParameter() { Value = locationSubsetEntryValue });

            cmd.ExecuteNonQuery();
        }

        private int GetSourceLocationSubsetEntryId(int sourceId, int sourceLocationSubsetId, string locationSubsetEntryValue, SQLiteCommand cmd)
        {
            int id = -1;

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_SOURCE_LOCATION_SUBSET_ENTRY_ID_FOR_SUBSET_ID_AND_SOURCE_ID_AND_ENTRY_VALUE_X_Y_Z;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceLocationSubsetId });
            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceId });
            cmd.Parameters.Add(new SQLiteParameter() { Value = locationSubsetEntryValue });

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    id = rdr.GetInt32(0);
                }
            }

            return id;
        }

        internal void UpdateSourceLocationSubsetEntryTimeStamps(ArchivistSourceLocationSubsetEntry slse)
        {
            if(slse.SourceLocationSubsetEntryId < 1)
            {
                throw new Exception("attempted to update timestamps with SourceLocationSubsetEntryId not set");
            }

            using (var conn = new SQLiteConnection(
                 @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        UpdateSourceLocationSubsetEntryTimeStamps(slse.SourceLocationSubsetEntryId, slse.VerifiedPresent, slse.VerifiedMissing, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        private void UpdateSourceLocationSubsetEntryTimeStamps(int sourceLocationSubsetEntryId, DateTime? verifiedPresent, DateTime? verifiedMissing, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.UPDATE_SOURCE_LOCATION_SUBSET_ENTRY_VERIFIED_PRESENT_VERIFIED_MISSING_FOR_ID_X_Y_Z;

            string verifiedPresentString =
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(verifiedPresent);
            string verifiedMissingString =
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(verifiedMissing);

            cmd.Parameters.Add(new SQLiteParameter() { Value = verifiedPresentString });
            cmd.Parameters.Add(new SQLiteParameter() { Value = verifiedMissingString });
            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceLocationSubsetEntryId });

            cmd.ExecuteNonQuery();
        }

        #endregion

        #region SourceExcerptTagging
        
        internal List<TaggingXmlWrapper> GetSourceExcerptTaggingXmlWrappersForExcerptId(int sourceExcerptId)
        {
            List<TaggingXmlWrapper> lst =
                new List<TaggingXmlWrapper>();

            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        lst = GetSourceExcerptTaggingXmlWrappersForExcerptId(sourceExcerptId, cmd);

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

            return lst;
        }

        internal List<TaggingXmlWrapper> GetSourceExcerptTaggingXmlWrappersForExcerptId(int sourceExcerptId, SQLiteCommand cmd)
        {
            List<TaggingXmlWrapper> lst =
                new List<TaggingXmlWrapper>();

            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.SELECT_SOURCE_EXCERPT_TAGGING_WRAPPER_VALS_FOR_EXCERPT_ID_X;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceExcerptId });

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    //query field order
                    //0:MediaTagValue
                    //1:SourceExcerptTaggingTaggedAt
                    //2:SourceExcerptTaggingUntaggedAt
                    
                    string mediaTagValue = rdr.GetString(0);
                    string taggedAtString =
                        DbV5Utils.GetNullableString(rdr, 1);

                    string untaggedAtString =
                        DbV5Utils.GetNullableString(rdr, 2);
                    
                    DateTime? taggedAt =
                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(taggedAtString);

                    DateTime? untaggedAt =
                        TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(untaggedAtString);

                    var taggingWrapper = new TaggingXmlWrapper(mediaTagValue);
                    taggingWrapper.SetTimeStamps(taggedAt, untaggedAt);

                    lst.Add(taggingWrapper);
                }
            }

            return lst;
        }

        private void UpsertExcerptTagging(int excerptId, int tagId, String taggedAt, String untaggedAt, SQLiteCommand cmd)
        {
            int taggingId = EnsureExcerptTaggingId(excerptId, tagId, cmd);

            UpdateExcerptTaggingTimestampsById(taggingId, taggedAt, untaggedAt, cmd);
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
            foreach (SourceExcerptTagging set in ase.ExcerptTaggings)
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

        private void UpdateExcerptTaggingTimestampsById(int sourceExcerptTaggingId, String taggedAt, String untaggedAt, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                NwdContract.UPDATE_SOURCE_EXCERPT_TAGGING_TIMESTAMPS_X_Y_Z;

            //// TAGGED PARAM ///////////////////////////////////////////////////
            SQLiteParameter taggedParam = new SQLiteParameter();

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
                Value = sourceExcerptTaggingId
            });

            cmd.ExecuteNonQuery();
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

        private int EnsureExcerptTaggingId(int sourceExcerptId, int mediaTagId, SQLiteCommand cmd)
        {
            int taggingId = GetSourceExcerptTaggingId(sourceExcerptId, mediaTagId, cmd);

            if (taggingId < 1)
            {
                InsertOrIgnoreExcerptTagging(sourceExcerptId, mediaTagId, cmd);
                taggingId = GetSourceExcerptTaggingId(sourceExcerptId, mediaTagId, cmd);
            }

            return taggingId;
        }

        private int GetSourceExcerptTaggingId(int sourceExcerptId, int mediaTagId, SQLiteCommand cmd)
        {
            //just a basic lookup

            if (sourceExcerptId < 1)
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
                NwdContract.SELECT_SOURCE_EXCERPT_TAGGING_ID_X_Y;

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

        internal void DeleteArchivistSourceExcerptTaggingsByExcerptId(int sourceExcerptId)
        {
            using (var conn = new SQLiteConnection(
                @"Data Source=" + Configuration.GetSqliteDbPath(DbName)))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        DeleteArchivistSourceExcerptTaggingsByExcerptId(sourceExcerptId, cmd);
                        transaction.Commit();
                    }
                }

                conn.Close();
            }
        }

        private void DeleteArchivistSourceExcerptTaggingsByExcerptId(int sourceExcerptId,
                                                                     SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = NwdContract.DELETE_ARCHIVIST_SOURCE_EXCERPT_TAGGINGS_FOR_EXID;

            cmd.Parameters.Add(new SQLiteParameter() { Value = sourceExcerptId });

            cmd.ExecuteNonQuery();
        }


        private void InsertOrIgnoreExcerptTagging(int sourceExcerptId, int mediaTagId, SQLiteCommand cmd)
        {

            if (sourceExcerptId < 1)
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

        }

        private void InsertOrIgnoreExcerptTagging(SourceExcerptTagging set, SQLiteCommand cmd)
        {
            int sourceExcerptId = set.Excerpt.SourceExcerptId;
            int mediaTagId = set.MediaTag.MediaTagId;

            if (sourceExcerptId < 1)
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

            if (sourceExcerptId < 1)
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

        #endregion
    }
}
