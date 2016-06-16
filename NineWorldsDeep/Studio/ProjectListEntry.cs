using System.IO;

namespace NineWorldsDeep.Studio
{
    public class ProjectListEntry
    {
        private string filePath;
        private string projectName;
        private string containingFolder;

        public ProjectListEntry(string filePath)
        {
            this.filePath = filePath;
            this.projectName = Path.GetFileNameWithoutExtension(filePath);
            this.containingFolder = Path.GetDirectoryName(filePath);
        }

        public string ContainingFolder
        {
            get
            {
                return containingFolder;
            }
        }

        public string ProjectName
        {
            get
            {
                return projectName;
            }
        }
    }
}