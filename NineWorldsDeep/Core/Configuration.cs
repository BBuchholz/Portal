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

        public static string AudacityWavExportsFolder { get { return @"C:\NWD-AUX\audacityWavExports"; } }

        public static string CubaseProjectsFolder { get { return @"C:\NWD-AUX\cubaseProjects"; } }
        public static string FLStudioProjectsFolder { get { return @"C:\NWD-AUX\flStudioProjects"; } }

        public static string ImagesFolder { get { return "c:\\NWD-AUX\\images"; } }

        public static string MySqlProjectsFolder
        {
            get
            {
                return @"C:\NWD\GERM\code\sql\";
            }
        }

        public static string PhoneSyncSynergyArchivedFolder
        {
            get
            {
                return @"C:\NWD-SYNC\phone\NWD\synergy\archived";
            }
        }

        public static string PhoneSyncSynergyFolder
        {
            get
            {
                return @"C:\NWD-SYNC\phone\NWD\synergy";
            }
        }

        public static string GetPhoneSyncSynergyFilePath(string listName)
        {
            return PhoneSyncSynergyFolder + "\\" + listName + ".txt";
        }

        public static string VisualStudioProjectsFolder
        {
            get
            {
                return @"C:\Users\Brent\Documents\Visual Studio 2015\Projects";
            }
        }

        public static string GetTagFilePath(string folderPath)
        {
            return folderPath + "\\fileTags.xml";
        }

        public static string VoiceMemosFolder
        {
            get
            {
                return @"C:\NWD-AUX\voicememos";
            }
        }

        public static string VoiceMemoTagFilePath
        {
            get
            {
                return VoiceMemosFolder + @"\fileTags.xml";
            }
        }
    }
}
