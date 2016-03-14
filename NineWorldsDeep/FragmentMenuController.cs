using NineWorldsDeep.Core;
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
            window.Menu.AddMenuItem("Fragments",
                                    "Extract By Keys",
                                    ExtractByKeys);
            window.Menu.AddMenuItem("Fragments",
                                    "Trim Prefix To New Key",
                                    TrimPrefixToNewKey);
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

                if (UI.Prompt.Confirm(output))
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
            List<Core.Fragment> lst = new List<Core.Fragment>();

            foreach(Core.Fragment f in window.GetFragments())
            {
                if (f.DisplayValue.Equals(selectedValue))
                {
                    lst.Add(f);
                }
            }

            window.Receive(lst);
        }

        private void ExtractByKeys(object sender, RoutedEventArgs e)
        {
            List<string> keys = new List<string>();
            List<Fragment> fragments = new List<Fragment>();

            string key = "";

            do
            {
                key = UI.Prompt.Input("Enter key to extract, or press enter to continue");
                if (!string.IsNullOrWhiteSpace(key))
                {
                    keys.Add(key);
                }

            } while (!string.IsNullOrWhiteSpace(key));

            bool keysVerified = true;

            foreach(Fragment f in window.GetFragments())
            {
                //break nested loop if key verification fails
                if (!keysVerified)
                {
                    break;
                }

                Fragment newFragment = null;
                
                foreach(string k in keys)
                {
                    string v = f.GetMeta(k);
                    if(v != null)
                    {
                        if(newFragment == null)
                        {
                            newFragment = new Fragment(k, v);
                        }
                        else
                        {
                            newFragment.SetMeta(k, v);
                        }

                    }else
                    {
                        keysVerified = false;
                        break;
                    }
                }

                fragments.Add(newFragment);
            }

            if (keysVerified)
            {
                window.Receive(fragments);
            }
            else
            {
                MessageBox.Show("keys could not be verified, one or more fragments missing specified keys");
            }
        }

        private void TrimPrefixToNewKey(object sender, RoutedEventArgs e)
        {
            string key = UI.Prompt.Input("Enter Key To Trim");
            if (!string.IsNullOrWhiteSpace(key))
            {
                string firstVal = window.GetFragments().First().GetMeta(key);
                if (!string.IsNullOrWhiteSpace(firstVal))
                {
                    string prefix = UI.Prompt.Input("Select/Enter prefix to trim", firstVal);
                    string newKey = UI.Prompt.Input("Enter new key to set");

                    if(!string.IsNullOrWhiteSpace(prefix) && !string.IsNullOrWhiteSpace(newKey))
                    {
                        foreach(Core.Fragment f in window.GetFragments())
                        {
                            string newVal = f.GetMeta(key);
                            if (newVal.StartsWith(prefix))
                            {
                                newVal = newVal.Remove(0, prefix.Length);
                            }
                            f.SetMeta(newKey, newVal);
                        }
                    }
                    else
                    {
                        MessageBox.Show("prefix and new key must be specified.");
                    }
                }
            }
        }

        private void LoadFromXml(object sender, RoutedEventArgs e)
        {
            XmlHandler xh = new XmlHandler();

            //TODO: replace hardcoded value with centralized configuration
            string path = UI.Prompt.ForXmlFileLoad(@"C:\NWD\fragments");
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
            string path = UI.Prompt.ForXmlFileSave(@"C:\NWD\fragments");
            if (path != null)
                xh.Save(
                    FragmentXmlAdapter.WrapAll(window.GetFragments()), path);
        }
        
        private void ReviewFlaggedFragments(object sender, RoutedEventArgs e)
        {
            IEnumerable<ReviewableFragment> reviewables =
                window.GetFragments().ToReviewables();

            foreach (ReviewableFragment f in reviewables)
            {                
                if (f.IsFlagged)
                {
                    if(f.KeyToReview != null)
                    {
                        if(UI.Prompt.Confirm(f.GetMeta(f.KeyToReview), true))
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

            window.Receive(reviewables);
        }
    }
}
