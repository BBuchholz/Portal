using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hive
{
    class UtilsHive
    {
        private static Db.Sqlite.HiveSubsetDb db = new Db.Sqlite.HiveSubsetDb();        

        internal static List<HiveRoot> GetActiveRoots()
        {
            return db.GetActiveRoots();
        }

        public static HiveRoot GetLocalHiveRoot()
        {
            return db.GetActiveRootByName(Configuration.GetLocalHiveRootName());
        }

        internal static void RefreshLobes(HiveRoot hr)
        {
            hr.Add(new MockHiveLobe("test lobe : " + TimeStamp.NowUTC().ToString()));
            hr.Add(new MockHiveLobe("test lobe 2 : " + TimeStamp.NowUTC().ToString()));
        }

        internal static void RefreshSpores(HiveLobe hl)
        {
            hl.Add(new MockHiveSpore("tst spore : " + TimeStamp.NowUTC().ToString()));
            hl.Add(new MockHiveSpore("test spore 2 : " + TimeStamp.NowUTC().ToString()));
        }

        internal static List<HiveRoot> GetDeactivatedRoots()
        {
            return db.GetDeactivatedRoots();
        }

        internal static bool HiveRootNameIsValid(string input)
        {
            Regex regex = new Regex(@"^[0-9a-z-]*$");
            Match match = regex.Match(input);

            return match.Success;
        }

        internal static void EnsureHiveRootName(string hiveRootName)
        {
            db.EnsureHiveRoot(hiveRootName);
        }

        internal static void Sync(HiveRoot hr)
        {
            db.Sync(hr);
        }

        internal static void EnsureFolderStructure(HiveRoot hr)
        {
            foreach (string folderPath in Configuration.HiveRootFolderPaths(hr))
            {
                //CreateDirectory skips any pre-existing folders
                System.IO.Directory.CreateDirectory(folderPath);
            }
        }
    }
}
