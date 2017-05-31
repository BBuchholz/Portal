using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Archivist
{
    public class ArchivistSource
    {
        public int SourceId { get; set; }
        public int SourceTypeId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Director { get; set; }
        public string Year { get; set; }
        public string Url { get; set; }
        public string RetrievalDate { get; set; }
        public List<ArchivistSourceExcerpt> Excerpts { get; private set; }

        public ArchivistSource()
        {
            Excerpts = new List<ArchivistSourceExcerpt>();
        }

        public void Add(ArchivistSourceExcerpt excerpt)
        {
            if(excerpt.SourceId != SourceId)
            {
                if(excerpt.SourceId < 1)
                {
                    excerpt.SourceId = SourceId;
                }
                else
                {
                    throw new Exception("cannot add excerpt with conflicting source id");
                }
            }

            if (string.IsNullOrWhiteSpace(excerpt.ExcerptValue))
            {
                throw new Exception("cannot add empty excerpt value");
            }

            bool found = false;

            foreach(ArchivistSourceExcerpt ase in Excerpts)
            {
                if(ase.SourceId == excerpt.SourceId &&
                    ase.ExcerptValue == excerpt.ExcerptValue)
                {
                    found = true;
                }
            }

            if (!found)
            {
                Excerpts.Add(excerpt);
            }
        }
    }
}
