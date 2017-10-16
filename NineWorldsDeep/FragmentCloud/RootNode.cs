using NineWorldsDeep.Tapestry.Fragments;
using NineWorldsDeep.Tapestry.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Tapestry;

namespace NineWorldsDeep.FragmentCloud
{
    class RootNode : Tapestry.TapestryNode
    {
        [Obsolete("Use Tapestry.Nodes.TapestryRootNode")]
        public RootNode() 
            : base("")
        {
            AddChild(new GardenFragment());
            AddChild(new SynergyFragment());
            AddChild(new MuseNode());
            AddChild(new HierophantNode());
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
