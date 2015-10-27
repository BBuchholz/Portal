﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep
{
    public class Fragment : IComparable
    {
        private const string name_key = "Name";
        private const string flag_verify_key = "FlagForVerification";

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

        public IEnumerable<KeyValuePair<string,string>> Meta
        {
            get { return _meta.ToList(); }
        }

        public string ToMultiLineString()
        {
            string output = "";
            foreach(KeyValuePair<string, string> kv in Meta)
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

        public bool IsFlagged
        {
            get
            {
                string val = GetMeta(flag_verify_key);
                return val != null && val.Equals("True");
            }
            private set
            {
                SetMeta(flag_verify_key, value.ToString());
            }
        }
        
        public string KeyToReview
        {
            get
            {
                return GetMeta("KeyToReview");
            }
            private set
            {
                SetMeta("KeyToReview", value);
            }
        }

        public void FlagForReview(string keyToReview)
        {
            IsFlagged = true;
            KeyToReview = keyToReview;
        }

        public void ProcessReviewed(string keyToReview)
        {
            IsFlagged = false;
            RemoveMeta(keyToReview);
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
            
            foreach(KeyValuePair<string, string> kv in _meta)
            {
                if(f == null)
                {
                    f = new Fragment(kv.Key, kv.Value);
                }

                f.SetMeta(kv.Key, kv.Value);
            }

            f.DisplayKey = this.DisplayKey;

            return f;
        }
    }
}
