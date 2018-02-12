using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Parser;
using System.Text.RegularExpressions;
using NineWorldsDeep.Warehouse;
using NineWorldsDeep.Db.Sqlite;
using NineWorldsDeep.Hive;
using NineWorldsDeep.Model;

namespace NineWorldsDeep.Core
{
    public class Configuration
    {
        #region static fields

        private static bool _testMode = false;
        private static string _localDeviceName;
        private static string _intakeImagesFolder;
        private static string _intakeVoicememosFolder;
        private static string _intakePdfsFolder;
        private static string _trashFolder;

        #endregion

        #region static properties

        public static bool GlobalNotes { get; internal set; }
        public static DatabaseMasterAccessPoint DB { get; internal set; }
        
        public static string SyncFolder
        {
            get
            {
                //return ProcessTestMode(@"NWD-SYNC\");
                return @"C:\NWD-SYNC\";
            }
        }

        #region configurable ecosystem folders 

        public static string TrashFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_trashFolder))
                {
                    ConfigFile cfgFile = new ConfigFile();

                    if (!string.IsNullOrWhiteSpace(
                        cfgFile.TrashFolderPath))
                    {
                        _trashFolder = cfgFile.TrashFolderPath;
                    }
                    else
                    {
                        _trashFolder = @"C:\NWD-AUX\trash";
                    }
                }

                return _trashFolder;
            }

            set
            {
                ConfigFile cfgFile = new ConfigFile();
                cfgFile.TrashFolderPath = value;
                cfgFile.Save();
            }
        }
        
        public static string IntakeVoiceMemosFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_intakeVoicememosFolder))
                {
                    ConfigFile cfgFile = new ConfigFile();

                    if (!string.IsNullOrWhiteSpace(
                        cfgFile.IntakeVoicememosPath))
                    {
                        _intakeVoicememosFolder = cfgFile.IntakeVoicememosPath;
                    }
                    else
                    {
                        _intakeVoicememosFolder = @"C:\NWD-AUX\voicememos";
                    }
                }

                return _intakeVoicememosFolder;
            }

            set
            {
                ConfigFile cfgFile = new ConfigFile();
                cfgFile.IntakeVoicememosPath = value;
                cfgFile.Save();
            }
        }

        public static string IntakePdfsFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_intakePdfsFolder))
                {
                    ConfigFile cfgFile = new ConfigFile();

                    if (!string.IsNullOrWhiteSpace(
                        cfgFile.IntakePdfsPath))
                    {
                        _intakePdfsFolder = cfgFile.IntakePdfsPath;
                    }
                    else
                    {
                        _intakePdfsFolder = @"C:\NWD-AUX\pdfs";
                    }
                }

                return _intakePdfsFolder;
            }

            set
            {
                ConfigFile cfgFile = new ConfigFile();
                cfgFile.IntakePdfsPath = value;
                cfgFile.Save();
            }
        }

        public static string IntakeImagesFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_intakeImagesFolder))
                {
                    ConfigFile cfgFile = new ConfigFile();

                    if (!string.IsNullOrWhiteSpace(
                        cfgFile.IntakeImagesPath))
                    {
                        _intakeImagesFolder = cfgFile.IntakeImagesPath;
                    }
                    else
                    {
                        _intakeImagesFolder = @"C:\NWD-AUX\images";
                    }
                }

                return _intakeImagesFolder;
            }

            set
            {
                ConfigFile cfgFile = new ConfigFile();
                cfgFile.IntakeImagesPath = value;
                cfgFile.Save();
            }
        }

        #endregion

        #endregion

        #region static creation

        static Configuration()
        {
            DB = new DatabaseMasterAccessPoint();
            EnsureDirectories();

            GlobalNotes = false;
        }

        #endregion

        #region public static ecosystem folder methods

        /// <summary>
        /// returns the folder paths of all ecosystem folders (NWD, NWD-AUX, NWD-MEDIA, NWD-SYNC) in 
        /// all logical drives (such as C:\, D:\, G:\, etc.) or in
        /// supported auxilliary locations (currently Dropbox or Google Drive)
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAllEcosystemFolders()
        {
            var allFolders = new List<string>();

            //check logical drives
            foreach(string driveName in GetAllLogicalDriveNames())
            {
                foreach(string possiblePath in PossibleEcosystemDrivesForLocationPath(driveName))
                {
                    if (Directory.Exists(possiblePath))
                    {
                        allFolders.Add(possiblePath);
                    }
                }
            }


            //check user folder
            var userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            foreach (string possiblePath in PossibleEcosystemDrivesForLocationPath(userFolderPath))
            {
                if (Directory.Exists(possiblePath))
                {
                    allFolders.Add(possiblePath);
                }
            }

            //check for dropbox
            var dropboxPath = Path.Combine(userFolderPath, "Dropbox");

            foreach (string possiblePath in PossibleEcosystemDrivesForLocationPath(dropboxPath))
            {
                if (Directory.Exists(possiblePath))
                {
                    allFolders.Add(possiblePath);
                }
            }

            //check for google drive
            var googleDrivePath = Path.Combine(userFolderPath, "Google Drive");

            foreach (string possiblePath in PossibleEcosystemDrivesForLocationPath(googleDrivePath))
            {
                if (Directory.Exists(possiblePath))
                {
                    allFolders.Add(possiblePath);
                }
            }

            return allFolders;
        }

        public static IEnumerable<string> GetAllEcosystemMediaFolders()
        {
            var allFolders = new List<string>();

            //check logical drives
            foreach (string driveName in GetAllLogicalDriveNames())
            {
                foreach (string possiblePath in PossibleEcosystemMediaFoldersForLocationPath(driveName))
                {
                    if (Directory.Exists(possiblePath))
                    {
                        allFolders.Add(possiblePath);
                    }
                }
            }
            
            //check user folder
            var userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            foreach (string possiblePath in PossibleEcosystemMediaFoldersForLocationPath(userFolderPath))
            {
                if (Directory.Exists(possiblePath))
                {
                    allFolders.Add(possiblePath);
                }
            }

            //check for dropbox
            var dropboxPath = Path.Combine(userFolderPath, "Dropbox");

            foreach (string possiblePath in PossibleEcosystemMediaFoldersForLocationPath(dropboxPath))
            {
                if (Directory.Exists(possiblePath))
                {
                    allFolders.Add(possiblePath);
                }
            }

            //check for google drive
            var googleDrivePath = Path.Combine(userFolderPath, "Google Drive");

            foreach (string possiblePath in PossibleEcosystemMediaFoldersForLocationPath(googleDrivePath))
            {
                if (Directory.Exists(possiblePath))
                {
                    allFolders.Add(possiblePath);
                }
            }

            return allFolders;
        }

        

        public static IEnumerable<FolderFileCountDisplayItem> GetEcosystemFolderCounts()
        {
            return GetFolderFileCounts(GetAllEcosystemFolders());
        }



        #endregion

        #region private static ecosystem folder helper methods
        
        public static IEnumerable<FolderFileCountDisplayItem> GetFolderFileCounts(IEnumerable<string> folderPaths)
        {
            var allCounts = new List<FolderFileCountDisplayItem>();

            foreach (string path in folderPaths)
            {
                allCounts.Add(new FolderFileCountDisplayItem()
                {
                    FolderName = path,
                    FileCount = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Length
                });
            }

            return allCounts;
        }

        private static IEnumerable<string> GetAllLogicalDriveNames()
        {
            return DriveInfo.GetDrives().Select(x => x.Name);
        }

        private static IEnumerable<string> PossibleEcosystemDrivesForLocationPath(string locationPath)
        {
            var allPaths = new List<string>
            {
                Path.Combine(locationPath, "NWD"),
                Path.Combine(locationPath, "NWD-AUX"),
                Path.Combine(locationPath, "NWD-MEDIA"),
                Path.Combine(locationPath, "NWD-SYNC")
            };

            return allPaths;
        }


        private static IEnumerable<string> PossibleEcosystemMediaFoldersForLocationPath(string locationPath)
        {
            var allPaths = new List<string>
            {
                Path.Combine(locationPath, @"NWD-AUX\images"),
                Path.Combine(locationPath, @"NWD-AUX\pdfs"),
                Path.Combine(locationPath, @"NWD-AUX\voicememos")
            };

            return allPaths;
        }

        #endregion

        public static string GetArphaBetFilePath()
        {
            return @"C:\NWD\config\studio\arpaBet\cmudict.0.7a.txt";
        }

        public static string ConfigFilePath()
        {
            return @"C:\NWD\config\config.txt";
        }

        //TODO: hard-coded values need to be transformed into config files and defaults

        //public static string AbletonProjectsFolder { get { return @"C:\NWD-AUX\abletonProjects"; } }
        public static string AbletonProjectsFolder { get { return ProcessTestMode(@"NWD-AUX\abletonProjects"); } }


        public static string SyncRoot(string name)
        {
            //return ProcessTestMode(@"NWD-SYNC\" + name);
            return SyncFolder + name;
        }

        public static string SyncRootConfigFile(string profileName, string fileNameWithoutExtension)
        {
            string fileName = fileNameWithoutExtension + ".txt";
            return Path.Combine(SyncRootConfigFolder(profileName), fileName);
        }

        [Obsolete("use corresponding method in class Hive.ConfigHive")]
        internal static List<string> HiveRootFolderPaths(HiveRoot hr)
        {
            //List<string> lst = new List<string>();

            //foreach (string subfolder in HiveRootSubfolderPaths())
            //{
            //    string fullPath =
            //        System.IO.Path.Combine(
            //            SyncFolder(),
            //            "hive",
            //            hr.HiveRootName,
            //            subfolder);

            //    lst.Add(fullPath);
            //}

            //return lst;

            return ConfigHive.HiveRootFolderPaths(hr);
        }


        [Obsolete("use corresponding method in class Hive.ConfigHive")]
        private static List<string> HiveRootSubfolderPaths()
        {
            //List<string> lst = new List<string>();

            //lst.Add("xml/incoming");
            //lst.Add("media/audio/incoming");
            //lst.Add("media/images/incoming");
            //lst.Add("media/pdfs/incoming");

            //return lst;

            return ConfigHive.HiveRootSubfolderPaths();
        }
        
        [Obsolete("use corresponding method in class Hive.ConfigHive")]
        public static string HiveRootXmlFolderPath(HiveRoot hr)
        {
            //return Path.Combine(
            //    SyncFolder(),
            //    "hive",
            //    hr.HiveRootName,
            //    "xml/incoming");

            return ConfigHive.HiveRootXmlFolderPath(hr);
        }
                

        public static string SyncRootNewXmlExportFile(SyncProfile sp)
        {
            string fileName = TimeStamp.Now() + "-nwd.xml";
            return Path.Combine(SyncRootXmlFolder(sp.Name), fileName);
        }

        public static string SyncRootMostRecentXmlFile(string profileName)
        {
            String xmlDir = SyncRootXmlFolder(profileName);

            List<string> validPaths = new List<string>();

            Regex regex = new Regex("^\\d{14}-nwd.xml$");

            foreach(String path in Directory.GetFiles(xmlDir, "*.xml", SearchOption.TopDirectoryOnly))
            {
                String fileName = Path.GetFileName(path);

                Match m = regex.Match(fileName);

                if (m.Success)
                {
                    validPaths.Add(path);
                }
            }

            string mostRecentPath = null;

            if(validPaths.Count > 0)
            {
                validPaths.Sort();
                mostRecentPath = validPaths.Last();
            }

            return mostRecentPath;            
        }
        
        public static string GetTrashFileFromPath(string path)
        {
            //string trashDir = ProcessTestMode("NWD-AUX/trash");
            
            //ensure it exists
            Directory.CreateDirectory(TrashFolder);
            string fName = Path.GetFileName(path);
            return Path.Combine(TrashFolder, fName);
        }

        public static string GetSqliteDbPath(string dbNameWithoutExtension)
        {
            string dbName = dbNameWithoutExtension + ".sqlite";
            return Path.Combine(ProcessTestMode("NWD/sqlite"), dbName);
        }

        public static string SyncRootConfigFolder(string name)
        {
            return Path.Combine(SyncRoot(name), "NWD/config");
        }

        public static string SyncRootXmlFolder(string name)
        {
            return Path.Combine(SyncRoot(name), "NWD/xml");
        }

        public static string SyncFolderMedia(string profileName, string mediaFolderName)
        {
            return Path.Combine(SyncRoot(profileName), "NWD-MEDIA", mediaFolderName);
        }

        /// <summary>
        /// node counts less than this number will be displayed as small 
        /// collections. node counts equal to or greater than this number
        /// will be displayed as large collections. (the distinctions
        /// vary depending on the particular display control)
        /// </summary>
        /// <returns></returns>
        public static int NodeCollectionThreshHold()
        {
            //previously "magic number"
            return 15;
        }

        //TODO: find all paths with a hard coded root and replace with ProcessTestMode(string)

        public static string AudacityProjectsFolder { get { return @"C:\NWD-AUX\audacityProjects"; } }

        public static string AudacityWavExportsFolder { get { return @"C:\NWD-AUX\audacityWavExports"; } }

        public static string CubaseProjectsFolder { get { return @"C:\NWD-AUX\cubaseProjects"; } }
        public static string FLStudioProjectsFolder { get { return @"C:\NWD-AUX\flStudioProjects"; } }

        public static string ImagesFolder { get { return ProcessTestMode(@"NWD-AUX\images"); } }

        public static string CameraFolder { get { return ProcessTestMode(@"NWD-AUX\camera"); } }

        public static string HierophantV5XmlFolder { get { return ProcessTestMode(@"NWD\hierophant"); } }

        public static string TempV5XmlFolder { get { return ProcessTestMode(@"NWD\tempV5Xml"); } }

        public static void EnsureDirectories()
        {
            //make sure directories exist, CreateDirectory() checks if exists first
            //Directory.CreateDirectory(Configuration.PhoneSyncSynergyFolder);
            //Directory.CreateDirectory(Configuration.PhoneSyncSynergyArchivedFolder);
            //Directory.CreateDirectory(Configuration.MtpSynergySyncPath);

            Directory.CreateDirectory(Configuration.PdfsFolder);
            Directory.CreateDirectory(Configuration.ImagesFolder);
            Directory.CreateDirectory(Configuration.VoiceMemosFolder);
            Directory.CreateDirectory(Configuration.HierophantV5XmlFolder);
            Directory.CreateDirectory(Configuration.TempV5XmlFolder);
        }

        public static bool FileIsAudio(string path)
        {
            return path.ToLower().EndsWith(".wav");
        }

        public static bool FileIsImage(string path)
        {
            return path.ToLower().EndsWith(".bmp") ||
                        path.ToLower().EndsWith(".gif") ||
                        path.ToLower().EndsWith(".ico") ||
                        path.ToLower().EndsWith(".jpg") ||
                        path.ToLower().EndsWith(".png") ||
                        path.ToLower().EndsWith(".tiff");
        }

        //public static string GetPhoneSyncConfigFilePath(string fileNameWithoutExtension)
        //{
        //    return PhoneSyncConfigFolder + "\\" + fileNameWithoutExtension + ".txt";
        //}

        //public static string PhoneSyncConfigFolder
        //{
        //    get
        //    {
        //        return ProcessTestMode(@"NWD-SYNC\phone\NWD\config");
        //    }
        //}

        public static string MySqlProjectsFolder
        {
            get
            {
                return @"C:\NWD\GERM\code\sql\";
            }
        }

        //public static string PhoneSyncSynergyArchivedFolder
        //{
        //    get
        //    {
        //        return ProcessTestMode(@"NWD-SYNC\phone\NWD\synergy\archived");
        //    }
        //}

        private static string ProcessTestMode(string pathWithoutRoot)
        {
            string path = @"C:\";

            if (TestMode)
            {
                path += @"NWD-SNDBX\";
            }

            path += pathWithoutRoot;

            return path;
        }

        //[Obsolete("hard-coded entry for deactivated device")]
        //public static string TabletSyncSynergyArchivedFolder
        //{
        //    get
        //    {
        //        return ProcessTestMode(@"NWD-SYNC\tablet\NWD\synergy\archived");
        //    }
        //}

        //[Obsolete("hard-coded entry for deactivated device")]
        //public static string TabletSyncSynergyFolder
        //{
        //    get
        //    {
        //        return ProcessTestMode(@"NWD-SYNC\tablet\NWD\synergy");
        //    }
        //}

        //public static string PhoneSyncSynergyFolder
        //{
        //    get
        //    {
        //        return ProcessTestMode(@"NWD-SYNC\phone\NWD\synergy");
        //    }
        //}

        //public static string SyncFolderSynergy(string profileName)
        //{
        //    return Path.Combine(ProcessTestMode("NWD-SYNC"), profileName, @"NWD\synergy");
        //}

        //public static string SyncFolderSynergyArchive(string profileName)
        //{
        //    return Path.Combine(SyncFolderSynergy(profileName), "archived");
        //}

        //public static string GetPhoneSyncSynergyArchiveFilePath(string listName)
        //{
        //    return PhoneSyncSynergyArchivedFolder + "\\" + listName + ".txt";
        //}

        //public static string SynergyV3SyncFilePath(string profileName, string listName)
        //{
        //    return Path.Combine(ProcessTestMode("NWD-SYNC"), 
        //                        profileName, 
        //                        @"NWD\synergy\" + listName + ".txt");
        //}

        public static List<string> GetActiveSyncProfileIncomingXmlFolders()
        {
            List<string> allFolderPaths = new List<string>();

            foreach(string profileName in GetAllActiveSyncProfileNames())
            {
                string xmlDir = Path.Combine(SyncFolder,
                                             profileName,
                                             @"NWD\xml\incoming");

                //ensure directory
                Directory.CreateDirectory(xmlDir);

                allFolderPaths.Add(xmlDir);
            }

            return allFolderPaths;
        }

        /// <summary>
        /// Will get the synergy xml files in the SyncRoot for the local device.        
        /// </summary>
        /// <returns></returns>
        [Obsolete("use corresponding method in class Hive.ConfigHive")]
        public static List<string> GetHiveMnemosyneV5XmlImportFilePaths()
        {
            //return GetHiveXmlImportFilePathsBySuffix("nwd-mnemosyne-v5");

            return ConfigHive.GetHiveMnemosyneV5XmlImportFilePaths();
        }

        /// <summary>
        /// Will get the mnemosyne xml files in the SyncRoot for the local device.        
        /// </summary>
        /// <returns></returns>
        [Obsolete("use corresponding method in class Hive.ConfigHive")]
        public static List<string> GetHiveSynergyV5XmlImportFilePaths()
        {
            //return GetHiveXmlImportFilePathsBySuffix("nwd-synergy-v5");

            return ConfigHive.GetHiveSynergyV5XmlImportFilePaths();
        }
        
        [Obsolete("use corresponding method in class Hive.ConfigHive")]
        private static List<string> GetHiveXmlImportFilePathsBySuffix(string suffix)
        {
            ///// Hive works differently from the previous version because all devices
            ///// put xml into one folder, the local device root, so we don't need
            ///// to iterate through profiles.

            //List<string> allPaths = new List<string>();
            //string xmlDir = HiveRootXmlFolderPath(UtilsHive.GetLocalHiveRoot());

            //if (Directory.Exists(xmlDir))
            //{
            //    foreach (string filePath in
            //                Directory.GetFiles(xmlDir,
            //                                    "*.xml",
            //                                    SearchOption.TopDirectoryOnly))
            //    {
            //        string fileName = System.IO.Path.GetFileName(filePath);

            //        if (fileName.ToLower().Contains(suffix))
            //        {
            //            allPaths.Add(filePath);
            //        }
            //    }
            //}


            //return allPaths;

            return ConfigHive.GetHiveXmlImportFilePathsBySuffix(suffix);
        }

        public static List<string> GetSynergyV5XmlImportFilePaths()
        {
            List<string> allPaths = new List<string>();
            
            foreach(string profileName in GetAllActiveSyncProfileNames())
            {
                string xmlDir = Path.Combine(SyncFolder,
                                             profileName,
                                             @"NWD\xml\outgoing");

                if (Directory.Exists(xmlDir))
                {
                    foreach (string filePath in
                                Directory.GetFiles(xmlDir,
                                                    "*.xml",
                                                    SearchOption.TopDirectoryOnly))
                    {
                        string fileName = System.IO.Path.GetFileName(filePath);

                        if (fileName.ToLower().Contains("nwd-synergy-v5"))
                        {
                            allPaths.Add(filePath);
                        }
                    }
                }
            }

            return allPaths;
        }

        /// <summary>
        /// will return the paths of the xml folders for each
        /// active hive root. all hive xml exports should go into these folders
        /// </summary>
        /// <returns></returns>
        [Obsolete("use corresponding method in class Hive.ConfigHive")]
        public static List<string> GetHiveFoldersForXmlExport()
        {
            //List<string> allFolders = new List<string>();

            //foreach(HiveRoot hr in UtilsHive.GetActiveRoots())
            //{
            //    allFolders.Add(HiveRootXmlFolderPath(hr));
            //}

            //return allFolders;

            return ConfigHive.GetHiveFoldersForXmlExport();
        }

        public static List<string> GetMnemosyneXmlImportPaths(SyncProfile sp)
        {
            List<string> pathList = new List<string>();

            PopulateMnemosyneXmlImportPathsForProfileName(sp.Name, pathList);

            return pathList;
        }

        public static List<string> GetMnemosyneXmlImportFilePaths()
        {
            List<string> allPaths = new List<string>();

            foreach (string profileName in GetAllActiveSyncProfileNames())
            {
                //string xmlDir = Path.Combine(ProcessTestMode("NWD-SYNC"),
                //                             profileName,
                //                             @"NWD\xml\outgoing");

                //if (Directory.Exists(xmlDir))
                //{
                //    foreach (string filePath in
                //                Directory.GetFiles(xmlDir,
                //                                    "*.xml",
                //                                    SearchOption.TopDirectoryOnly))
                //    {
                //        string fileName = System.IO.Path.GetFileName(filePath);

                //        if (fileName.ToLower().Contains("nwd-mnemosyne-v5"))
                //        {
                //            allPaths.Add(filePath);
                //        }
                //    }
                //}

                PopulateMnemosyneXmlImportPathsForProfileName(profileName, allPaths);
            }

            return allPaths;
        }

        private static void PopulateMnemosyneXmlImportPathsForProfileName(
            string profileName,
            List<string> pathList)
        {
            string xmlDir = Path.Combine(SyncFolder,
                             profileName,
                             @"NWD\xml\outgoing");

            if (Directory.Exists(xmlDir))
            {
                foreach (string filePath in
                            Directory.GetFiles(xmlDir,
                                                "*.xml",
                                                SearchOption.TopDirectoryOnly))
                {
                    string fileName = System.IO.Path.GetFileName(filePath);

                    if (fileName.ToLower().Contains("nwd-mnemosyne-v5"))
                    {
                        pathList.Add(filePath);
                    }
                }
            }
        }

        private static IEnumerable<string> GetAllActiveSyncProfileNames()
        {
            List<String> profileNames = new List<string>();

            //todo: populate from db sync profiles eventually
            profileNames.Add("phone");
            profileNames.Add("logos");
            profileNames.Add("galaxy-a");
            profileNames.Add("droid-z");

            return profileNames;
        }

        //public static string GetPhoneSyncSynergyFilePath(string listName)
        //{
        //    return PhoneSyncSynergyFolder + "\\" + listName + ".txt";
        //}

        //[Obsolete("hard-coded entry for deactivated device")]
        //public static string GetTabletSyncSynergyFilePath(string listName)
        //{
        //    return TabletSyncSynergyFolder + "\\" + listName + ".txt";
        //}

        //[Obsolete("hard-coded entry for deactivated device")]
        //public static string GetTabletSyncSynergyArchiveFilePath(string listName)
        //{
        //    return TabletSyncSynergyArchivedFolder + "\\" + listName + ".txt";
        //}

        public static string VisualStudioProjectsFolder
        {
            get
            {
                return @"C:\Users\Brent\Documents\Visual Studio 2015\Projects";
            }
        }

        public static string GetTagFilePath(string folderPath)
        {
            return folderPath + "\\fileTags.xml";
        }

        public static string VoiceMemosFolder
        {
            get
            {
                return @"C:\NWD-AUX\voicememos";
            }
        }


        public static string PdfsFolder
        {
            get { return @"C:\NWD-AUX\pdfs"; }
        }


        public static string VoiceMemoTagFilePath
        {
            get
            {
                return VoiceMemosFolder + @"\fileTags.xml";
            }
        }

        //public static string PhoneSyncDisplayNameIndexFile
        //{
        //    get
        //    {
        //        return @"C:\NWD-SYNC\phone\NWD\config\DisplayNameIndex.txt";
        //    }
        //}

        //public static string PhoneSyncFileHashIndexFile
        //{
        //    get
        //    {
        //        return @"C:\NWD-SYNC\phone\NWD\config\FileHashIndex.txt";
        //    }
        //}
        
        public static bool TestMode
        {
            get
            {
                return _testMode;
            }
            set
            {
                _testMode = value;
            }
        }

        public static string PlaylistsFolder
        {
            get
            {
                return ProcessTestMode(@"NWD\playlists");
            }
        }

        /// <summary>
        /// converts path in the NWD folder hierarchy to an NwdUri
        /// supports both forward and backward slash, does not
        /// support paths outside of the NWD hierarchy
        /// eg: "/storage/0/NWD/config" and "C:\NWD\config"
        /// will both return "NWD/config" as a NwdUri object
        /// but "/storage/0/SomeOtherFolder/config" will
        /// return null
        /// </summary>
        /// <param name="path">path containing 'NWD' 
        /// (supports 'NWD-MEDIA', 'NWD-AUX', 'NWD-SNDBX', &c.)</param>
        /// <returns>a new NwdUri if the path is valid, null if the path is invalid</returns>
        public static NwdUri NwdPathToNwdUri(string path)
        {
            //TODO: remember to allow looking in folders outside of 
            //  NWD-MEDIA, using a list of top level folder uris to check 
            //  in addition to the NWD paths
            //  eg. Find(List<NwdUri> uris) 
            //  where uris = new List({new NwdUri("Pictures/Skitch"), &c.})
            //  this should be passed to the NwdUri method to prevent
            //  errors being thrown for URIs that don't start with
            //  'NWD' (this allows the programmer to specify, yes this
            //  folder is outside the ecosystem, I am intentionally including
            //  it).
            //  Maybe store the list of valid external paths in Configuration
            //  so it can be set once, system wide (to eventually be a 
            //  Configurable value, like all Properties in Configuration should
            //  eventually be)
            //
            //  INCLUDE THESE NOTES IN WHEREVER THIS GETS IMPLEMENTED 
            //  (copied verbatim from elsewhere, may need editing, 
            //   I wasn't sure on the referenced method names, but 
            //   you get the idea)

            if (!path.Contains("NWD"))
            {
                return null;
            }

            string trimmedPath = path.Substring(path.IndexOf("NWD"));

            //convert backslash style to forward slash notation
            if (trimmedPath.Contains(@"\"))
            {
                trimmedPath = trimmedPath.Replace(@"\", "/");
            }

            return new NwdUri(trimmedPath);
        }

        //[Obsolete("use corresponding method in class Hive.ConfigHive")]
        //public static string GetLocalHiveRootName()
        //{
        //    //return "main-laptop";
        //    return ConfigHive.GetLocalHiveRootName();
        //}

        public static string GetLocalDeviceDescription()
        {
            if (string.IsNullOrWhiteSpace(_localDeviceName))
            {
                //TODO: this is a hack; NEED DEVICE NAME AFTER DB REFACTOR (Device table should mimic Android)
                //string deviceName = "Main Laptop";
                
                               
                ConfigFile cfg = new ConfigFile();

                if (string.IsNullOrWhiteSpace(cfg.LocalDeviceName) || !UtilsHive.HiveRootNameIsValid(cfg.LocalDeviceName))
                {
                    var deviceName = "";

                    while (string.IsNullOrWhiteSpace(deviceName) || !UtilsHive.HiveRootNameIsValid(deviceName))
                    {
                        deviceName = UI.Prompt.Input("Enter local device name (use only lowercase letters, numbers, and hyphens)");
                    }
                    
                    cfg.LocalDeviceName = deviceName;
                    cfg.Save();

                    _localDeviceName = cfg.LocalDeviceName;
                    
                }
                else
                {
                    _localDeviceName = cfg.LocalDeviceName;
                }
            }

            return _localDeviceName;
        }

        /// <summary>
        /// converts path in the NWD folder hierarchy to an NwdUri
        /// supports both forward and backward slash, does 
        /// support paths outside of the NWD hierarchy if they are 
        /// explicitly passed to the constructor
        /// eg: "/storage/0/NWD/config" and "C:\NWD\config"
        /// will both return "NWD/config" as a NwdUri object
        /// but "/storage/0/SomeOtherFolder/config" will
        /// return null unless "SomeOtherFolder" is in the 
        /// externalFolderNames
        /// </summary>
        /// <param name="path">path containing 'NWD' 
        /// (supports 'NWD-MEDIA', 'NWD-AUX', 'NWD-SNDBX', &c.) or
        /// one of the folder names specified in external folder names</param>
        /// <param name="containsValues">a list of foldernames or 
        /// prefixes ("MtpSandbox" and "MtpDocumentation" will both match "Mtp")
        /// to permit that are outside the NWD hierarchy. Note: the entries are 
        /// matched in List order, so those with lower indexes will take 
        /// precedence over those with higher indexes. eg. if you wanted to match
        /// Pictures/Screenshots over Pictures/, you should have Pictures added
        /// to the list after the Screenshots, so that Screenshots takes precedence
        /// but Pictures will still be matched.</param>
        /// <returns>a new NwdUri if the path is valid, null if the path is invalid</returns>
        public static NwdUri NwdPathToNwdUri(string path, List<string> containsValues)
        {
            //TODO: Maybe store the list of valid external paths in Configuration
            //  so it can be set once, system wide (to eventually be a 
            //  Configurable value, like all Properties in Configuration should
            //  eventually be)

            if (!path.Contains("NWD") && !path.ContainsAny(containsValues))
            {
                return null;
            }

            string folderPrefix = null;

            //give precedence to NWD folders (so NWD/Pictures would find 
            //NWD not Pictures if Pictures/Screenshots was also an authorized folder
            if (path.Contains("NWD"))
            {
                folderPrefix = "NWD";
            }

            int idx = 0;

            while(folderPrefix == null)
            {
                if (path.Contains(containsValues[idx]))
                {
                    folderPrefix = containsValues[idx];
                }

                idx++;
            }

            string trimmedPath = path.Substring(path.IndexOf(folderPrefix));

            //convert backslash style to forward slash notation
            if (trimmedPath.Contains(@"\"))
            {
                trimmedPath = trimmedPath.Replace(@"\", "/");
            }

            return new NwdUri(trimmedPath);
        }

        /// <summary>
        /// gets filePath for file with specified name in the 
        /// mtp synergy sync folder
        /// </summary>
        /// <param name="name">file name with extension</param>
        /// <returns></returns>
        //public static string MtpSynergySyncPath
        //{
        //    get
        //    {
        //        return ProcessTestMode(@"NWD-SYNC\mtp\NWD\synergy");
        //    }
        //}

        public static string NwdAuxFolder
        {
            get
            {
                return ProcessTestMode(@"NWD-AUX");
            }
        }

        public static string NwdMediaFolder
        {
            get
            {
                return ProcessTestMode(@"NWD-MEDIA");
            }
        }

        public static string ImageStagingFolder
        {
            get
            {
                return Path.Combine(NwdMediaFolder, "exportStaging/images");
            }
        }

        public static string VoiceMemoStagingFolder
        {
            get
            {
                return Path.Combine(NwdMediaFolder, "exportStaging/audio/voicememos");
            }
        }


        public static string NwdUriToLocalPath(string uri)
        {
            if (!uri.Contains("NWD"))
            {
                return null;
            }

            string trimmedPath = uri.Substring(uri.IndexOf("NWD"));

            //convert forward slash notation to backslash style
            if (trimmedPath.Contains("/"))
            {
                trimmedPath = trimmedPath.Replace("/", @"\");
            }

            return ProcessTestMode(trimmedPath);
        }

        public class DatabaseMasterAccessPoint
        {
            public DatabaseMasterAccessPoint()
            {
                MediaSubset = new MediaV5SubsetDb();
            }

            public MediaV5SubsetDb MediaSubset { get; internal set; }
        }
    }
}
