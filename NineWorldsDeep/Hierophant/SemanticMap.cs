using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hierophant
{
    public class SemanticMap : Dictionary<SemanticKey, SemanticDefinition>
    {
        private Dictionary<string, SemanticMap> semanticGroupNamesToSemanticGroupMaps =
            new Dictionary<string, SemanticMap>();

        public string Name { get; set; }

        /// <summary>
        /// convenience method, same as Add(SemanticKey, SemanticDefinition), 
        /// using SemanticDefinition.SemanticKey for the SemanticKey
        /// </summary>
        /// <param name="def"></param>
        public void Add(SemanticDefinition def)
        {
            Add(def.SemanticKey, def);
        }        

        /// <summary>
        /// will add to both this map and the group map specified
        /// 
        /// convenience method, same as calling thisMap.Add(def) and then
        /// thisMap.SemanticGroup(semanticGroupName).Add(def)
        /// </summary>
        /// <param name="semanticGroupName"></param>
        public void AddTo(string semanticGroupName, SemanticDefinition def)
        {
            Add(def);
            SemanticGroup(semanticGroupName).Add(def);
        }

        /// <summary>
        /// if semantic group with semantic group name doesn't exist, it
        /// will be created
        /// </summary>
        /// <param name="semanticGroupName"></param>
        /// <returns></returns>
        public SemanticMap SemanticGroup(string semanticGroupName)
        {
            if (!semanticGroupNamesToSemanticGroupMaps.ContainsKey(semanticGroupName))
            {
                semanticGroupNamesToSemanticGroupMaps[semanticGroupName] =
                    new SemanticMap();
            }

            return semanticGroupNamesToSemanticGroupMaps[semanticGroupName];
        }

        public IEnumerable<string> SemanticGroupNames
        {
            get { return semanticGroupNamesToSemanticGroupMaps.Keys; }
        }
    }
}
