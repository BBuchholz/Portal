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
        private string CopyToImageStaging(string path)
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

        public void StageForExportByPath(IEnumerable<string> paths)
        {
            foreach(string path in paths)
            {
                //look into Folder Load Strategies, from ? 
                //if path is image copy to image staging
                //if path is audio copy to audio staging
                //ignore rest but can add pdfs, &c. here in the future
                asdf;
            }
        }

        private string CopyToVoiceMemoStaging(string path)
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
    }
}
