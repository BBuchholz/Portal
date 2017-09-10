using NineWorldsDeep.Core;
using NineWorldsDeep.Hive.Lobes;
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

        public static void RefreshLobes(HiveRoot hr)
        {
            //these can be moved to the db (and will be) once we 
            //start defining more complicated Lobe Types
            //for now just a one to one mapping to sync folder hierarchy

            //xml
            hr.Add(new HiveXmlLobe(hr));
            //images
            hr.Add(new HiveImagesLobe(hr));
            //audio
            hr.Add(new HiveAudioLobe(hr));
            //pdfs
            hr.Add(new HivePdfsLobe(hr));

            //hr.Add(new MockHiveLobe("test lobe : " + TimeStamp.NowUTC().ToString()));
            //hr.Add(new MockHiveLobe("test lobe 2 : " + TimeStamp.NowUTC().ToString()));
        }

        public static void RefreshSpores(HiveLobe hl)
        {
            //for now, this is simple, but will eventually pull data from 
            //db also, so it has a wrapper method here
            hl.Collect();
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
