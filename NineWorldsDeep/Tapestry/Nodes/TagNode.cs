using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Core;

namespace NineWorldsDeep.Tapestry.Nodes
{
    class TagNode
    {
        private string mTag;

        private List<DevicePathNode> mDevicePathNodes =
            new List<DevicePathNode>();

        public TagNode(DevicePathNode devicePathNode, string tagValue)
        {

            mTag = tagValue;
            Add(devicePathNode);
        }

        public void Add(DevicePathNode nd)
        {            
            mDevicePathNodes.AddIdempotent(nd);
        }
    }
}
