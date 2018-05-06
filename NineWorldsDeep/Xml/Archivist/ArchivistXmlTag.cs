using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Xml.Archivist
{
    public class ArchivistXmlTag
    {
        public string TagValue { get; internal set; }
        public string TaggedAt { get; internal set; }
        public string UntaggedAt { get; internal set; }
    }
}
