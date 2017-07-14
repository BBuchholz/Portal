using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hive
{
    class UtilsHive
    {
        internal static List<HiveRoot> GetAllRoots()
        {
            List<HiveRoot> allRoots = new List<HiveRoot>();

            allRoots.Add(new HiveRoot("test root : " + TimeStamp.NowUTC().ToString()));
            allRoots.Add(new HiveRoot("test root 2 : " + TimeStamp.NowUTC().ToString()));

            return allRoots;
        }

        internal static void RefreshLobes(HiveRoot hr)
        {
            hr.Add(new HiveLobe("test lobe : " + TimeStamp.NowUTC().ToString()));
            hr.Add(new HiveLobe("test lobe 2 : " + TimeStamp.NowUTC().ToString()));
        }

        internal static void RefreshSpores(HiveLobe hl)
        {
            hl.Add(new HiveSpore("tst spore : " + TimeStamp.NowUTC().ToString()));
            hl.Add(new HiveSpore("test spore 2 : " + TimeStamp.NowUTC().ToString()));
        }
    }
}
