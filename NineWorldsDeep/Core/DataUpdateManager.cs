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
        private static List<ISourceListDisplay> sourceDisplays;

        static DataUpdateManager()
        {
            excerptDisplays = new List<ISourceExcerptDisplay>();
            sourceDisplays = new List<ISourceListDisplay>();
        }

        public static void Register(ISourceExcerptDisplay sed)
        {
            if (!excerptDisplays.Contains(sed))
            {
                excerptDisplays.Add(sed);
            }
        }
        
        public static void Register(ISourceListDisplay sld)
        {
            if (!sourceDisplays.Contains(sld))
            {
                sourceDisplays.Add(sld);
            }
        }

        public static void UpdateSourceExcerptDisplays()
        {
            foreach(ISourceExcerptDisplay sed in excerptDisplays)
            {
                sed.RefreshSourceAndExcerptsFromDb();
            }
        }

        public static void UpdateSourceListDisplays()
        {
            foreach(ISourceListDisplay sld in sourceDisplays)
            {
                sld.RefreshSourcesFromDb();
            }
        }
    }
}
