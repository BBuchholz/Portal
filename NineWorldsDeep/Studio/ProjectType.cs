using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NineWorldsDeep.Studio
{
    public abstract class ProjectType
    {
        private string displayName, projectFileExtension, folderPath;

        public ProjectType(string displayName, string projectFileExtension, string folderPath)
        {
            this.displayName = displayName;
            this.projectFileExtension = projectFileExtension;
            this.folderPath = folderPath;
        }

        public override string ToString()
        {
            return displayName;
        }

        public String DisplayName
        {
            get { return displayName; }
        }

        public IEnumerable<string> GetProjectFiles()
        {
            return Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(s => s.ToLower().EndsWith(projectFileExtension));
        }

        public List<ProjectListEntry> GetProjectNames()
        {
            return GetProjectFiles()
                .Select(x => new ProjectListEntry(x)).ToList();
        }

        public abstract ProjectTypeInstance InstantiateForPath(String filePath);
    }
}