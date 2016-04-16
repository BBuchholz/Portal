using NineWorldsDeep.Core;

namespace NineWorldsDeep.Studio
{
    public class AbletonProjects : ProjectType
    {
        public AbletonProjects()
            : base("Ableton Projects",
                  ".als",
                  Configuration.AbletonProjectsFolder)
        {

        }

        public override ProjectTypeInstance InstantiateForPath(string filePath)
        {
            return new AbletonProject(filePath);
        }
    }
}