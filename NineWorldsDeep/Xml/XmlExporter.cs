using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Sqlite.Model;
using System.Xml.Linq;

namespace NineWorldsDeep.Xml
{
    public class XmlExporter
    {
        public void Export(string deviceName, List<LocalConfigModelItem> cfg, List<FileModelItem> files, string targetFile)
        {
            XDocument doc = 
                new XDocument(new XElement("nwd", 
                    new XElement("local-config",
                        new XAttribute("device", deviceName)),
                    new XElement("files")));

            if(cfg != null)
            {
                //TODO: implement this (LocalConfigModelItem export to XML)
            }

            if(files != null)
            {
                XElement filesEl = doc.Descendants("files").First();

                foreach (FileModelItem fmi in files)
                {
                    XElement fileEl = 
                        new XElement("file",
                            new XElement("device", fmi.GetDevice()),
                            new XElement("path"), fmi.GetPath());

                    if (!string.IsNullOrWhiteSpace(fmi.GetDisplayName()))
                    {
                        fileEl.Add(new XElement("display-name", fmi.GetDisplayName()));
                    }

                    XElement hashesEl =
                        new XElement("hashes",
                            from h in fmi.GetHashes()
                            select new XElement("hash",
                                new XAttribute("hash", h.GetHash()),
                                new XAttribute("hashedAt", h.GetHashedAt())));

                    fileEl.Add(hashesEl);

                    XElement tagsEl =
                        new XElement("tags",
                            from t in fmi.GetTags()
                            select new XElement("tag", t));

                    fileEl.Add(tagsEl);

                    filesEl.Add(fileEl);
                }
            }

            doc.Save(targetFile);
        }
    }
}
