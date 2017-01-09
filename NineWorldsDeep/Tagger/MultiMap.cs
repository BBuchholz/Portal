using System;
using System.Collections.Generic;

namespace NineWorldsDeep.Tagger
{
    [Obsolete("use Core.MultiMap")]
    public class MultiMap<K, V>
    {
        private Dictionary<K, List<V>> _dict;

        public MultiMap(IEqualityComparer<K> iec)
        {
            _dict = new Dictionary<K, List<V>>(iec);
        }

        /// <summary>
        /// adds key value pair to the multimap, a null value for a key 
        /// will ensure key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(K key, V value)
        {
            List<V> lst;
            if (this._dict.TryGetValue(key, out lst))
            {
                if (!lst.Contains(value))
                {
                    lst.Add(value);
                }
            }
            else
            {
                lst = new List<V>();
                if (value != null)
                {
                    lst.Add(value);
                }
                this._dict[key] = lst;
            }
        }

        public IEnumerable<K> Keys
        {
            get { return this._dict.Keys; }
        }

        public void ClearAll()
        {
            this._dict.Clear();
        }

        public List<V> this[K key]
        {
            get
            {
                List<V> lst;
                if (!this._dict.TryGetValue(key, out lst))
                {
                    lst = new List<V>();
                    this._dict[key] = lst;
                }
                return lst;
            }
        }

        public void RemoveValue(K key, V value)
        {
            if (this._dict.ContainsKey(key))
            {
                this._dict[key].Remove(value);
            }
        }

        public void PurgeValue(V value)
        {
            foreach (K key in this.Keys)
            {
                if (this._dict[key].Contains(value))
                {
                    this._dict[key].Remove(value);
                }
            }
        }

        public void Clear(K key)
        {
            if (this._dict.ContainsKey(key))
            {
                this._dict[key].Clear();
            }
        }

        public void RemoveKey(K key)
        {
            if (this._dict.ContainsKey(key))
            {
                this._dict.Remove(key);
            }
        }

        public bool ContainsKey(K key)
        {
            return this._dict.ContainsKey(key);
        }
    }
}