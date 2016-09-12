using NineWorldsDeep.Tapestry.Fragments;
using NineWorldsDeep.Tapestry.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.FragmentCloud
{
    class RootNode : Tapestry.TapestryNode
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
            AddChild(new GrowthAreasNode());
            AddChild(new TapestrySplitViewNode());
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
