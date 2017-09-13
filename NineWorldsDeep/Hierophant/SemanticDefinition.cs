using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hierophant
{
    public class SemanticDefinition : Dictionary<string, string>
    {
        public SemanticKey SemanticKey { get; private set; }

        public SemanticDefinition(SemanticKey semanticKey)
        {
            SemanticKey = semanticKey;
        }
    }
}
