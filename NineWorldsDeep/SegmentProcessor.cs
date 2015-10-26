using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep
{
    public class SegmentProcessor
    {
        private string processed_key;

        public SegmentProcessor(string processedTagsKey)
        {
            processed_key = processedTagsKey;
        }

        public void MarkUnprocessed(IEnumerable<Fragment> ie)
        {
            foreach (Fragment f in ie)
            {
                f.SetMeta(processed_key, "False");
            }
        }

        public void MarkProcessed(Fragment f)
        {
            f.SetMeta(processed_key, "True");
        }

        public IEnumerable<Fragment> 
            GetUnprocessedSegment(IEnumerable<Fragment> ie, 
                                  int segmentSize)
        {
            int segmentSizeRemaining = segmentSize;
            List<Fragment> lst = new List<Fragment>();

            foreach (Fragment f in ie)
            {
                string processed = f.GetMeta(processed_key);
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

        public IEnumerable<Fragment> GetProcessed(IEnumerable<Fragment> ie)
        {
            return ie.Where(x => x.GetMeta(processed_key).Equals("True"));
        }
    }
}
