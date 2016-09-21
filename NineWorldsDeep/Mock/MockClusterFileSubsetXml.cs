using NineWorldsDeep.Tapestry.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Tapestry;

namespace NineWorldsDeep.Mock
{
    class MockClusterFileSubsetXml : ClusterNode
    {
        public MockClusterFileSubsetXml()
            : base("MockClusters/Xml/FileSubset")
        {

        }

        public override IEnumerable<TapestryNode> Children(TapestryNodeType nodeType)
        {
            List<FragmentCloud.FileSystemNode> lst =
                new List<FragmentCloud.FileSystemNode>();

            return (IEnumerable<TapestryNode>)lst;
        }

        public override string GetShortName()
        {
            return "Mock File Subset XML Cluster";
        }
    }
}
