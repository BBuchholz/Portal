using System;
using System.Collections.Generic;
using NineWorldsDeep.Tapestry;
using NineWorldsDeep.Tapestry.Nodes;

namespace NineWorldsDeep.Mock
{
    internal class MockClusterDb : ClusterNode
    {
        public MockClusterDb()
            : base("MockClusters/Db")
        {

        }

        public override IEnumerable<TapestryNode> Children(TapestryNodeType nodeType)
        {
            List<FragmentCloud.FileSystemNode> lst =
                new List<FragmentCloud.FileSystemNode>();

            switch (nodeType)
            {
                case TapestryNodeType.DevicePath:

                    lst.Add(new FragmentCloud.FileSystemNode(MockUtils.MockUri(), true));

                    break;
            }

            return (IEnumerable<TapestryNode>)lst;
        }

        public override string GetShortName()
        {
            return "Mock Database Cluster";
        }
    }
}