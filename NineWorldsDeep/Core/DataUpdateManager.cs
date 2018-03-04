using NineWorldsDeep.Tapestry.NodeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Core
{
    public class DataUpdateManager
    {
        private static List<ISourceExcerptDisplay> excerptDisplays;

        static DataUpdateManager()
        {
            excerptDisplays = new List<ISourceExcerptDisplay>();
        }

        public static void Register(ISourceExcerptDisplay sed)
        {
            if (!excerptDisplays.Contains(sed))
            {
                excerptDisplays.Add(sed);
            }
        }

        public static void UpdateSourceExcerptDisplays()
        {
            foreach(ISourceExcerptDisplay sed in excerptDisplays)
            {
                sed.RefreshSourceAndExcerptsFromDb();
            }
        }
    }
}
