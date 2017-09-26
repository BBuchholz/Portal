using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hierophant
{
    public class SemanticMap
    {
        private Dictionary<SemanticKey, SemanticDefinition> keysToDefs =
            new Dictionary<SemanticKey, SemanticDefinition>();

        private Dictionary<string, SemanticMap> groupNamesToGroupMaps =
            new Dictionary<string, SemanticMap>();

        public string Name { get; set; }

        /// <summary>
        /// convenience method, same as Add(SemanticKey, SemanticDefinition), 
        /// using SemanticDefinition.SemanticKey for the SemanticKey
        /// </summary>
        /// <param name="def"></param>
        public void Add(SemanticDefinition def)
        {
            keysToDefs.Add(def.SemanticKey, def);
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
            if (!groupNamesToGroupMaps.ContainsKey(semanticGroupName))
            {
                groupNamesToGroupMaps[semanticGroupName] =
                    new SemanticMap();
            }

            return groupNamesToGroupMaps[semanticGroupName];
        }

        public IEnumerable AsDictionary()
        {
            return keysToDefs;
        }

        public IEnumerable<string> SemanticGroupNames
        {
            get { return groupNamesToGroupMaps.Keys; }
        }

        public IEnumerable<SemanticDefinition> SemanticDefinitions
        {
            get
            {
                return keysToDefs.Values;
            }
        }

        public bool HasGroup(string groupName)
        {
            return groupNamesToGroupMaps.ContainsKey(groupName);
        }
    }
}
