using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    class LocalConfigNode
    {
        private string key;
        private string value;

        public LocalConfigNode(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
