using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Archivist
{
    public class ArchivistSourceExcerptAnnotation
    {
        public int SourceExcerptAnnotationId { get; set; }
        public int SourceExcerptId { get; set; }
        public int SourceAnnotationId { get; set; }
        public string SourceAnnotationValue { get; set; }
    }
}
