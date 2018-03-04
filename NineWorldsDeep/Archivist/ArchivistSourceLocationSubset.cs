using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Archivist
{
    class ArchivistSourceLocationSubset
    {
        public int SourceLocationSubsetId { get; set; }
        public int SourceLocationId { get; set; }
        public string SourceLocationSubsetValue { get; set; }

        public override string ToString()
        {
            return SourceLocationSubsetValue;
        }
    }
}
