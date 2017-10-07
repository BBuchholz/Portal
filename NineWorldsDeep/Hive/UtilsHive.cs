using NineWorldsDeep.Core;
using NineWorldsDeep.Hive.Lobes;
using NineWorldsDeep.Mnemosyne.V5;
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
            foreach (string folderPath in ConfigHive.HiveRootFolderPaths(hr))
            {
                //CreateDirectory skips any pre-existing folders
                System.IO.Directory.CreateDirectory(folderPath);
            }
        }

        public static void CopyToStaging(List<string> selectedPaths)
        {
            AddToStaging(selectedPaths, FileMovementType.CopyTo);
        }

        private static void AddToStaging(List<string> selectedPaths, FileMovementType moveType)
        {
            if(moveType == FileMovementType.CopyTo)
            {

                foreach (string filePathToMove in selectedPaths)
                {
                    string stagingDirectoryForFileType =
                        GetStagingDirectoryForFileType(filePathToMove);

                    string destinationFileName = Path.GetFileName(filePathToMove);
                    string destFilePath = 
                        Path.Combine(stagingDirectoryForFileType, destinationFileName);

                    if (!File.Exists(destFilePath))
                    {
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

        public static void Intake(List<string> intakeFiles)
        {
            //images
            foreach (string imageFile in
                SiftFilesForSporeType(HiveSporeType.Image, intakeFiles))
            {
                MoveFile(imageFile, Configuration.ImagesFolder);
            }

            //audio
            foreach (string audioFile in
                SiftFilesForSporeType(HiveSporeType.Audio, intakeFiles))
            {

                MoveFile(audioFile, Configuration.VoiceMemosFolder);
            }

            //pdfs
            foreach (string pdfFile in
                SiftFilesForSporeType(HiveSporeType.Pdf, intakeFiles))
            {

                MoveFile(pdfFile, Configuration.PdfsFolder);
            }
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
                        UI.Display.Message("intake destination file " + 
                            destFilePath + " already exists with a different" + 
                            " hash than the intake file " + sourceFilePath + 
                            " [aborting intake operation]");
                    }
                }
                else
                {
                    File.Move(sourceFilePath, destFilePath);
                }
            }
        }

        private static bool FileHashesAreEqual(string filePathOne, string filePathTwo)
        {
            return Hashes.Sha1ForFilePath(filePathOne).Equals(
                filePathTwo, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    public enum FileMovementType
    {
        MoveTo,
        CopyTo
    }
}
