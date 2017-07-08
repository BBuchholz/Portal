using NineWorldsDeep.FragmentCloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    class WindowedUiNode : Tapestry.TapestryNode
    {
        public WindowedUiNode()
            : base("WindowedUI")
        {
            AddChild(new WareHouseFragment());
            AddChild(new AudioBrowserFragment());
            AddChild(new ImageBrowserFragment());
        }

        public override string GetShortName()
        {
            return "Windowed UI";
        }

        public override bool Parallels(Tapestry.TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            //do nothing
        }
    }
}
