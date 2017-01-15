using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Synergy.V5;
using System.Text.RegularExpressions;

namespace NineWorldsDeep.Core
{
    public class TimeStamp
    {
        public static string Now()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public static DateTime NowUTC()
        {
            return DateTime.UtcNow;
        }
        
        /// <summary>
        /// converts datetime to UTC first, depending on DateTime.Kind
        /// property. If the parameter is already a UTC datetime, be sure
        /// the Kind property is set to UTC to avoid offset issues
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string To_UTC_YYYY_MM_DD_HH_MM_SS(DateTime? dateTime)
        {
            if(dateTime != null)
            {
                return dateTime.Value.ToUniversalTime().ToString("yyyy-MM-dd HH':'mm':'ss");
            }
            else
            {
                return "";
            }
        }

        internal static DateTime? YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(string utcDateString)
        {
            DateTime? date = null;

            try
            {
                date = DateTime.Parse(utcDateString);
                date = DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
            }
            catch(Exception)
            {
                //do nothing
            }

            return date;
        }

        public static bool IsTimeStampedList_YYYYMMDD(SynergyV5List synergyV5List)
        {
            return ContainsTimeStamp_YYYYMMDD(synergyV5List.ListName);
        }

        public static bool ContainsTimeStamp_YYYYMMDD(string listName)
        {
            return ExtractTimeStamp_YYYYMMDD(listName) != null;
        }

        public static string StripTimeStamp_YYYYMMDD(string listName)
        {
            string timeStamp = ExtractTimeStamp_YYYYMMDD(listName);

            timeStamp += "-";

            return listName.Replace(timeStamp, "");
        }

        private static string ExtractTimeStamp_YYYYMMDD(string listName)
        {
            Regex r = new Regex(@"\d{8}");
            Match m = r.Match(listName);
            if (m.Success)
            {
                return m.Value;
            }

            return null;
        }
    }
}
