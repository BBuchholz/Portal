using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    class HashNode : TapestryNode
    {
        private DevicePathNode devicePathNode;
        private string hashString;
        private string hashedAtTimeStamp;

        private HashNode(string uri, params TapestryNode[] children) : base(uri, children)
        {
            //use public constructor, chains to this
        }

        public HashNode(DevicePathNode devicePathNode, string hashString, string hashedAtTimeStamp)
            : this ("Hashes/" + hashString)
        {
            this.devicePathNode = devicePathNode;
            this.hashString = hashString;
            this.hashedAtTimeStamp = hashedAtTimeStamp;
        }

        public override string GetShortName()
        {
            throw new NotImplementedException();
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            throw new NotImplementedException();
        }
    }
}
