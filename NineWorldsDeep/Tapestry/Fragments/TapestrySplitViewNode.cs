using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Fragments
{
    public class TapestrySplitViewNode : TapestryNode
    {
        public TapestrySplitViewNode()
            : base("Tapestry/SplitNodeView")
        {
        }

        public override string GetShortName()
        {
            return "Tapestry Split";
        }

        public override void PerformSelectionAction()
        {
            var window = new TapestrySplitViewWindow();
            window.Show();
        }
    }
}
