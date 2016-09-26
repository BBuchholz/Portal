using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.FragmentCloud;

namespace NineWorldsDeep.Tapestry.Fragments
{
    class StudioProjectsNode : TapestryNode
    {
        public StudioProjectsNode() 
            : base("Studio/Projects")
        {
        }

        public override string GetShortName()
        {
            return "Studio Projects";
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            var window = new Studio.ProjectsWindow();
            window.Show();
        }
    }
}
