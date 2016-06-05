using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tagger
{
    public class TaggerDbAdapter : ITaggerDb
    {
        private string connectionString;

        public TaggerDbAdapter(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// will go through entire list, compare it to existing entries in the DB, and
        /// only insert those file elements which are new, ignoring others
        /// </summary>
        /// <param name="feLst">list to go through</param>
        /// <returns>count of files inserted (ignoring existing files)</returns>
        public int AddNewFileElementsToDb(List<FileElement> feLst)
        {
            List<FileElement> newList = new List<FileElement>();
            List<FileElement> existingList = GetFileElementsFromDb();

            foreach(FileElement fe in feLst)
            {
                if (!existingList.Contains(fe))
                {
                    newList.Add(fe);
                }
            }

            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {

                conn.Open();

                MySqlCommand cmd = new MySqlCommand("INSERT INTO fileelement (FileElementPath) VALUES (@path)", conn);
                cmd.Prepare();
                cmd.Parameters.Add("@path", MySqlDbType.VarString);

                foreach (FileElement fe in newList)
                {
                    cmd.Parameters["@path"].Value = fe.Path;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return newList.Count;
        }

        [Obsolete("use AddNewFileElementsToDb(List<FileElement>)")]
        public void AddFileElementsToDb(List<FileElement> feLst)
        {//TODO: LICENSE NOTES
            //from http://dev.mysql.com/doc/connector-net/en/connector-net-programming-prepared-preparing.html

            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {

                conn.Open();

                MySqlCommand cmd = new MySqlCommand("INSERT INTO fileelement (FileElementPath) VALUES (@path)", conn);
                cmd.Prepare();
                cmd.Parameters.Add("@path", MySqlDbType.VarString);

                foreach (FileElement fe in feLst)
                {
                    cmd.Parameters["@path"].Value = fe.Path;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

        }

        public List<FileElement> GetFileElementsFromDb()
        {//TODO: LICENSE NOTES
            //working from http://stackoverflow.com/questions/11070434/using-prepared-statement-in-c-sharp-with-mysql

            List<FileElement> feList = new List<FileElement>();
            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT FileElementId, FileElementPath FROM fileelement ", conn);
                cmd.Prepare();

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    feList.Add(FileElement.FromPath(rdr.GetString("FileElementPath")));
                }
                rdr.Close();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return feList;

        }
    }
}
