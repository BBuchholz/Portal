using NineWorldsDeep.Core;
using NineWorldsDeep.Db.Sqlite;
using NineWorldsDeep.Tapestry.NodeUI;
using NineWorldsDeep.Warehouse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public static bool IsAudio(string path)
        {
            return path.ToLower().EndsWith(".wav") ||
                   path.ToLower().EndsWith(".mp3");
        }

        public static bool IsImage(string path)
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

        public static void ExportXml(
            IAsyncStatusResponsive ui, 
            List<string> filePathsToExportFor,  
            List<string> exportFolderPathsToWriteTo)
        {
            MediaV5SubsetDb db = new MediaV5SubsetDb();

            string detail = "starting export of mnemosyne metadata";
                        
            ui.StatusDetailUpdate(detail);
            
            XElement mnemosyneSubsetEl = new XElement(Xml.Xml.TAG_MNEMOSYNE_SUBSET);

            int total = filePathsToExportFor.Count;
            int count = 0;

            foreach (string filePath in filePathsToExportFor)
            {
                count++;

                ui.StatusDetailUpdate("hashing " + count + " of " + total);

                var hash = Hashes.Sha1ForFilePath(filePath);

                detail = count + " of " + total + ":" + hash + ": processing";

                //create media tag with attribute set for hash
                XElement mediaEl = Xml.Xml.CreateMediaElement(hash);

                detail = count + " of " + total + ":" + hash + ": processing tags";
                ui.StatusDetailUpdate(detail);

                var taggings = db.GetMediaTaggingsForHash(hash);

                foreach (MediaTagging tag in taggings)
                {
                    //create tag element and append to 
                    XElement tagEl = Xml.Xml.CreateTagElement(tag);
                    mediaEl.Add(tagEl);
                }

                //  db.getDevicePaths(media.Hash) <- create this
                ///////--> return a MultiMap keyed on device name, with a list of path objects (path, verified, missing)

                detail = count + " of " + total + ":" + hash + ": processing device paths";
                ui.StatusDetailUpdate(detail);

                MultiMap<string, DevicePath> devicePaths = db.GetDevicePaths(hash);

                foreach (string deviceName in devicePaths.Keys)
                {
                    XElement deviceEl = Xml.Xml.CreateDeviceElement(deviceName);

                    foreach (DevicePath path in devicePaths[deviceName])
                    {
                        XElement pathEl = Xml.Xml.CreatePathElement(path);
                        deviceEl.Add(pathEl);
                    }

                    mediaEl.Add(deviceEl);
                }

                mnemosyneSubsetEl.Add(mediaEl);
            }

            XDocument doc =
                new XDocument(
                    new XElement("nwd", mnemosyneSubsetEl));

            //here, take doc and save to all sync locations            
            string fileName =
                NwdUtils.GetTimeStamp_yyyyMMddHHmmss() + "-nwd-mnemosyne-v5.xml";
            
            foreach (string xmlIncomingFolderPath in exportFolderPathsToWriteTo)
            {
                //Ensure the directory
                Directory.CreateDirectory(xmlIncomingFolderPath);

                string fullFilePath =
                    System.IO.Path.Combine(xmlIncomingFolderPath, fileName);

                doc.Save(fullFilePath);
            }
        }
    }
}
