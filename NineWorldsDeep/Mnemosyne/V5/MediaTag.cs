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

        public void Add(Media m)
        {
            //ToDo: should check if exists/contains here (need to define uniqueness contraints for Media)
            Media.Add(m);
        }

        public void Add(ArchivistSourceExcerpt ase)
        {
            //ToDo: should check if exists/contains here (need to define uniqueness contraints for ArchivistSourceExcerpt)
            SourceExcerpts.Add(ase);
        }
    }
}
