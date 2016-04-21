using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NineWorldsDeep.Core
{
    public class NwdUtils
    {
        public static string GetTimeStamp_yyyyMMddHHmmss()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// Throws an ArgumentException if name
        /// is not valid according to NWD naming conventions
        /// </summary>
        /// <param name="name"></param>
        public static void ValidateColumnAndTableName(string name)
        {
            Regex regex = new Regex("^[a-zA-Z0-9_]*$");
            if (!regex.IsMatch(name))
            {
                throw new ArgumentException("invalid name: " + name);
            }
        }
    }
}
