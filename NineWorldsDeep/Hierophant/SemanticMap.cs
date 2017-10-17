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
        #region fields

        private Dictionary<SemanticKey, SemanticDefinition> keysToDefs =
            new Dictionary<SemanticKey, SemanticDefinition>();

        private Dictionary<string, SemanticMap> groupNamesToGroupMaps =
            new Dictionary<string, SemanticMap>();

        private SemanticMap parentMap = null;

        #endregion

        #region properties and accessors

        public string Name { get; set; }
        
        public IEnumerable<SemanticKey> SemanticKeys
        {
            get { return keysToDefs.Keys; }
        }

        public SemanticDefinition this[SemanticKey key]
        {
            get { return keysToDefs[key]; }
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

        #endregion
        
        #region public interface

        /// <summary>
        /// will add definition to the map, indexed by its semantic key. 
        /// if the key already exists, the new value will be ignored for 
        /// this map, but Add(def) will still be called for the parent
        /// map if one is assigned. 
        /// </summary>
        /// <param name="def"></param>
        public void Add(SemanticDefinition def)
        {
            //prevent overwrite, but still add to parent map as well
            //to propogate the add up the hierarchy (namely the [[ALL]] group)
            //this means that adding a new def to an existing group
            //will add it to the group without adding it to the parent 
            //twice...

            //prevent overwrite
            if (!keysToDefs.ContainsKey(def.SemanticKey))
            {
                //even if it isn't in this map, this map
                //could be a group within another map
                //and we want to get a reference to the definition
                //with that already assigned key (to prevent 
                //duplication errors)
                if (parentMap != null )
                {
                    if (!parentMap.ContainsKey(def.SemanticKey))
                    {
                        //add to parentMap
                        parentMap.Add(def);
                    }
                    
                    //whether it was already there, or just got added, we replace our reference with
                    //the def from the parent map so it's the same object in both maps
                    def = parentMap[def.SemanticKey];                    
                }

                //finally, we add the def to this map
                keysToDefs[def.SemanticKey] = def;
            }           
            
        }

        /// <summary>
        /// will add to both this map and the group map specified.
        /// 
        /// this is the same as calling thisMap.Add(def) followed by 
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
        
        public Dictionary<SemanticKey,SemanticDefinition> AsDictionary()
        {
            return keysToDefs;
        }

        public bool HasGroup(string groupName)
        {
            return groupNamesToGroupMaps.ContainsKey(groupName);
        }

        public bool ContainsKey(SemanticKey semanticKey)
        {
            return this.keysToDefs.ContainsKey(semanticKey);
        }

        #endregion

        #region private helper methods



        #endregion
    }
}
