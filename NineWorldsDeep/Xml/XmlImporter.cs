using NineWorldsDeep.Sqlite.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NineWorldsDeep.Xml
{
    public class XmlImporter
    {
        private XDocument doc;

        public XmlImporter(string filePath)
        {
            doc = XDocument.Load(filePath);
        }

        public List<LocalConfigModelItem> GetConfig()
        {
            List<LocalConfigModelItem> cfg =
                new List<LocalConfigModelItem>();

            foreach(XElement configItemEl in doc.Descendants("config-item"))
            {
                String cfgKey = configItemEl.Element("config-key").Value;
                String cfgVal = configItemEl.Element("config-value").Value;

                cfg.Add(new LocalConfigModelItem(cfgKey, cfgVal));
            }

            return cfg;
        }

        public List<FileModelItem> GetFiles()
        {
            List<FileModelItem> files =
                new List<FileModelItem>();

            foreach(XElement fileEl in doc.Descendants("file"))
            {
                String device = 
                    fileEl.Element("device").Value;
                String path =
                    fileEl.Element("path").Value;

                FileModelItem fmi =
                    new FileModelItem(device, path);

                XElement displayNameEl =
                    fileEl.Element("display-name");

                String displayName = null;

                if (displayNameEl != null)
                {
                    displayName = displayNameEl.Value;
                }
                
                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    fmi.SetDisplayName(displayName);
                }

                XElement audioTranscriptEl =
                    fileEl.Element("audio-transcript");

                String audioTranscript = null;

                if(audioTranscriptEl != null)
                {
                    audioTranscript = audioTranscriptEl.Value; 
                }                   

                if (!string.IsNullOrWhiteSpace(audioTranscript))
                {
                    fmi.SetAudioTranscript(audioTranscript);
                }

                foreach(XElement hashEl in fileEl.Descendants("hash"))
                {
                    String hashValue = 
                        hashEl.Attribute("hash").Value;
                    String hashedAt = 
                        hashEl.Attribute("hashedAt").Value;

                    fmi.GetHashes().Add(
                        new HashModelItem(hashValue, hashedAt));
                }

                foreach(XElement tagEl in fileEl.Descendants("tag"))
                {
                    String tag = tagEl.Value;

                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        fmi.GetTags().Add(tag);
                    }
                }

                files.Add(fmi);
            }

            return files;
        }
    }
}
