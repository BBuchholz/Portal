using NineWorldsDeep.Core;

namespace NineWorldsDeep.Studio
{
    public class AudacityProjects : ProjectType
    {
        public AudacityProjects()
            : base("Audacity Projects",
                  ".aup",
                  Configuration.AudacityProjectsFolder)
        {

        }

        public override ProjectTypeInstance InstantiateForPath(string filePath)
        {
            return new AudacityProject(filePath);
        }
    }
}