using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Core
{
    public class Configuration
    {
        //TODO: hard-coded values need to be transformed into config files and defaults

        public static string AbletonProjectsFolder { get { return @"C:\NWD-AUX\abletonProjects"; } }
        public static string AudacityProjectsFolder { get { return @"C:\NWD-AUX\audacityProjects"; } }
        public static string CubaseProjectsFolder { get { return @"C:\NWD-AUX\cubaseProjects"; } }
        public static string FLStudioProjectsFolder { get { return @"C:\NWD-AUX\flStudioProjects"; } }

        public static string MySqlProjectsFolder
        {
            get
            {
                return @"C:\NWD\GERM\code\sql\";
            }
        }

        public static string VisualStudioProjectsFolder
        {
            get
            {
                return @"C:\Users\Brent\Documents\Visual Studio 2015\Projects";
            }
        }
    }
}
