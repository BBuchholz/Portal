using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Mnemosyne.V5
{
    public class UtilsMnemosyneV5
    {
        private static string CopyToImageStaging(string path)
        {
            string imageStagingFolderPath = Configuration.ImageStagingFolder;

            //ensure directory exists
            Directory.CreateDirectory(imageStagingFolderPath);

            //create destination file path
            string fName = System.IO.Path.GetFileName(path);
            string destFilePath = System.IO.Path.Combine(imageStagingFolderPath, fName);
            
            //copy if !exists, else message                
            if (!File.Exists(destFilePath))
            {
                File.Copy(path, destFilePath);
                return "copied to: " + destFilePath;
            }
            else
            {
                return "file exists: " + destFilePath;
            }
        }

        public static void StageForExportByPath(IEnumerable<string> paths)
        {
            foreach(string path in paths)
            {
                if (IsImage(path))
                {
                    CopyToImageStaging(path);
                }
                else if (IsAudio(path))
                {
                    CopyToVoiceMemoStaging(path);
                }

                ///////////////////////////////////////////////
                //add more use cases here (pdfs, &c.)
                ///////////////////////////////////////////////


                //paths not meeting any criteria are ignored
            }
        }

        private static bool IsAudio(string path)
        {
            return path.ToLower().EndsWith(".wav") ||
                   path.ToLower().EndsWith(".mp3");
        }

        private static bool IsImage(string path)
        {
            return path.ToLower().EndsWith(".bmp") ||
                   path.ToLower().EndsWith(".gif") ||
                   path.ToLower().EndsWith(".ico") ||
                   path.ToLower().EndsWith(".jpg") ||
                   path.ToLower().EndsWith(".png") ||
                   path.ToLower().EndsWith(".tiff");
        }

        private static string CopyToVoiceMemoStaging(string path)
        {
            string vmStagingFolderPath = Configuration.VoiceMemoStagingFolder;

            //ensure directory exists
            Directory.CreateDirectory(vmStagingFolderPath);

            //create destination file path
            string fName = Path.GetFileName(path);
            string destFilePath = Path.Combine(vmStagingFolderPath, fName);
            
            //copy if !exists, else message                
            if (!File.Exists(destFilePath))
            {
                File.Copy(path, destFilePath);
                return "copied to: " + destFilePath;
            }
            else
            {
                return "file exists: " + destFilePath;
            }
        }

        public static void MoveToTrash(IEnumerable<string> paths)
        {
            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        string destFile = Configuration.GetTrashFileFromPath(path);

                        if (!File.Exists(destFile))
                        {
                            File.Move(path, destFile);
                        }
                        else
                        {
                            File.Delete(path);
                        }
                    }
                    catch (Exception ex)
                    {
                        UI.Display.Exception(ex);
                    }
                }
            }
        }
    }
}
