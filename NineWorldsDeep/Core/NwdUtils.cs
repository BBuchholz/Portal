using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public static string GetTimeStamp_yyyyMMddHHmmss(DateTime dt)
        {
            return dt.ToString("yyyyMMddHHmmss");
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

        public static void OpenFileExternally(string path)
        {
            if (File.Exists(path))
            {
                //open externally
                Process proc = new Process();
                proc.StartInfo.FileName = path;
                proc.Start();
            }
        }
    }
}
