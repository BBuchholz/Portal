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
    }
}
