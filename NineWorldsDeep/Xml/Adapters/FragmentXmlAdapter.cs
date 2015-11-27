using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NineWorldsDeep.Xml.Adapters
{
    public class FragmentXmlAdapter : IXmlTransportable
    {
        private Core.Fragment fragment;

        public static IEnumerable<FragmentXmlAdapter> 
            WrapAll(IEnumerable<Core.Fragment> ie)
        {
            List<FragmentXmlAdapter> lst = new List<FragmentXmlAdapter>();

            foreach(Core.Fragment f in ie)
            {
                lst.Add(new FragmentXmlAdapter(f));
            }

            return lst;
        }

        public static IEnumerable<Core.Fragment> UnWrapAll(IEnumerable<IXmlTransportable> ie)
        {
            List<Core.Fragment> lst = new List<Core.Fragment>();

            foreach(IXmlTransportable ixt in ie)
            {
                FragmentXmlAdapter fxa = (FragmentXmlAdapter)ixt;
                lst.Add(fxa.fragment);
            }

            return lst;
        }

        public FragmentXmlAdapter(Core.Fragment f)
        {
            fragment = f;
        }

        public XName PluralizedTag
        {
            get
            {
                return "fragments";
            }
        }

        public XName SingularTag
        {
            get
            {
                return "fragment";
            }
        }

        public XElement ToXElement()
        {
            XElement el =
                new XElement(SingularTag,
                    fragment.Meta.Select(x => MetaToXElement(x))
                );

            return el;
        }

        private XElement MetaToXElement(KeyValuePair<string, string> kv)
        {
            XElement el =
                new XElement("meta",
                    new XElement("key", kv.Key),
                    new XElement("value", kv.Value)
                );

            return el;
        }

        public IXmlTransportable FromXElement(XElement x)
        {
            Core.Fragment f = null;

            foreach(var metaElement in x.Elements("meta"))
            {
                if (f == null) { 
                    f = new Core.Fragment(metaElement.Element("key").Value,
                                          metaElement.Element("value").Value);
                }

                f.SetMeta(metaElement.Element("key").Value,
                          metaElement.Element("value").Value);
            }

            return (IXmlTransportable)new FragmentXmlAdapter(f);
        }
    }
}
