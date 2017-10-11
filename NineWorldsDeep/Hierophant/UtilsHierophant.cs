using NineWorldsDeep.Hive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hierophant
{
    public class UtilsHierophant
    {
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

            //uncomment when done testing
            //IEnumerable<string> paths = ConfigHive.GetHiveFoldersForXmlExport();
            List<string> paths = new List<string>();
            paths.Add(@"C:\NWD-SYNC\hive\test-root\xml\incoming"); //just for testing

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
            //uncomment when done testing
            //IEnumerable<string> paths = 
            //    ConfigHive.GetHiveHierophantXmlImportFilePaths();
            IEnumerable<string> paths =
                ConfigHive.TestingGetHiveHierophantXmlImportFilePaths(); //just for testing
            
            List<SemanticMap> semanticMaps = new List<SemanticMap>();

            foreach (string path in paths)
            {
                semanticMaps.AddRange(Xml.Xml.ImportHierophantSemanticMaps(path));
            }

            return semanticMaps;
        }
    }
}
