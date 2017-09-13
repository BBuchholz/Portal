using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hierophant
{
    public class SemanticMap : Dictionary<SemanticKey, SemanticDefinition>
    {
        /// <summary>
        /// convenience method, same as Add(SemanticKey, SemanticDefinition), 
        /// using SemanticDefinition.SemanticKey for the SemanticKey
        /// </summary>
        /// <param name="def"></param>
        public void Add(SemanticDefinition def)
        {
            Add(def.SemanticKey, def);
        }        
    }
}
