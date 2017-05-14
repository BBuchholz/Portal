using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Archivist
{
    public class ArchivistSourceExcerpt
    {
        public int SourceExcerptId { get; set; }
        public int SourceId { get; set; }
        public string ExcerptValue { get; set; }
        public string ExcerptPages { get; set; }
        public string ExcerptBeginTime { get; set; }
        public string ExcerptEndTime { get; set; }

        public string TagString { get; set; }
    }
}
