using NineWorldsDeep.FragmentCloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    class TapestryRootNode : Tapestry.TapestryNode
    {
        public TapestryRootNode() 
            : base("TapestryRoot")
        {
            //AddChild(new GardenFragment());
            //AddChild(new SynergyFragment());
            AddChild(new StudioNode());
            //AddChild(new HierophantNode());
            AddChild(new WareHouseFragment());
            AddChild(new AudioBrowserFragment());
            AddChild(new ImageBrowserFragment());
            //AddChild(new MtpFragment());
            //AddChild(new GrowthAreasNode());
            AddChild(new SynergyV5MasterListNode());
            AddChild(new MediaMasterNode());
            AddChild(new HierophantTreeOfLifeNode());
            AddChild(new ArchivistMasterNode());
            // shelved currently, may come 
            // back, so leaving code, this is the only entry point
            // AddChild(new NullClusterNode()); 
        }

        public override string GetShortName()
        {
            return "Tapestry Root";
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            //do nothing
        }
    }
}
