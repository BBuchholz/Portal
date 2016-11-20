using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Model
{
    public class MediaDeviceModelItem
    {
        public int MediaDeviceId { get; set; }
        public string MediaDeviceDescription { get; set; }

        public override string ToString()
        {
            return MediaDeviceDescription;
        }
    }
}
