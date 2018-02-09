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
        private string ConfigFilePath { get; set; }
        private Dictionary<string, string> ConfigLines { get; set; }

        private static string LOCAL_DEVICE_NAME = "localDeviceName";

        public ConfigFile()
        {
            ConfigFilePath = Configuration.ConfigFilePath();
            ConfigLines = new Dictionary<string, string>();
            Load();
        }

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
        
        private void Save()
        {
            using (StreamWriter file = new StreamWriter(ConfigFilePath))
            {
                foreach (var entry in ConfigLines)
                {
                    file.WriteLine("{0}={1}", entry.Key, entry.Value);
                }
            }
        }

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
                Save();
            }
        }
    }
}
