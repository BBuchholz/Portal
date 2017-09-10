using System;

namespace NineWorldsDeep.Hive
{
    internal class MockHiveLobe : HiveLobe
    {
        public MockHiveLobe(string name, HiveRoot hr) : base(name, hr)
        {
        }

        public override void Collect()
        {
            //do nothing
        }
    }
}