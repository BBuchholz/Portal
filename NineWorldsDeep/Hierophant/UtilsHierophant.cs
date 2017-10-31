using NineWorldsDeep.Hive;
using NineWorldsDeep.Tapestry.NodeUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NineWorldsDeep.Hierophant
{
    public class UtilsHierophant
    {
        public static SolidColorBrush HighlightBrush {
            get { return Brushes.Red; }
        }

        public static SolidColorBrush ClearHighlightBrush {
            get { return Brushes.White; }
        }

        private static SemanticDefinition MockDefPlanet(SemanticKey semanticKey, 
            string planet, string alchemy, string astro, string color)
        {
            var def = new SemanticDefinition(semanticKey);

            def.Add("Planet", planet);
            def.Add("Alchemical Element", alchemy);
            def.Add("Astrological Sign", astro);
            def.Add("Color", color);

            return def;
        }

        private static SemanticDefinition MockDefHerb(SemanticKey semanticKey,
            string herb, string stone, string color)
        {
            var def = new SemanticDefinition(semanticKey);

            def.Add("Herb", herb);
            def.Add("Stone", stone);
            def.Add("Color", color);

            return def;
        }

        public static IEnumerable<SemanticKey> GetHighlightedKeys(
            Dictionary<SemanticKey, HierophantUiCoupling> keysToCouplings)
        {
            List<SemanticKey> semKeys = new List<SemanticKey>();
            
            foreach(SemanticKey semKey in keysToCouplings.Keys)
            {
                var coupling = keysToCouplings[semKey];

                if (coupling.Highlighted)
                {
                    semKeys.Add(semKey);
                }
            }

            return semKeys;
        }

        //public static SemanticMap MockMap2()
        //{
        //    var semanticMap = new SemanticMap();

        //    semanticMap.Add(MockDefPlanet(new SemanticKey("testKey2"), "Venus", "Salt", "Libra", "Yellow"));
        //    semanticMap.Add(MockDefHerb(new SemanticKey("testKey3"), "Aconite", "Obsidian", "Black"));

        //    return semanticMap;
        //}

        //public static SemanticMap MockMap1()
        //{
        //    var semanticMap = new SemanticMap();

        //    semanticMap.Add(MockDefPlanet(new SemanticKey("testKey1"), "Mercury", "Sulphur", "Aries", "Silver"));
        //    semanticMap.Add(MockDefHerb(new SemanticKey("testKey4"), "Cannabis", "Emerald", "Green"));

        //    return semanticMap;
        //}

        public static SemanticMap MockMapWithGroups(string uniqueId)
        {
            var semanticMap = new SemanticMap();

            semanticMap.Name = "Semantic Map Mockup: " + uniqueId;

            semanticMap.Add(MockDefPlanet(new SemanticKey("testKey1"), "Mercury", "Sulphur", "Aries", "Silver"));
            semanticMap.Add(MockDefHerb(new SemanticKey("testKey4"), "Cannabis", "Emerald", "Green"));
            
            semanticMap.SemanticGroup("demo group").Add(MockDefPlanet(new SemanticKey("testKey2"), "Venus", "Salt", "Libra", "Yellow"));
            semanticMap.SemanticGroup("demo group").Add(MockDefHerb(new SemanticKey("testKey3"), "Aconite", "Obsidian", "Black"));

            return semanticMap;
        }

        /// <summary>
        /// convenience method. generates a one item list and
        /// calls ExportToXml(List<SemanticMap> semanticMaps)
        /// </summary>
        /// <param name="sm"></param>
        /// <returns>filename of exported file, without path</returns>
        public static string ExportToXml(SemanticMap sm)
        {
            var lst = new List<SemanticMap>();
            lst.Add(sm);

            return ExportToXml(lst);
        }

        /// <summary>
        /// generates xml file and writes it to all active hive roots
        /// </summary>
        /// <returns>filename of exported file, without path</returns>
        public static string ExportToXml(IEnumerable<SemanticMap> semanticMaps)
        {
            var doc =
                Xml.Xml.Export(semanticMaps);

            string fileName = ConfigHive.GenerateHiveHierophantXmlFileName();
            
            //this supports V5 configuration (non-Hive)
            IEnumerable<string> paths = ConfigHive.GetHiveFoldersForXmlExport();

            //just for testing
            //List<string> paths = new List<string>();
            //paths.Add(@"C:\NWD-SYNC\hive\test-root\xml\incoming"); 

            foreach (string folderPath in paths)
            {
                //ensure directory
                Directory.CreateDirectory(folderPath);

                string fullFilePath =
                    System.IO.Path.Combine(folderPath, fileName);

                doc.Save(fullFilePath);
            }

            return fileName;
        }

        public static bool IsAllKeysGroup(string groupName)
        {
            return ConfigHierophant.ALL_KEYS_GROUP_NAME.Equals(
                groupName, 
                StringComparison.CurrentCultureIgnoreCase);
        }

        public static List<SemanticMap> ImportXml()
        {
            //uncomment for V6
            //IEnumerable<string> paths = 
            //    ConfigHive.GetHiveHierophantXmlImportFilePaths();

            //just for testing
            //IEnumerable<string> paths =
            //    ConfigHive.TestingGetHiveHierophantXmlImportFilePaths();

            //V5
            List<string> paths = new List<string>();

            string archiveFilePath = ConfigHive.GetMostRecentHierophantV5XmlArchiveFilePath();
            
            List<SemanticMap> semanticMaps = new List<SemanticMap>();

            if (archiveFilePath != null)
            {
                //all this is a bit hackish for V5, V6 will change all of this, not too worried
                paths.Add(archiveFilePath);

                foreach (string path in paths)
                {
                    semanticMaps.AddRange(Xml.Xml.ImportHierophantSemanticMaps(path));
                }
            }

            return semanticMaps;
        }
    }
}
