using System;
using System.Collections.Generic;
using NineWorldsDeep.Tapestry;
using NineWorldsDeep.Tapestry.Nodes;

namespace NineWorldsDeep.Mock
{
    internal class MockClusterDb : ClusterNode
    {
        private List<FileSystemNode> fileNodes =
            new List<FileSystemNode>();

        public MockClusterDb()
            : base("MockClusters/Db")
        {
            //FileSubSet
            PopulateFileSubset();
        }

        private void PopulateFileSubset()
        {
            fileNodes.Add(new FileSystemNode(MockUtils.MockUri(), true));
            fileNodes.Add(new FileSystemNode(MockUtils.MockAudioUri(), true));
            fileNodes.Add(new FileSystemNode(MockUtils.MockImageUri(), true));
        }

        public override IEnumerable<TapestryNode> Children(TapestryNodeType nodeType)
        {
            List<FileSystemNode> lst =
                new List<FileSystemNode>();

            switch (nodeType)
            {
                case TapestryNodeType.DevicePath:

                    break;
            }

            return (IEnumerable<TapestryNode>)lst;
        }

        public override string GetShortName()
        {
            return "Mock Database Cluster";
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }
    }
}