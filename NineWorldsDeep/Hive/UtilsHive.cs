﻿using NineWorldsDeep.Core;
using NineWorldsDeep.Hive.Lobes;
using NineWorldsDeep.Mnemosyne.V5;
using NineWorldsDeep.Tapestry.NodeUI;
using NineWorldsDeep.Warehouse;
using System;
using System.Collections.Generic;
using System.IO;
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

        public static List<HiveRoot> GetAllHiveRoots()
        {
            return db.GetAllHiveRoots();
        }

        public static List<HiveRoot> GetTestRoot()
        {
            string testRootName = "test-root";
            int testRootId = db.EnsureHiveRoot(testRootName);

            var lst = new List<HiveRoot>();
            lst.Add(new HiveRoot()
            {
                HiveRootId = testRootId,
                HiveRootName = testRootName
            });

            return lst;
        }
        
        public static HiveRoot GetLocalHiveRoot()
        {
            return db.GetActiveRootByName(ConfigHive.GetLocalHiveRootName());
        }

        public static HiveSporeType SporeTypeFromFilePath(string filePath)
        {
            if (filePath.ToLower().EndsWith(".xml"))
            {
                return HiveSporeType.Xml;
            }

            if (filePath.ToLower().EndsWith(".pdf"))
            {
                return HiveSporeType.Pdf;
            }
            
            if (UtilsMnemosyneV5.IsAudio(filePath)){

                return HiveSporeType.Audio;
            }

            if (UtilsMnemosyneV5.IsImage(filePath))
            {
                return HiveSporeType.Image;
            }

            return HiveSporeType.Unknown;
        }

        /// <summary>
        /// registers any folders found in hive top level folder that aren't in the database
        /// and returns the count of new roots registered this way. 
        /// 
        /// roots are added in a deactivated status
        /// </summary>
        /// <returns></returns>
        public static int CheckAndRegisterNewRootFolders()
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //copied from Gauntlet's Java implementation, PORT THIS...
            List<string> rootNamesFromFolderNames = new List<string>();
            List<string> unregisteredFolderNames = new List<string>();
            List<HiveRoot> allRoots = GetAllHiveRoots();
            
            //get immediate sub folders of SyncRoot (NWD-SYNC/hive/)
            foreach (string hiveRootFolderPath in 
                    ConfigHive.GetFileSystemTopLevelHiveRootFolders())
            {
                bool found = false;

                string dirName = new DirectoryInfo(hiveRootFolderPath).Name;

                foreach (HiveRoot hr in allRoots)
                {
                    if(hr.HiveRootName.Equals(dirName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        found = true;
                    }
                }

                if (!found && !dirName.StartsWith("."))
                {
                    unregisteredFolderNames.Add(dirName);
                }
            }

            //register them all
            foreach (string unregisteredRootName in unregisteredFolderNames)
            {
                HiveRoot newHr = new HiveRoot() { HiveRootName = unregisteredRootName };
                db.Sync(newHr);
                newHr.Deactivate();
                db.Sync(newHr);
            }

            return unregisteredFolderNames.Count();
        }

        public static void RefreshLobes(HiveRoot hr)
        {
            hr.ClearLobes();

            //these can be moved to the db (and will be) once we 
            //start defining more complicated Lobe Types
            //for now just a one to one mapping to sync folder hierarchy

            //xml
            hr.Add(new HiveLobeXml(hr));
            //images
            hr.Add(new HiveLobeImages(hr));
            //audio
            hr.Add(new HiveLobeAudio(hr));
            //pdfs
            hr.Add(new HiveLobePdfs(hr));

            //hr.Add(new MockHiveLobe("test lobe : " + TimeStamp.NowUTC().ToString()));
            //hr.Add(new MockHiveLobe("test lobe 2 : " + TimeStamp.NowUTC().ToString()));
        }

        public static void RefreshSpores(HiveLobe hl)
        {
            hl.ClearSpores();
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

        internal static int EnsureHiveRootName(string hiveRootName)
        {
            return db.EnsureHiveRoot(hiveRootName);
        }

        internal static void Sync(HiveRoot hr)
        {
            db.Sync(hr);
        }

        internal static void EnsureFolderStructure(HiveRoot hr)
        {
            foreach (string folderPath in ConfigHive.HiveRootFolderPaths(hr))
            {
                //CreateDirectory skips any pre-existing folders
                System.IO.Directory.CreateDirectory(folderPath);
            }
        }

        public static void CopyToAllActiveRootsExcludingStagingRoot(IEnumerable<string> sourceFilePaths)
        {
            List<HiveRoot> allButStagingRoot =
                GetActiveRoots().Where(hr => !IsStagingRoot(hr)).ToList();

            ProcessMovement(sourceFilePaths, allButStagingRoot);
        }

        public static void CopyToTestRoot(IEnumerable<string> sourceFilePaths)
        {
            ProcessMovement(sourceFilePaths, GetTestRoot());
        }

        public static void CopyToStaging(List<string> selectedPaths, IAsyncStatusResponsive ui = null)
        {
            AddToStaging(selectedPaths, FileTransportOperationType.CopyTo, ui);
        }

        private static void AddToStaging(List<string> selectedPaths, FileTransportOperationType moveType, IAsyncStatusResponsive ui = null)
        {
            if(moveType == FileTransportOperationType.CopyTo)
            {
                int count = 0;
                int total = selectedPaths.Count();

                foreach (string filePathToMove in selectedPaths)
                {
                    count++;

                    string stagingDirectoryForFileType =
                        GetStagingDirectoryForFileType(filePathToMove);

                    string destinationFileName = Path.GetFileName(filePathToMove);
                    string destFilePath = 
                        Path.Combine(stagingDirectoryForFileType, destinationFileName);

                    if (!File.Exists(destFilePath))
                    {
                        if(ui != null)
                        {
                            ui.StatusDetailUpdate("copying " + count + " of " + total + " to " + destFilePath);
                        }
                        File.Copy(filePathToMove, destFilePath);
                    }
                    else
                    {
                        //ignore if same, display message if they differ
                        if (!FileHashesAreEqual(filePathToMove, destFilePath))
                        {
                            UI.Display.Message("unable to copy file to " + destFilePath + ", file already exists and hashes do not match, aborting.");
                        }
                    }
                }
            }
            else
            {
                UI.Display.Message("Move To Staging not supported at this time, Copy to staging instead");
            }
        }

        public static bool IsLocalRoot(HiveRoot hiveRoot)
        {
            return hiveRoot.HiveRootName.Equals(
                ConfigHive.GetLocalHiveRootName(), 
                StringComparison.CurrentCultureIgnoreCase);
        }
        
        public static bool IsStagingRoot(HiveRoot hiveRoot)
        {
            return hiveRoot.HiveRootName.Equals(
                ConfigHive.STAGING_ROOT_NAME,
                StringComparison.CurrentCultureIgnoreCase);
        }

        private static string GetStagingDirectoryForFileType(string filePath)
        {
            HiveSporeType sporeType = SporeTypeFromFilePath(filePath);

            return ConfigHive.GetHiveSubFolderForRootNameAndType(
                    ConfigHive.STAGING_ROOT_NAME, sporeType);            
        }

        public static string Intake(List<string> intakeFiles)
        {
            //we will be dealing with multiple sorts of file intake
            List<string> responses = new List<string>();
            int count = 0;

            //images
            foreach (string imageFile in
                SiftFilesForSporeType(HiveSporeType.Image, intakeFiles))
            {
                count++;
                MoveFile(imageFile, Configuration.IntakeImagesFolder);
            }

            if (count > 0) responses.Add(count + " images");

            count = 0;
            //audio
            foreach (string audioFile in
                SiftFilesForSporeType(HiveSporeType.Audio, intakeFiles))
            {
                count++;
                MoveFile(audioFile, Configuration.IntakeVoiceMemosFolder);
            }

            if (count > 0) responses.Add(count + " audio files");

            count = 0;
            //pdfs
            foreach (string pdfFile in
                SiftFilesForSporeType(HiveSporeType.Pdf, intakeFiles))
            {
                count++;
                MoveFile(pdfFile, Configuration.IntakePdfsFolder);
            }

            if (count > 0) responses.Add(count + " pdfs");

            count = 0;
            //xml
            if (SiftFilesForSporeType(HiveSporeType.Xml, intakeFiles).Count() > 0)
            {
                count++;
            }

            if (count > 0) responses.Add(count + 
                " xml files found, intake awaiting implementation, use Media -> Utilities -> Xml Import");

            return String.Join(" : ", responses);
        }

        private static IEnumerable<string> SiftFilesForSporeType(
            HiveSporeType sporeType, List<string> filePathsToSift)
        {
            List<string> siftedFiles = new List<string>();

            foreach(string filePath in filePathsToSift)
            {
                if(SporeTypeFromFilePath(filePath) == sporeType)
                {
                    siftedFiles.Add(filePath);
                }
            }

            return siftedFiles;
        }

        private static void MoveFile(
            string sourceFilePath, string destinationFolderPath)
        {
            if(File.Exists(sourceFilePath) && 
                Directory.Exists(destinationFolderPath))
            {
                string destFilePath = 
                    Path.Combine(destinationFolderPath,
                                 Path.GetFileName(sourceFilePath));

                if (File.Exists(destFilePath))
                {
                    if(FileHashesAreEqual(sourceFilePath, destFilePath))
                    {
                        //already exists
                        File.Delete(sourceFilePath);
                    }
                    else
                    {
                        UI.Display.Message("destination file " + 
                            destFilePath + " already exists with a different" + 
                            " hash than the source file " + sourceFilePath + 
                            " [aborting intake operation]");
                    }
                }
                else
                {
                    File.Move(sourceFilePath, destFilePath);
                }
            }
        }

        private static void CopyFile(
            string sourceFilePath, string destinationFolderPath)
        {
            if (File.Exists(sourceFilePath) &&
                Directory.Exists(destinationFolderPath))
            {
                string destFilePath =
                    Path.Combine(destinationFolderPath,
                                 Path.GetFileName(sourceFilePath));

                if (File.Exists(destFilePath))
                {
                    if (FileHashesAreEqual(sourceFilePath, destFilePath))
                    {
                        //already exists
                        //do nothing
                    }
                    else
                    {
                        UI.Display.Message("destination file " +
                            destFilePath + " already exists with a different" +
                            " hash than the source file " + sourceFilePath +
                            " [aborting intake operation]");
                    }
                }
                else
                {
                    File.Copy(sourceFilePath, destFilePath);
                }
            }
        }

        private static bool FileHashesAreEqual(string filePathOne, string filePathTwo)
        {
            return Hashes.Sha1ForFilePath(filePathOne).Equals(
                Hashes.Sha1ForFilePath(filePathTwo), StringComparison.CurrentCultureIgnoreCase);
        }

        internal static void ProcessMovement(
            IEnumerable<string> sourceFilePaths, 
            HiveRoot destinationRoot, 
            FileTransportOperationType fileTransportType)
        {
            if(destinationRoot == null)
            {
                UI.Display.Message("need a destination root...");
                return;
            }

            foreach (string sourceFilePath in sourceFilePaths)
            {
                //create destination file path
                string destinationFilePathFolder =
                    ConfigHive.GetHiveSubFolderForRootNameAndType(
                        destinationRoot.HiveRootName,
                        SporeTypeFromFilePath(sourceFilePath));
                
                switch (fileTransportType)
                {
                    case FileTransportOperationType.CopyTo:

                        CopyFile(sourceFilePath, destinationFilePathFolder);
                        break;

                    case FileTransportOperationType.MoveTo:

                        MoveFile(sourceFilePath, destinationFilePathFolder);
                        break;
                }
            }
        }

        internal static void ProcessMovement(
            IEnumerable<string> sourceFilePaths,
            IEnumerable<HiveRoot> destinationRoots)
        {
            if (destinationRoots == null || destinationRoots.Count() < 1)
            {
                UI.Display.Message("need at least one destination root to copy to...");
                return;
            }
            
            foreach (string sourceFilePath in sourceFilePaths)
            {
                foreach (var destinationRoot in destinationRoots)
                {
                    //create destination file path
                    string destinationFilePathFolder =
                        ConfigHive.GetHiveSubFolderForRootNameAndType(
                            destinationRoot.HiveRootName,
                            SporeTypeFromFilePath(sourceFilePath));

                    CopyFile(sourceFilePath, destinationFilePathFolder);
                }
            }
        }
    }

    public enum FileTransportOperationType
    {
        MoveTo,
        CopyTo
    }
}
