using NineWorldsDeep.Core;

namespace NineWorldsDeep.Studio
{
    public class CubaseProjects : ProjectType
    {
        public CubaseProjects()
            : base("Cubase Projects",
                  ".cpr",
                  Configuration.CubaseProjectsFolder)
        {

        }

        public override ProjectTypeInstance InstantiateForPath(string filePath)
        {
            return new CubaseProject(filePath);
        }
    }
}