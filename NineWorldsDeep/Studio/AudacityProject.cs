using NineWorldsDeep.Core;
using System.IO;

namespace NineWorldsDeep.Studio
{
    internal class AudacityProject : ProjectTypeInstance
    {
        private string filePath;

        public AudacityProject(string filePath)
        {
            this.filePath = filePath;
        }

        public override void PreProcessing()
        {
            string fName = Path.GetFileNameWithoutExtension(filePath);
            string dir = Configuration.AudacityWavExportsFolder;
            string folderToCheck = Path.Combine(dir, fName);

            if (!Directory.Exists(folderToCheck))
            {
                if (UI.Prompt.Confirm("Create folder " + folderToCheck + "?"))
                {
                    Directory.CreateDirectory(folderToCheck);
                }
            }

        }

        public override void Process_Exited()
        {

        }
    }
}