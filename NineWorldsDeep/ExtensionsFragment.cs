using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep
{
    public static class ExtensionsFragment
    {
        [Obsolete("use Core.Fragment.ToList")]
        public static List<Fragment> ToList(this Fragment f)
        {
            return new Fragment[] { f.DeepCopy() }.ToList();
        }

        //TODO: move to ProcessableFragment (and remove processedKey as parameter)
        public static void MarkUnprocessed(this IEnumerable<Fragment> ie,
                                           string processedKey)
        {
            foreach (Fragment f in ie)
            {
                f.SetMeta(processedKey, "False");
            }
        }

        [Obsolete("use Core.ProcessableFragment")]
        public static void SetProcessed(this Deprecated.Fragment f, string processedKey)
        {
            f.SetMeta(processedKey, "True");
        }

        [Obsolete("use IEnumerable<ProcessableFragment>.GetUnprocessedSegment()")]
        public static IEnumerable<Deprecated.Fragment> 
            GetUnprocessedSegment(this IEnumerable<Deprecated.Fragment> ie,
                                  int segmentSize,
                                  string processedKey)
        {
            int segmentSizeRemaining = segmentSize;
            List<Deprecated.Fragment> lst = new List<Deprecated.Fragment>();

            foreach (Deprecated.Fragment f in ie)
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

        public static IEnumerable<ProcessableFragment>
            GetUnprocessedSegment(this IEnumerable<ProcessableFragment> ie,
                                  int segmentSize)
        {
            int segmentSizeRemaining = segmentSize;
            List<ProcessableFragment> lst = new List<ProcessableFragment>();

            foreach (ProcessableFragment f in ie)
            {
                if (!f.IsProcessed())
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

        [Obsolete("use IEnumerable<ProcessableFragment>.GetProcessed()")]
        public static IEnumerable<Fragment> 
            GetProcessed(this IEnumerable<Fragment> ie,
                         string processedKey)
        {
            return ie.Where(x => x.IsProcessed(processedKey));
        }

        public static IEnumerable<ProcessableFragment>
            GetProcessed(this IEnumerable<ProcessableFragment> ie)
        {
            return ie.Where(x => x.IsProcessed());
        }

        [Obsolete("use Core.ProcessableFragment")]
        public static bool IsProcessed(this Fragment f, string processedKey)
        {
            string val = f.GetMeta(processedKey);
            return val != null && val.Equals("True");
        }

        public static IEnumerable<string> 
            GetMetaKeys(this IEnumerable<Core.Fragment> fragments)
        {
            List<string> lst = new List<string>();

            foreach (Core.Fragment f in fragments)
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

        public static IEnumerable<ReviewableFragment>
            ToReviewables(this IEnumerable<Core.Fragment> ie)
        {
            List<ReviewableFragment> lst = new List<ReviewableFragment>();

            foreach(Core.Fragment f in ie)
            {
                lst.Add(new ReviewableFragment(f));
            }

            return lst;
        }

        public static IEnumerable<ProcessableFragment>
            ToProcessables(this IEnumerable<Fragment> ie, string processedKey)
        {
            List<ProcessableFragment> lst = new List<ProcessableFragment>();

            foreach (Fragment f in ie)
            {
                lst.Add(new ProcessableFragment(f, processedKey));
            }

            return lst;
        }

        public static IEnumerable<Core.Fragment> DeepCopy(this IEnumerable<Core.Fragment> ie)
        {
            List<Core.Fragment> lst = new List<Core.Fragment>();

            foreach (Core.Fragment f in ie)
            {
                lst.Add(f.DeepCopy());
            }

            return lst;
        }
    }
}
