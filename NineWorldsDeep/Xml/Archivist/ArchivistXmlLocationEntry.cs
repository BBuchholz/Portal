using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Xml.Archivist
{
    public class ArchivistXmlLocationEntry
    {
        public string Location { get; internal set; }
        public string LocationSubset { get; internal set; }
        public string LocationSubsetEntry { get; internal set; }
        public string VerifiedPresent { get; internal set; }
        public string VerifiedMissing { get; internal set; }
    }
}
