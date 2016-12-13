using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Core
{
    public class TimeStamp
    {
        public static string Now()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
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

        internal static DateTime YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(string utcDateString)
        {
            DateTime date = DateTime.Parse(utcDateString);
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            return date;
        }
    }
}
