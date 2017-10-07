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
                        if (!Hashes.Sha1ForFilePath(filePathToMove).Equals(destFilePath, StringComparison.CurrentCultureIgnoreCase))
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

        private static string GetStagingDirectoryForFileType(string filePath)
        {
            HiveSporeType sporeType = SporeTypeFromFilePath(filePath);

            return ConfigHive.GetHiveSubFolderForRootNameAndType(
                    ConfigHive.STAGING_ROOT_NAME, sporeType);            
        }
    }

    public enum FileMovementType
    {
        MoveTo,
        CopyTo
    }
}
