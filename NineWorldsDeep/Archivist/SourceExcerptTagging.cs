using NineWorldsDeep.Mnemosyne.V5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Archivist
{
    public class SourceExcerptTagging
    {
        public int SourceExcerptTaggingId { get; set; }
        public ArchivistSourceExcerpt Excerpt { get; set; }
        public MediaTag MediaTag { get; set; }
        public DateTime? TaggedAt { get; private set; }
        public DateTime? UntaggedAt { get; private set; }
        
        public void Tag()
        {
            TaggedAt = Core.TimeStamp.NowUTC();
        }

        public void Untag()
        {
            UntaggedAt = Core.TimeStamp.NowUTC();
        }
    }
}
