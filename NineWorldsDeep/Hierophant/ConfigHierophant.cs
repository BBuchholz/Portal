using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hierophant
{
    public class ConfigHierophant
    {
        public const string ALL_KEYS_GROUP_NAME = "[[ALL]]";
        public static string HIEROPHANT_SUFFIX = "nwd-hierophant";
        
        public static string HierophantV5XmlFolder
        {
            get { return Configuration.HierophantV5XmlFolder; }
        }

        public static string GetMostRecentHierophantV5XmlArchiveFilePath()
        {
            string mostRecentFilePath = null;

            string xmlDir = HierophantV5XmlFolder;

            if (Directory.Exists(xmlDir))
            {
                List<string> hierophantFilePaths = new List<string>();

                foreach (string filePath in
                            Directory.GetFiles(xmlDir,
                                                "*.xml",
                                                SearchOption.TopDirectoryOnly))
                {
                    string fileName = System.IO.Path.GetFileName(filePath);

                    if (fileName.ToLower().Contains(HIEROPHANT_SUFFIX))
                    {
                        hierophantFilePaths.Add(filePath);
                    }
                }

                if (hierophantFilePaths.Count > 0)
                {
                    mostRecentFilePath = hierophantFilePaths.OrderBy(f => f).Last();
                }
            }

            return mostRecentFilePath;
        }

    }
}
