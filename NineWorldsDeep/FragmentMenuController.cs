using NineWorldsDeep.Xml;
using NineWorldsDeep.Xml.Adapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NineWorldsDeep
{
    public class FragmentMenuController
    {
        private FragmentMetaWindow window;

        public void Configure(FragmentMetaWindow w)
        {
            window = w;
            window.Menu.AddMenuItem("Fragments", "Load From Xml", LoadFromXml);
            window.Menu.AddMenuItem("Fragments", "Save To Xml", SaveToXml);
            window.Menu.AddMenuItem("Fragments",
                                    "Review Flagged Fragments",
                                     ReviewFlaggedFragments);
            window.Menu.AddMenuItem("Fragments",
                                    "Filter By Selected DisplayKey Value",
                                    FilterBySelected);
        }

        private void FilterBySelected(object sender, RoutedEventArgs e)
        {
            string selectedValue = window.Selected.DisplayValue;
            List<Fragment> lst = new List<Fragment>();

            foreach(Fragment f in window.GetFragments())
            {
                if (f.DisplayValue.Equals(selectedValue))
                {
                    lst.Add(f);
                }
            }

            window.SetItemsSource(lst);
        }

        private void LoadFromXml(object sender, RoutedEventArgs e)
        {
            XmlHandler xh = new XmlHandler();

            //TODO: replace hardcoded value with centralized configuration
            string path = Prompt.ForXmlFileLoad(@"C:\NWD\fragments");
            if (path != null && File.Exists(path))
            {
                FragmentXmlAdapter template = new FragmentXmlAdapter(null);
                window.SetItemsSource(
                    FragmentXmlAdapter.UnWrapAll(xh.Load(path, template)));
            }
        }

        private void SaveToXml(object sender, RoutedEventArgs e)
        {
            XmlHandler xh = new XmlHandler();

            //TODO: replace hardcoded value with centralized configuration
            string path = Prompt.ForXmlFileSave(@"C:\NWD\fragments");
            if (path != null)
                xh.Save(
                    FragmentXmlAdapter.WrapAll(window.GetFragments()), path);
        }
        
        private void ReviewFlaggedFragments(object sender, RoutedEventArgs e)
        {
            foreach (Fragment f in window.GetFragments())
            {
                if (f.IsFlagged)
                {
                    MessageBox.Show(f.ToMultiLineString());
                }
            }
        }
    }
}
