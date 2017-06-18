using NineWorldsDeep.Archivist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Mnemosyne.V5
{
    public class MediaTag
    {
        public int MediaTagId { get; set; }
        public string MediaTagValue { get; set; }
        public List<Media> Media { get; private set; }
        public List<ArchivistSourceExcerpt> SourceExcerpts { get; private set; }

        public MediaTag()
        {
            Media = new List<Media>();
            SourceExcerpts = new List<ArchivistSourceExcerpt>();
        }

        public void Add(Media newMedia)
        {
            bool found = false;

            foreach(Media existingMedia in Media)
            {
                if (!string.IsNullOrWhiteSpace(newMedia.MediaHash) &&
                    !string.IsNullOrWhiteSpace(existingMedia.MediaHash) &&
                    newMedia.MediaHash.Equals(
                        existingMedia.MediaHash,
                        StringComparison.CurrentCultureIgnoreCase)){

                    found = true;
                }
            }

            if (!found)
            {
                Media.Add(newMedia);
            }
        }

        public void Add(ArchivistSourceExcerpt newAse)
        {
            bool found = false;

            foreach (ArchivistSourceExcerpt existingAse in SourceExcerpts)
            {
                if (!string.IsNullOrWhiteSpace(newAse.ExcerptValue) &&
                    !string.IsNullOrWhiteSpace(existingAse.ExcerptValue)){

                    if (newAse.ExcerptValue.Equals(
                            existingAse.ExcerptValue, 
                            StringComparison.CurrentCultureIgnoreCase) &&
                        newAse.SourceId == existingAse.SourceId)
                    {
                        found = true;
                    }
                }
            }

            if (!found)
            {
                SourceExcerpts.Add(newAse);
            }
        }
    }
}
