using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Core
{
    public class TaggingXmlWrapper : TaggingBase
    {
        public string MediaTagValue { get; private set; }

        public TaggingXmlWrapper(string tagValue)
        {
            MediaTagValue = tagValue;
        }
    }
}
