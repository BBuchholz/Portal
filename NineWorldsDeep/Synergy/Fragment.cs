using System;
using System.Collections.Generic;

namespace NineWorldsDeep.Synergy
{
    [Obsolete("use SynergyItem")]
    public class Fragment
    {
        Dictionary<string, string> keyVals =
            new Dictionary<string, string>();

        public Fragment(string fragmentString)
        {
            Parser.Parser p = new Parser.Parser();

            if (!p.IsAtomic(fragmentString))
            {
                fragmentString = "item={" + fragmentString + "}";
            }

            if (!p.Validate(fragmentString))
            {
                //if its not valid, its broken
                throw new ArgumentException("invalid fragment: " + fragmentString);
            }

            ParseIntoDictionary(keyVals, fragmentString);
        }

        //public Fragment()
        //{
        //    FragmentValue = "";
        //}

        public string FragmentValue
        {
            get
            {
                return DictionaryToFragmentValue(keyVals);
            }
        }

        public void Merge(Fragment otherFragment)
        {
            ParseIntoDictionary(keyVals, otherFragment.FragmentValue);            
        }

        //public void Merge(IEnumerable<Fragment> fragments)
        //{
        //    Dictionary<string, string> keyVals =
        //        new Dictionary<string, string>();

        //    ParseIntoDictionary(keyVals, this.FragmentValue);

        //    foreach(Fragment f in fragments)
        //    {
        //        ParseIntoDictionary(keyVals, f.FragmentValue);
        //    }

        //    FragmentValue = DictionaryToFragmentValue(keyVals);
        //}

        private string DictionaryToFragmentValue(Dictionary<string, string> keyVals)
        {
            string output = "";

            foreach(KeyValuePair<string,string> kv in keyVals)
            {
                output += kv.Key + "={" + kv.Value + "} ";
            }

            return output.Trim();
        }

        private void ParseIntoDictionary(Dictionary<string, string> keyVals, string fragmentVal)
        {
            Parser.Parser p = new Parser.Parser();

            while (fragmentVal.Length > 0 && p.Validate(fragmentVal) && p.IsAtomic(fragmentVal))
            {
                string firstKey = p.GetFirstKey(fragmentVal);

                keyVals[firstKey] = p.Extract(firstKey, fragmentVal);

                fragmentVal = p.TrimKeyVal(firstKey, fragmentVal);
            }
        }
    }
}