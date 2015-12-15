using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Core
{
    public class Fragment : IComparable
    {
        private const string name_key = "Name";
        
        private Dictionary<string, string> _meta =
            new Dictionary<string, string>();

        public Fragment(string name)
            : this(name_key, name)
        {
            //chained constructor (do nothing here)
        }

        public Fragment(string key, string value)
        {
            SetMeta(key, value);
            DisplayKey = key;
        }

        public string DisplayKey { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Meta
        {
            get { return _meta.ToList(); }
        }

        public string ToMultiLineString()
        {
            string output = "";
            foreach (KeyValuePair<string, string> kv in Meta)
            {
                output += kv.Key + ": " + kv.Value + System.Environment.NewLine;
            }
            return output;
        }

        public IEnumerable<string> MetaKeys
        {
            get
            {
                return _meta.Keys;
            }
        }

        public string DisplayValue
        {
            get
            {
                if (_meta.ContainsKey(DisplayKey) &&
                _meta[DisplayKey] != null)
                {
                    return _meta[DisplayKey];
                }

                return "[" + DisplayKey + " not specified]";
            }
        }
        
        public void SetMeta(string key, string value)
        {
            _meta[key] = value;
        }

        public void RemoveMeta(string key)
        {
            if (_meta.ContainsKey(key))
            {
                _meta.Remove(key);
            }
        }

        public string GetMeta(string key)
        {
            if (_meta.ContainsKey(key))
            {
                return _meta[key];
            }

            return null;
        }

        public override string ToString()
        {
            return DisplayValue;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Fragment otherFragment = obj as Fragment;
            if (otherFragment != null)
                return this.ToString().CompareTo(otherFragment.ToString());
            else
                throw new ArgumentException("Object is not a Fragment");
        }

        public Fragment DeepCopy()
        {
            Fragment f = null;

            foreach (KeyValuePair<string, string> kv in _meta)
            {
                if (f == null)
                {
                    f = new Fragment(kv.Key, kv.Value);
                }

                f.SetMeta(kv.Key, kv.Value);
            }

            f.DisplayKey = this.DisplayKey;

            return f;
        }

        public void Merge(Fragment f, ConflictMergeAction cma)
        {
            foreach (KeyValuePair<string, string> kv in f.Meta)
            {
                if (_meta.ContainsKey(kv.Key))
                {
                    switch (cma)
                    {
                        case ConflictMergeAction.OverwriteConflicts:
                            //overwrite current value
                            SetMeta(kv.Key, kv.Value);
                            break;
                        case ConflictMergeAction.SkipConflicts:
                            //skip value (leave alone)
                            break;
                        case ConflictMergeAction.ThrowErrorOnConflicts:
                            throw new FragmentMergeException("Merge Conflict for Fragment Key: " + kv.Key);
                    }
                }
                else
                {
                    //no conflicts, just copy regardless of ConflictMergeAction
                    SetMeta(kv.Key, kv.Value);
                }

            }

            DisplayKey = f.DisplayKey;
        }

        public List<Fragment> ToList()
        {
            return new Fragment[] { DeepCopy() }.ToList();
        }
    }
}
