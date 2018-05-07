using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Xml.Archivist
{
    public class ArchivistXmlSourceExcerpt
    {
        public string Pages { get; internal set; }
        public string BeginTime { get; internal set; }
        public string EndTime { get; internal set; }
        public List<ArchivistXmlExcerptAnnotation> Annotations { get; private set; }
        public List<ArchivistXmlTag> Tags { get; private set; }
        public string ExcerptValue { get; internal set; }

        public ArchivistXmlSourceExcerpt()
        {
            Annotations = new List<ArchivistXmlExcerptAnnotation>();
            Tags = new List<ArchivistXmlTag>();
        }

        internal void Add(ArchivistXmlExcerptAnnotation annotation)
        {
            Annotations.Add(annotation);
        }

        internal void Add(ArchivistXmlTag tag)
        {
            Tags.Add(tag);
        }
    }
}
