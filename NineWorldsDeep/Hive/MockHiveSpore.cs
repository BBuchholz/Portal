using System;

namespace NineWorldsDeep.Hive
{
    internal class MockHiveSpore : HiveSpore
    {


        public MockHiveSpore(string name, HiveLobe parentLobe) : base(name, parentLobe)
        {
            HiveSporeType = HiveSporeType.Unknown;
        }

        public override HiveSporeType HiveSporeType { get; protected set; }
    }
}