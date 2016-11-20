using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Model
{
    public class MediaRootModelItem
    {
        public int MediaRootId { get; set; }
        public int MediaDeviceId { get; set; }
        public string MediaRootPath { get; set; }

        public override string ToString()
        {
            return MediaRootPath;
        }
    }
}
