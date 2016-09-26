using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Core;

namespace NineWorldsDeep.Tapestry.Nodes
{
    class DevicePathNode : TapestryNode
    {
        private List<TagNode> tagNodes =
            new List<TagNode>();

        private List<HashNode> hashNodes =
            new List<HashNode>();

        private string path;
        private string device;

        private DevicePathNode(string uri, params TapestryNode[] children) : base(uri, children)
        {
            //use public constructor (chains to this)
        }

        public DevicePathNode(string device, string path) 
            : base("DEVICE[" + device + "]-PATH[" + path + "]")
        {
            this.path = path;
            this.device = device;
        }

        public void Add(TagNode tagNode)
        {
            tagNodes.Add(tagNode);
        }

        public void Add(HashNode hashNode)
        {
            
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            throw new NotImplementedException();
        }

        public override string GetShortName()
        {
            throw new NotImplementedException();
        }
    }
}
