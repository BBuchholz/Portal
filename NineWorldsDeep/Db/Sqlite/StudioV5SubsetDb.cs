using NineWorldsDeep.Core;
using NineWorldsDeep.Sqlite;
using NineWorldsDeep.Studio;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Db.Sqlite
{
    class StudioV5SubsetDb
    {
        protected string DbName { get; private set; }

        public StudioV5SubsetDb()
        {
            DbName = "nwd"; ;
        }

        public void Save(ChordProgression prog)
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
                            //insert or ignore
                            InsertChordProgression(prog, cmd);

                            //update or ignore
                            UpdateChordProgression(prog, cmd);

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            //handle exception here
                            transaction.Rollback();
                        }
                    }
                }

                conn.Close();
            }
        }

        private void InsertChordProgression(ChordProgression prog, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                STUDIO_V5_INSERT_CHORD_PROGRESSION_SIGNATURE_NOTES_X_Y;

            SQLiteParameter signatureParam = new SQLiteParameter();
            signatureParam.Value = prog.Signature;
            cmd.Parameters.Add(signatureParam);

            SQLiteParameter notesParam = new SQLiteParameter();
            notesParam.Value = prog.Notes;
            cmd.Parameters.Add(notesParam);
            
            cmd.ExecuteNonQuery();
        }

        private void UpdateChordProgression(ChordProgression prog, SQLiteCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                STUDIO_V5_UPDATE_CHORD_PROGRESSION_NOTES_BY_SIGNATURE_X_Y;

            SQLiteParameter notesParam = new SQLiteParameter();
            notesParam.Value = prog.Notes;
            cmd.Parameters.Add(notesParam);

            SQLiteParameter signatureParam = new SQLiteParameter();
            signatureParam.Value = prog.Signature;
            cmd.Parameters.Add(signatureParam);

            cmd.ExecuteNonQuery();
        }

        public List<ChordProgression> GetAllChordProgressions()
        {
            List<ChordProgression> chordProgressions = new List<ChordProgression>();

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
                            cmd.Parameters.Clear();
                            cmd.CommandText =
                        STUDIO_V5_SELECT_ALL_CHORD_PROGRESSIONS_ID_SIGNATURE_NOTES;

                            using (var rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    int id = DbV5Utils.GetNullableInt32(rdr, 0);
                                    string signature = DbV5Utils.GetNullableString(rdr, 1);
                                    string notes = DbV5Utils.GetNullableString(rdr, 2);
                                    
                                    if (!string.IsNullOrWhiteSpace(signature))
                                    {
                                        ChordProgression prog = new ChordProgression(signature);
                                        prog.ChordProgressionId = id;
                                        prog.Notes = notes;

                                        chordProgressions.Add(prog);
                                    }
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            //handle exception here
                            transaction.Rollback();
                        }
                    }
                }

                conn.Close();
            }
            
            List<ChordProgression> output = 
                chordProgressions.OrderBy(p => p.Signature, StringComparer.Ordinal).ToList();

            return output;
        }

        #region "queries"

        public static string 
            STUDIO_V5_SELECT_ALL_CHORD_PROGRESSIONS_ID_SIGNATURE_NOTES =

             "SELECT " + NwdContract.COLUMN_CHORD_PROGRESSION_ID + ", "
             + "	   " + NwdContract.COLUMN_CHORD_PROGRESSION_SIGNATURE + ", "
             + "	   " + NwdContract.COLUMN_CHORD_PROGRESSION_NOTES + " "
             + "FROM " + NwdContract.TABLE_CHORD_PROGRESSION + " ; ";

        public static string 
            STUDIO_V5_INSERT_CHORD_PROGRESSION_SIGNATURE_NOTES_X_Y =

            "INSERT OR IGNORE INTO " + NwdContract.TABLE_CHORD_PROGRESSION + "  "
             + "	(" + NwdContract.COLUMN_CHORD_PROGRESSION_SIGNATURE + ", "
             + "	 " + NwdContract.COLUMN_CHORD_PROGRESSION_NOTES + ") "
             + "VALUES "
             + "	(?, ?) ; ";

        public static string
            STUDIO_V5_UPDATE_CHORD_PROGRESSION_NOTES_BY_SIGNATURE_X_Y =

             "UPDATE " + NwdContract.TABLE_CHORD_PROGRESSION + " "
             + "SET " + NwdContract.COLUMN_CHORD_PROGRESSION_NOTES + " = ? "
             + "WHERE " + NwdContract.COLUMN_CHORD_PROGRESSION_SIGNATURE + " = ? ;";

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
                        catch (Exception ex)
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
