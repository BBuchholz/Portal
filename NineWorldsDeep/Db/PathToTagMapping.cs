using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Db
{
    public class PathToTagMapping
    {
        public int PathId { get; set; }
        public int TagId { get; set; }
        public int DeviceId { get; set; }
        public int FileId { get; set; }
    }
}
