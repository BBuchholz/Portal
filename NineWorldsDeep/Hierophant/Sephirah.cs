using System;

namespace NineWorldsDeep.Hierophant
{
    public class Sephirah : HierophantVertex
    {
        public Sephirah(string name)           
            : base(name)
        {
        }

        public override string Details()
        {
            return "Sephirah: " + NameId;
        }
    }
}