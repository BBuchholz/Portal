using NineWorldsDeep.Core;

namespace NineWorldsDeep.Studio
{
    public class VisualStudioProjects : ProjectType
    {
        public VisualStudioProjects()
            : base("Visual Studio Projects",
                  ".sln",
                  Configuration.VisualStudioProjectsFolder)
        {

        }

        public override ProjectTypeInstance InstantiateForPath(string filePath)
        {
            return new VisualStudioProject(filePath);
        }
    }
}