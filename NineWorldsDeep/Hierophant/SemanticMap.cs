using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

        private SemanticMap parentMap = null;
        
        public string Name { get; set; }
        
        public IEnumerable<SemanticKey> SemanticKeys
        {
            get { return keysToDefs.Keys; }
        }

        public SemanticDefinition this[SemanticKey key]
        {
            get { return keysToDefs[key]; }
        }
        
        /// <summary>
        /// will add definition to the map, indexed by its semantic key
        /// if the key already exists, the new value will OVERWRITE the
        /// previous one.
        /// </summary>
        /// <param name="def"></param>
        public void Add(SemanticDefinition def)
        {
            keysToDefs[def.SemanticKey] = def;

            //needed so adding to submaps from grid panes will
            //propogate up the hierarchy to the top level parent
            if (parentMap != null)
            {
                parentMap.Add(def);
            }
            
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
            if (string.IsNullOrWhiteSpace(semanticGroupName))
            {
                throw new Exception("semanticGroupName cannot be null or whitespace");
            }

            if (!groupNamesToGroupMaps.ContainsKey(semanticGroupName))
            {
                var newMap = new SemanticMap()
                {
                    parentMap = this
                };

                groupNamesToGroupMaps[semanticGroupName] = newMap;                
            }

            return groupNamesToGroupMaps[semanticGroupName];
        }

        //public IEnumerable AsDictionary()
        //{
        //    return keysToDefs;
        //}

        public Dictionary<SemanticKey,SemanticDefinition> AsDictionary()
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
