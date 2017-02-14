using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hierophant
{
    public abstract class HierophantVertex
    {
        public HierophantVertex(string nameId)
        {
            NameId = nameId;
        }

        public string NameId { get; set; }

        public abstract string Details();
    }
}
