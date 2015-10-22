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
        {
            SetMeta(name_key, name);
        }

        public IEnumerable<KeyValuePair<string,string>> Meta
        {
            get { return _meta.ToList(); }
        }

        public void SetMeta(string key, string value)
        {
            _meta[key] = value;
        }

        public override string ToString()
        {
            if (_meta.ContainsKey(name_key))
            {
                return _meta[name_key];
            }

            return "[unnamed fragment]";
        }
    }
}
