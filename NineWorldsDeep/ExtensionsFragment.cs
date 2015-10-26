using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep
{
    public static class ExtensionsFragment
    {
        public static IEnumerable<string> GetMetaKeys(this IEnumerable<Fragment> fragments)
        {
            List<string> lst = new List<string>();

            foreach (Fragment f in fragments)
            {
                foreach (string key in f.MetaKeys)
                {
                    if (!lst.Contains(key))
                    {
                        lst.Add(key);
                    }
                }
            }

            return lst;
        }

        public static IEnumerable<Fragment> DeepCopy(this IEnumerable<Fragment> ie)
        {
            List<Fragment> lst = new List<Fragment>();

            foreach (Fragment f in ie)
            {
                lst.Add(f.DeepCopy());
            }

            return lst;
        }
    }
}
