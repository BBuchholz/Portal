using NineWorldsDeep.Core;

namespace NineWorldsDeep.Studio
{
    public class FLStudioProjects : ProjectType
    {
        public FLStudioProjects()
            : base("FL Studio Projects",
                   ".flp",
                   Configuration.FLStudioProjectsFolder)
        {

        }

        public override ProjectTypeInstance InstantiateForPath(string filePath)
        {
            return new FLStudioProject(filePath);
        }
    }
}