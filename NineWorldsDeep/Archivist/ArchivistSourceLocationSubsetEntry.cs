using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Archivist
{
    public class ArchivistSourceLocationSubsetEntry : Core.VerifiablePresenceBase
    {
        public int SourceLocationSubsetEntryId { get; set; }
        public int SourceLocationSubsetId { get; set; }
        public int SourceId { get; set; }
        public string SourceLocationValue { get; set; }
        public string SourceLocationSubsetValue { get; set; }
        public string SourceLocationSubsetEntryValue { get; set; }

        public override string ToString()
        {
            return SourceLocationSubsetEntryValue;
        }
    }
}
