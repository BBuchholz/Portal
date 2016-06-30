using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.FragmentCloud
{
    class RootNode : TapestryNode
    {
        public RootNode() 
            : base("")
        {
            AddChild(new GardenFragment());
            AddChild(new SynergyFragment());
            AddChild(new StudioNode());
            AddChild(new HierophantFragment());
            AddChild(new WareHouseFragment());
            AddChild(new AudioBrowserFragment());
            AddChild(new ImageBrowserFragment());
            AddChild(new MtpFragment());
        }

        public override string GetShortName()
        {
            return "Root";
        }

        public override void PerformSelectionAction()
        {
            //do nothing
        }
    }
}
