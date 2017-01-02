using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Db.Sqlite
{
    public class DbV5Utils
    {
        /// <summary>
        /// returns -1 if field is null
        /// </summary>
        /// <param name="rdr"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public static int GetNullableInt32(SQLiteDataReader rdr, int idx)
        {
            if (!rdr.IsDBNull(idx))
            {
                return rdr.GetInt32(idx);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// returns "" (empty string) if field is null
        /// </summary>
        /// <param name="rdr"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public static string GetNullableString(SQLiteDataReader rdr, int idx)
        {
            if (!rdr.IsDBNull(idx))
            {
                return rdr.GetString(idx);
            }
            else
            {
                return "";
            }
        }
    }
}
