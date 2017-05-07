using System;
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
    }
}
