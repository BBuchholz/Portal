using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NineWorldsDeep.Xml
{
    public interface IXmlTransportable
    {
        XName PluralizedTag { get; }
        XName SingularTag { get; }
        XElement ToXElement();
        IXmlTransportable FromXElement(XElement x);
    }
}
