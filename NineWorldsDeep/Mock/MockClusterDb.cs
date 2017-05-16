using System;
using System.Collections.Generic;
using NineWorldsDeep.Tapestry;
using NineWorldsDeep.Tapestry.Nodes;

namespace NineWorldsDeep.Mock
{
    internal class MockClusterDb : ClusterNode
    {
        private List<DevicePathNode> mDevicePathNodes =
            new List<DevicePathNode>();

        private List<SynergyListNode> mSynergyListNodes =
            new List<SynergyListNode>();

        public MockClusterDb()
            : base("MockClusters/Db")
        {
            PopulateFileSubset();
            PopulateSynergySubset();
        }

        private void PopulateSynergySubset()
        {
            mSynergyListNodes.AddRange(Utils.GetAllSynergyListNodes());
        }

        private void PopulateFileSubset()
        {
            mDevicePathNodes.AddRange(Utils.GetAllDevicePathNodes());
        }

        public override IEnumerable<TapestryNode> Children(TapestryNodeType nodeType)
        {
            List<TapestryNode> lst =
                new List<TapestryNode>();

            /////USING THIS TO FLESH OUT ClusterDisplay.xaml
            /////add data template to user control resources for each type
            switch (nodeType)
            {
                case TapestryNodeType.DevicePath:

                    lst.AddRange(mDevicePathNodes);

                    break;

                case TapestryNodeType.SynergyList:

                    lst.AddRange(mSynergyListNodes);

                    break;

                case TapestryNodeType.SynergyListItem:

                    foreach(SynergyListNode synergyList in mSynergyListNodes)
                    {
                        lst.AddRange(synergyList.Children());
                    }

                    break;
            }

            return lst;
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