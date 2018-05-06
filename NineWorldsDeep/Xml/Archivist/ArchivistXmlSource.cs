using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Xml.Archivist
{
    public class ArchivistXmlSource
    {
        public string SourceType { get; set; }
        public string Author { get; set; }
        public string Director { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string Url { get; set; }
        public string RetrievalDate { get; set; }
        public string SourceTag { get; set; }

        public List<ArchivistXmlLocationEntry> LocationEntries { get; private set; }
        public List<ArchivistXmlSourceExcerpt> Excerpts { get; private set; }

        public ArchivistXmlSource()
        {
            LocationEntries = new List<ArchivistXmlLocationEntry>();
            Excerpts = new List<ArchivistXmlSourceExcerpt>();
        }

        internal void Add(ArchivistXmlLocationEntry slse)
        {
            LocationEntries.Add(slse);
        }

        internal void Add(ArchivistXmlSourceExcerpt excerpt)
        {
            Excerpts.Add(excerpt);
        }
    }
}
