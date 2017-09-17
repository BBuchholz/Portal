using System;

namespace NineWorldsDeep.Hive
{
    internal class MockHiveSpore : HiveSpore
    {


        public MockHiveSpore(string name) : base(name)
        {
            HiveSporeType = HiveSporeType.Unknown;
        }

        public override HiveSporeType HiveSporeType { get; protected set; }
    }
}