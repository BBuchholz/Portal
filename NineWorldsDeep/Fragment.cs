using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep
{
    public class Fragment
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

        public IEnumerable<KeyValuePair<string,string>> Meta
        {
            get { return _meta.ToList(); }
        }

        public IEnumerable<string> MetaKeys
        {
            get
            {
                return _meta.Keys;
            }
        }

        public void SetMeta(string key, string value)
        {
            _meta[key] = value;
        }

        public override string ToString()
        {
            if (_meta.ContainsKey(DisplayKey))
            {
                return _meta[DisplayKey];
            }

            return "[display key not specified]";
        }
    }
}
