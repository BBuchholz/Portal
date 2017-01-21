using System;
using NineWorldsDeep.Tapestry;
using NineWorldsDeep.Tapestry.Nodes;
using NineWorldsDeep.Core;

namespace NineWorldsDeep.FragmentCloud
{
    public class GardenFragment : Tapestry.TapestryNode
    {
        public GardenFragment()
            : base("")
        {
            int localMediaDeviceId =
                Configuration.DB.MediaSubset.LocalDeviceId;

            AddChild(new FileSystemNode("NWD", true, localMediaDeviceId));
            AddChild(new FileSystemNode("NWD-AUX", true, localMediaDeviceId));
            AddChild(new FileSystemNode("NWD-MEDIA", true, localMediaDeviceId));
            AddChild(new FileSystemNode("NWD-SNDBX", true, localMediaDeviceId));
            AddChild(new FileSystemNode("NWD-SYNC", true, localMediaDeviceId));
        }

        public override string GetShortName()
        {
            return "Garden";
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