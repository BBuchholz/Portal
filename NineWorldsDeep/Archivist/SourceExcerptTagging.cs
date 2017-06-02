using NineWorldsDeep.Core;
using NineWorldsDeep.Mnemosyne.V5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Archivist
{
    public class SourceExcerptTagging : TaggingBase
    {
        public int SourceExcerptTaggingId { get; set; }
        public ArchivistSourceExcerpt Excerpt { get; set; }
        public MediaTag MediaTag { get; set; }

        internal void Merge(SourceExcerptTagging tagging)
        {
            //mimic MediaTagging.Merge()
            
            SourceExcerptTaggingId = TryMergeInt(SourceExcerptTaggingId, tagging.SourceExcerptTaggingId);

            SetTimeStamps(tagging.TaggedAt, tagging.UntaggedAt);
            
        }
    }
}
