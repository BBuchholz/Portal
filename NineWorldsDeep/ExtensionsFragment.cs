using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep
{
    public static class ExtensionsFragment
    {
        public static void MarkUnprocessed(this IEnumerable<Fragment> ie,
                                           string processedKey)
        {
            foreach (Fragment f in ie)
            {
                f.SetMeta(processedKey, "False");
            }
        }

        public static void SetProcessed(this Fragment f, string processedKey)
        {
            f.SetMeta(processedKey, "True");
        }

        public static IEnumerable<Fragment> 
            GetUnprocessedSegment(this IEnumerable<Fragment> ie,
                                  int segmentSize,
                                  string processedKey)
        {
            int segmentSizeRemaining = segmentSize;
            List<Fragment> lst = new List<Fragment>();

            foreach (Fragment f in ie)
            {
                string processed = f.GetMeta(processedKey);
                if (processed == null || processed.Equals("False"))
                {
                    lst.Add(f);
                    segmentSizeRemaining -= 1;

                    if (segmentSizeRemaining < 1)
                    {
                        break;
                    }
                }
            }

            return lst;
        }

        public static IEnumerable<Fragment> 
            GetProcessed(this IEnumerable<Fragment> ie,
                         string processedKey)
        {
            return ie.Where(x => x.IsProcessed(processedKey));
        }
        
        public static bool IsProcessed(this Fragment f, string processedKey)
        {
            string val = f.GetMeta(processedKey);
            return val != null && val.Equals("True");
        }

        public static IEnumerable<string> 
            GetMetaKeys(this IEnumerable<Fragment> fragments)
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
