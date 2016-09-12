﻿using NineWorldsDeep.FragmentCloud;
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
            AddChild(new GardenFragment());
            AddChild(new SynergyFragment());
            AddChild(new StudioNode());
            AddChild(new HierophantFragment());
            AddChild(new WareHouseFragment());
            AddChild(new AudioBrowserFragment());
            AddChild(new ImageBrowserFragment());
            AddChild(new MtpFragment());
            AddChild(new GrowthAreasNode());
            AddChild(new ClusterNode());
        }

        public override string GetShortName()
        {
            return "Tapestry Root";
        }

        public override void PerformSelectionAction()
        {
            //do nothing
        }
    }
}
