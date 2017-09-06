using System;
using System.Collections.Generic;
using System.Linq;

namespace NineWorldsDeep.Hierophant
{
    /// <summary>
    /// semantic keys will be equal if they are one word, multiple words can be 
    /// separated by a hyphen and any combination of those words will be
    /// equal as well (case insensitive).
    /// 
    /// eg. "Binah-Chokmah", "Chokmah-Binah", "binah-Chokmah", etc. are all
    /// equal
    /// 
    /// </summary>
    public class SemanticKey : IEquatable<SemanticKey>
    {
        private string KeyString { get; set; }
        private List<string> SubKeys { get; set; }

        public SemanticKey(string keyString)
        {
            KeyString = keyString;
            SubKeys = keyString.Split('-')
                .Where(x => x.Trim() != "")
                .ToList();
        }

        public override string ToString()
        {
            return KeyString;
        }

        private bool ContainsSubKey(string subKey)
        {
            return SubKeys.Contains(subKey, StringComparer.OrdinalIgnoreCase);
        }

        #region "equality"

        public bool Equals(SemanticKey other)
        {
            if (other == null) return false;

            bool isEqual = true;

            foreach(string subKey in SubKeys)
            {
                if (!other.ContainsSubKey(subKey))
                {
                    isEqual = false;
                }
            }

            foreach(string subKey in other.SubKeys)
            {
                if (!ContainsSubKey(subKey))
                {
                    isEqual = false;
                }
            }

            return isEqual;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as SemanticKey);
        }

        public override int GetHashCode()
        {
            //adapted from: https://stackoverflow.com/a/1008666/670768 

            int hash = 23;

            //we sort them for comparing two SemanticKeys with the 
            //same subkeys but in a different order (addition and multiplication
            //need to happen in the exact same order with same values to 
            //yield same result)
            //
            //we create a new list of references to leave the original
            //alone
            var tempList = new List<string>(SubKeys);
            tempList.Sort();

            foreach(string subKey in tempList)
            {
                hash = hash * 37 + subKey.ToLower().GetHashCode();
            }

            return hash;
        }

        #endregion
    }
}