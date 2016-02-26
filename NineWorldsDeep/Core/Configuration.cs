﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Parser;

namespace NineWorldsDeep.Core
{
    public class Configuration
    {
        private static bool _testMode = false;
        
        static Configuration()
        {
            EnsureDirectories();
        }

        //TODO: hard-coded values need to be transformed into config files and defaults

        public static string AbletonProjectsFolder { get { return @"C:\NWD-AUX\abletonProjects"; } }

        public static string GetSqliteDbPath(string dbNameWithoutExtension)
        {
            string dbName = dbNameWithoutExtension + ".sqlite";
            return Path.Combine(ProcessTestMode("NWD/sqlite"), dbName);
        }

        public static string AudacityProjectsFolder { get { return @"C:\NWD-AUX\audacityProjects"; } }

        public static string AudacityWavExportsFolder { get { return @"C:\NWD-AUX\audacityWavExports"; } }

        public static string CubaseProjectsFolder { get { return @"C:\NWD-AUX\cubaseProjects"; } }
        public static string FLStudioProjectsFolder { get { return @"C:\NWD-AUX\flStudioProjects"; } }

        public static string ImagesFolder { get { return ProcessTestMode(@"NWD-AUX\images"); } }

        public static string CameraFolder { get { return ProcessTestMode(@"NWD-AUX\camera"); } }

        public static void EnsureDirectories()
        {
            //make sure directories exist
            Directory.CreateDirectory(Configuration.PhoneSyncSynergyFolder);
            Directory.CreateDirectory(Configuration.PhoneSyncSynergyArchivedFolder);
            Directory.CreateDirectory(Configuration.TabletSyncSynergyFolder);
            Directory.CreateDirectory(Configuration.TabletSyncSynergyArchivedFolder);
            Directory.CreateDirectory(Configuration.MtpSynergySyncPath);
        }

        public static string GetPhoneSyncConfigFilePath(string fileNameWithoutExtension)
        {
            return PhoneSyncConfigFolder + "\\" + fileNameWithoutExtension + ".txt";
        }

        public static string PhoneSyncConfigFolder
        {
            get
            {
                return ProcessTestMode(@"NWD-SYNC\phone\NWD\config");
            }
        }

        public static string MySqlProjectsFolder
        {
            get
            {
                return @"C:\NWD\GERM\code\sql\";
            }
        }

        public static string PhoneSyncSynergyArchivedFolder
        {
            get
            {
                return ProcessTestMode(@"NWD-SYNC\phone\NWD\synergy\archived");
            }
        }

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

        public static string TabletSyncSynergyArchivedFolder
        {
            get
            {
                return ProcessTestMode(@"NWD-SYNC\tablet\NWD\synergy\archived");
            }
        }

        public static string TabletSyncSynergyFolder
        {
            get
            {
                return ProcessTestMode(@"NWD-SYNC\tablet\NWD\synergy");
            }
        }

        public static string PhoneSyncSynergyFolder
        {
            get
            {
                return ProcessTestMode(@"NWD-SYNC\phone\NWD\synergy");
            }
        }

        public static string GetPhoneSyncSynergyArchiveFilePath(string listName)
        {
            return PhoneSyncSynergyArchivedFolder + "\\" + listName + ".txt";
        }

        public static string GetPhoneSyncSynergyFilePath(string listName)
        {
            return PhoneSyncSynergyFolder + "\\" + listName + ".txt";
        }

        public static string GetTabletSyncSynergyFilePath(string listName)
        {
            return TabletSyncSynergyFolder + "\\" + listName + ".txt";
        }

        public static string GetTabletSyncSynergyArchiveFilePath(string listName)
        {
            return TabletSyncSynergyArchivedFolder + "\\" + listName + ".txt";
        }

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

        public static string VoiceMemoTagFilePath
        {
            get
            {
                return VoiceMemosFolder + @"\fileTags.xml";
            }
        }

        public static string PhoneSyncDisplayNameIndexFile
        {
            get
            {
                return @"C:\NWD-SYNC\phone\NWD\config\DisplayNameIndex.txt";
            }
        }

        public static string PhoneSyncFileHashIndexFile
        {
            get
            {
                return @"C:\NWD-SYNC\phone\NWD\config\FileHashIndex.txt";
            }
        }

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

            while(folderPrefix == null)
            {
                int idx = 0;

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
        public static string MtpSynergySyncPath
        {
            get
            {
                return ProcessTestMode(@"NWD-SYNC\mtp\NWD\synergy");
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
    }
}