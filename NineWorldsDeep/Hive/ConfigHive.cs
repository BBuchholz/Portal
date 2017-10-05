using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hive
{
    public class ConfigHive
    {
        private static string HIEROPHANT_SUFFIX = "nwd-hierophant";

        public static string GenerateHiveHierophantXmlFileName()
        {
            return NwdUtils.GetTimeStamp_yyyyMMddHHmmss() + "-" + HIEROPHANT_SUFFIX + ".xml";
        }

        /// <summary>
        /// Will get the hierophant xml files in the SyncRoot for the local device.        
        /// </summary>
        /// <returns></returns>
        public static List<string> GetHiveHierophantXmlImportFilePaths()
        {
            return GetHiveXmlImportFilePathsBySuffix(HIEROPHANT_SUFFIX);
        }

        public static List<string> TestingGetHiveHierophantXmlImportFilePaths()
        {
            return GetHiveXmlImportFilePathsBySuffix(
                HIEROPHANT_SUFFIX, 
                new HiveRoot()
                {
                    HiveRootName = "test-root"
                });
        }

        /// <summary>
        /// Will get the synergy xml files in the SyncRoot for the local device.        
        /// </summary>
        /// <returns></returns>
        public static List<string> GetHiveMnemosyneV5XmlImportFilePaths()
        {
            return GetHiveXmlImportFilePathsBySuffix("nwd-mnemosyne-v5");
        }

        /// <summary>
        /// Will get the mnemosyne xml files in the SyncRoot for the local device.        
        /// </summary>
        /// <returns></returns>
        public static List<string> GetHiveSynergyV5XmlImportFilePaths()
        {
            return GetHiveXmlImportFilePathsBySuffix("nwd-synergy-v5");
        }

        /// <summary>
        /// will return the paths of the xml folders for each
        /// active hive root. all hive xml exports should go into these folders
        /// </summary>
        /// <returns></returns>
        public static List<string> GetHiveFoldersForXmlExport()
        {
            List<string> allFolders = new List<string>();

            foreach (HiveRoot hr in UtilsHive.GetActiveRoots())
            {
                allFolders.Add(HiveRootXmlFolderPath(hr));
            }

            return allFolders;
        }

        public static string GetLocalHiveRootName()
        {
            return "main-laptop";
        }

        /// <summary>
        /// omit hiveRoot to default to local root
        /// </summary>
        /// <param name="suffix"></param>
        /// <param name="hiveRoot"></param>
        /// <returns></returns>
        public static List<string> GetHiveXmlImportFilePathsBySuffix(string suffix, HiveRoot hiveRoot = null)
        {
            /// Hive works differently from the previous version because all devices
            /// put xml into one folder, the local device root, so we don't need
            /// to iterate through profiles.

            List<string> allPaths = new List<string>();

            if(hiveRoot == null)
            {
                hiveRoot = UtilsHive.GetLocalHiveRoot();
            }

            string xmlDir = HiveRootXmlFolderPath(hiveRoot);

            if (Directory.Exists(xmlDir))
            {
                foreach (string filePath in
                            Directory.GetFiles(xmlDir,
                                                "*.xml",
                                                SearchOption.TopDirectoryOnly))
                {
                    string fileName = System.IO.Path.GetFileName(filePath);

                    if (fileName.ToLower().Contains(suffix))
                    {
                        allPaths.Add(filePath);
                    }
                }
            }


            return allPaths;
        }

        public static List<string> HiveRootFolderPaths(HiveRoot hr)
        {
            List<string> lst = new List<string>();

            foreach (string subfolder in HiveRootSubfolderPaths())
            {
                string fullPath =
                    System.IO.Path.Combine(
                        SyncFolder(),
                        "hive",
                        hr.HiveRootName,
                        subfolder);

                lst.Add(fullPath);
            }

            return lst;
        }

        public static string SyncFolder()
        {
            return Core.Configuration.SyncFolder();
        }


        private static string XML_SUB_FOLDER = "xml/incoming";
        private static string AUDIO_SUB_FOLDER = "media/audio/incoming";
        private static string IMAGE_SUB_FOLDER = "media/images/incoming";
        private static string PDFS_SUB_FOLDER = "media/pdfs/incoming";

        public static List<string> HiveRootSubfolderPaths()
        {
            List<string> lst = new List<string>();

            lst.Add(XML_SUB_FOLDER);
            lst.Add(AUDIO_SUB_FOLDER);
            lst.Add(IMAGE_SUB_FOLDER);
            lst.Add(PDFS_SUB_FOLDER);

            return lst;
        }


        private static string HiveRootSubFolderPath(HiveRoot hr, string subPath)
        {
            return Path.Combine(
                SyncFolder(),
                "hive",
                hr.HiveRootName,
                subPath);
        }

        public static string HiveRootXmlFolderPath(HiveRoot hr)
        {
            return HiveRootSubFolderPath(hr, XML_SUB_FOLDER);
        }

        public static string HiveRootAudioFolderPath(HiveRoot hr)
        {
            return HiveRootSubFolderPath(hr, AUDIO_SUB_FOLDER);
        }

        public static string HiveRootImagesFolderPath(HiveRoot hr)
        {
            return HiveRootSubFolderPath(hr, IMAGE_SUB_FOLDER);
        }

        public static string HiveRootPdfsFolderPath(HiveRoot hr)
        {
            return HiveRootSubFolderPath(hr, PDFS_SUB_FOLDER);
        }


    }
}
