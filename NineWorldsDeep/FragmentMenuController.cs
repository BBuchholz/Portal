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
            window.Menu.AddMenuItem("Workbench",
                                    "Send To Workbench First",
                                    SendToWorkbenchFirst);
            window.Menu.AddMenuItem("Workbench",
                                    "Send To Workbench Last",
                                    SendToWorkbench);
            window.Menu.AddMenuItem("Meta",
                                    "Remove Selected Meta",
                                    RemoveSelectedMeta);
        }

        private void RemoveSelectedMeta(object sender, RoutedEventArgs e)
        {
            string output = "remove the following meta data?" +
                Environment.NewLine;
            Fragment f = window.GetSelectedFragment();
            if(f != null)
            {
                IEnumerable<KeyValuePair<string, string>> selectedMeta = window.GetSelectedMeta();
                foreach (KeyValuePair<string, string> kv in selectedMeta)
                {
                    output += Environment.NewLine + kv.Key + ": " + kv.Value;
                }

                if (Prompt.Confirm(output))
                {
                    foreach(KeyValuePair<string, string> kv in selectedMeta)
                    {
                        f.RemoveMeta(kv.Key);
                    }
                }
            }            
        }

        private void SendToWorkbench(object sender, RoutedEventArgs e)
        {
            WorkbenchWindow w = WorkbenchWindow.Instance;
            w.Receive(window.GetFragments().DeepCopy());
            w.Show();
        }

        private void SendToWorkbenchFirst(object sender, RoutedEventArgs e)
        {
            WorkbenchWindow w = WorkbenchWindow.Instance;
            w.ReceiveFirst(window.GetFragments().DeepCopy());
            w.Show();
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

            window.Receive(lst);
        }

        private void LoadFromXml(object sender, RoutedEventArgs e)
        {
            XmlHandler xh = new XmlHandler();

            //TODO: replace hardcoded value with centralized configuration
            string path = Prompt.ForXmlFileLoad(@"C:\NWD\fragments");
            if (path != null && File.Exists(path))
            {
                FragmentXmlAdapter template = new FragmentXmlAdapter(null);
                window.Receive(
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
                    if(f.KeyToReview != null)
                    {
                        if(Prompt.Confirm(f.GetMeta(f.KeyToReview), true))
                        {
                            f.ProcessReviewed();
                        }
                    }
                    else
                    {
                        MessageBox.Show(f.ToMultiLineString());
                    }
                }
            }
        }
    }
}
