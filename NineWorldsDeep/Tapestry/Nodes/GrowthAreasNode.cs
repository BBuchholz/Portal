using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.FragmentCloud;

namespace NineWorldsDeep.Tapestry.Nodes
{
    public class GrowthAreasNode : TapestryNode
    {
        public GrowthAreasNode()
            : base("GrowthAreas")
        {
        }

        public override string GetShortName()
        {
            return "Growth Areas";
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            var window = new Studio.GrowthAreas.GrowthAreasWindow();
            window.Show();
        }
    }
}
