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
            AddChild(new MuseNode());

            AddChild(new WindowedUiNode());
            //AddChild(new WareHouseFragment());
            //AddChild(new AudioBrowserFragment());
            //AddChild(new ImageBrowserFragment());
            
            AddChild(new GoogleApiNode());
            AddChild(new SynergyV5MasterListNode());
            AddChild(new MediaMasterNode());
            AddChild(new HierophantTreeOfLifeNode());
            AddChild(new ArchivistMasterNode());
            AddChild(new HiveMainNode());
            AddChild(new TaggedMediaMainNode());
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
