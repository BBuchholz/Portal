using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Archivist
{
    public class ArchivistSourceType
    {
        public int SourceTypeId { get; set; }
        public string SourceTypeValue { get; set; }

        public override string ToString()
        {
            return SourceTypeValue;
        }
    }
}
