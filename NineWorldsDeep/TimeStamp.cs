using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep
{
    public class TimeStamp
    {
        public static string Now()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}
