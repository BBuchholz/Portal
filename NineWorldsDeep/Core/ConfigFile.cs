using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Core
{
    class ConfigFile
    {
        #region private properties

        private string ConfigFilePath { get; set; }
        private Dictionary<string, string> ConfigLines { get; set; }

        #endregion

        #region constants

        private static string LOCAL_DEVICE_NAME = "localDeviceName";
        private static string INTAKE_VOICEMEMOS_PATH = "intakeVoicememosPath";
        private static string INTAKE_PDFS_PATH = "intakePdfsPath";
        private static string INTAKE_IMAGES_PATH = "intakeImagesPath";
        private static string TRASH_FOLDER_PATH = "trashFolderPath";

        #endregion

        #region creation

        public ConfigFile()
        {
            ConfigFilePath = Configuration.ConfigFilePath();
            ConfigLines = new Dictionary<string, string>();
            Load();
        }

        #endregion

        #region private helper methods

        private void Load()
        {
            if (!File.Exists(ConfigFilePath))
            {
                File.Create(ConfigFilePath);
            }

            foreach (string line in File.ReadLines(Configuration.ConfigFilePath()))
            {
                if (!line.Contains('='))
                {
                    throw new Exception("unrecognized line in " + ConfigFilePath + ": " + line);
                }

                var keyAndValueAsArray = line.Split(new char[] { '=' }, 2);

                ConfigLines.Add(keyAndValueAsArray[0], keyAndValueAsArray[1]);
            }
        }

        #endregion

        #region public interface

        public void Save()
        {
            using (StreamWriter file = new StreamWriter(ConfigFilePath))
            {
                foreach (var entry in ConfigLines)
                {
                    file.WriteLine("{0}={1}", entry.Key, entry.Value);
                }
            }
        }

        #endregion

        #region public properties

        public string LocalDeviceName
        {
            get
            {
                if (ConfigLines.ContainsKey(LOCAL_DEVICE_NAME))
                {
                    return ConfigLines[LOCAL_DEVICE_NAME];
                }

                return "";
            }

            set
            {
                ConfigLines[LOCAL_DEVICE_NAME] = value;
            }
        }

        public string IntakeVoicememosPath
        {
            get
            {
                if (ConfigLines.ContainsKey(INTAKE_VOICEMEMOS_PATH))
                {
                    return ConfigLines[INTAKE_VOICEMEMOS_PATH];
                }

                return "";
            }

            set
            {
                ConfigLines[INTAKE_VOICEMEMOS_PATH] = value;
            }
        }

        public string IntakePdfsPath
        {
            get
            {
                if (ConfigLines.ContainsKey(INTAKE_PDFS_PATH))
                {
                    return ConfigLines[INTAKE_PDFS_PATH];
                }

                return "";
            }

            set
            {
                ConfigLines[INTAKE_PDFS_PATH] = value;
            }
        }

        public string IntakeImagesPath
        {
            get
            {
                if (ConfigLines.ContainsKey(INTAKE_IMAGES_PATH))
                {
                    return ConfigLines[INTAKE_IMAGES_PATH];
                }

                return "";
            }

            set
            {
                ConfigLines[INTAKE_IMAGES_PATH] = value;
            }
        }

        public string TrashFolderPath
        {
            get
            {
                if (ConfigLines.ContainsKey(TRASH_FOLDER_PATH))
                {
                    return ConfigLines[TRASH_FOLDER_PATH];
                }

                return "";
            }

            set
            {
                ConfigLines[TRASH_FOLDER_PATH] = value;
            }
        }

        #endregion
    }
}
