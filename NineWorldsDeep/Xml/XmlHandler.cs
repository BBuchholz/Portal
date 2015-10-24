using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NineWorldsDeep.Xml
{
    public class XmlHandler
    {
        public void Save(IEnumerable<IXmlTransportable> ie, string path)
        {
            if (path != null && ie.Count() > 0)
            {
                XDocument doc =
                    new XDocument(
                        new XElement(ie.First().PluralizedTag,
                            ie.Select(x => x.ToXElement())
                        )
                    );

                doc.Save(path);
            }
        }

        public IEnumerable<IXmlTransportable> Load(string path, 
                                                   IXmlTransportable template)
        {
            if (System.IO.File.Exists(path))
            {
                var xDoc = XDocument.Load(path);

                var transportables =
                    xDoc.Descendants(template.SingularTag)
                        .Select(x => template.FromXElement(x));

                return transportables;
            }

            return null;
        }
    }
}
