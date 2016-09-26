using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.FragmentCloud;

namespace NineWorldsDeep.Tapestry.Fragments
{
    public class StudioScalesNode : TapestryNode
    {
        public StudioScalesNode()
            : base("Studio/Scales")
        {
        }

        public override string GetShortName()
        {
            return "Studio Scales";
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            var window = new Studio.VisualScales();
            window.Show();
        }
    }
}
