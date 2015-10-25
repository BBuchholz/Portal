using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep
{
    public class SegmentProcessor
    {
        private string processed_tags_key;

        public SegmentProcessor(string processedTagsKey)
        {
            processed_tags_key = processedTagsKey;
        }

        public IEnumerable<Fragment> 
            GetUnprocessedSegment(IEnumerable<Fragment> ie, 
                                  int segmentSize)
        {
            int segmentSizeRemaining = segmentSize;
            List<Fragment> lst = new List<Fragment>();

            foreach (Fragment f in ie)
            {
                string processed = f.GetMeta(processed_tags_key);
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
    }
}
