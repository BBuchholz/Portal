using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Warehouse
{
    public static class Extensions
    {
        public static List<string> AllPaths(this IEnumerable<SyncMap> ie)
        {
            List<string> paths = new List<string>();

            foreach (SyncMap map in ie)
            {
                paths.Add(map.Source);
                paths.Add(map.Destination);
            }

            return paths;
        }
    }
}
