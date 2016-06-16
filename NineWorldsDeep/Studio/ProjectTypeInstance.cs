using System;

namespace NineWorldsDeep.Studio
{
    public abstract class ProjectTypeInstance
    {
        private string filePath;

        public abstract void PreProcessing();
        public abstract void Process_Exited();

        public ProjectTypeInstance(string filePath)
        {
            this.filePath = filePath;
        }

        public string FilePath
        {
            get { return filePath; }
        }

        public string ProjectName
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(filePath);
            }
        }

        public void Process_Exited(object sender, EventArgs e)
        {
            //calls postprocessing specific to derived types
            Process_Exited();

            //TODO: add notes
        }
    }
}