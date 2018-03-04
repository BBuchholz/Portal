using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Archivist
{
    class ArchivistSourceLocation
    {
        public int SourceLocationId { get; set; }
        public string SourceLocationValue { get; set; }

        public override string ToString()
        {
            return SourceLocationValue;
        }
    }
}
