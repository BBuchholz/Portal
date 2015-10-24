using NineWorldsDeep.Xml;
using NineWorldsDeep.Xml.Adapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NineWorldsDeep
{
    /// <summary>
    /// Interaction logic for ListViewDetail.xaml
    /// </summary>
    public partial class FragmentMetaWindow : Window
    {
        public FragmentMetaWindow()
        {
            InitializeComponent();
            AddMenuItem("Review Flagged Fragments", ReviewFlaggedFragments);
            AddMenuItem("Save To Xml", SaveToXml);
            AddMenuItem("Load From Xml", LoadFromXml);
        }

        private void LoadFromXml(object sender, RoutedEventArgs e)
        {
            XmlHandler xh = new XmlHandler();

            //TODO: replace hardcoded value with centralized configuration
            string path = Prompt.ForXmlFileLoad(@"C:\NWD\fragments");
            if (path != null && File.Exists(path)) { 
                FragmentXmlAdapter template = new FragmentXmlAdapter(null);
                SetItemsSource(FragmentXmlAdapter.UnWrapAll(xh.Load(path, template)));
            }
        }

        private void SaveToXml(object sender, RoutedEventArgs e)
        {
            XmlHandler xh = new XmlHandler();
            
            //TODO: replace hardcoded value with centralized configuration
            string path = Prompt.ForXmlFileSave(@"C:\NWD\fragments");
            if(path != null)
                xh.Save(FragmentXmlAdapter.WrapAll(GetFragments()), path);
        }
        
        public IEnumerable<Fragment> GetFragments()
        {
            IEnumerable<Fragment> ie = 
                (IEnumerable<Fragment>)lvItems.ItemsSource;

            if(ie == null)
            {
                ie = new List<Fragment>();
            }

            return ie;
        }

        public void SetItemsSource(IEnumerable<Fragment> fragments)
        {
            lvItems.ItemsSource = fragments;
            lvDetail.ItemsSource = null;
            RefreshMetaKeys();
        }

        public void RefreshMetaKeys()
        {
            cmbDisplayKey.ItemsSource = GetMetaKeys(GetFragments());
        }

        public void RefreshFragmentList()
        {
            IEnumerable<Fragment> frgs = GetFragments();
            lvItems.ItemsSource = null;           
            lvItems.ItemsSource = frgs.OrderBy(s => s);
        }

        private IEnumerable<string> GetMetaKeys(IEnumerable<Fragment> fragments)
        {
            List<string> lst = new List<string>();

            foreach(Fragment f in fragments)
            {
                foreach(string key in f.MetaKeys)
                {
                    if (!lst.Contains(key))
                    {
                        lst.Add(key);
                    }
                }
            }

            return lst;
        }

        public void AddMenuItem(MenuItem mi)
        {
            menuItemOptions.Items.Add(mi);
        }

        public void AddMenuItem(string header, RoutedEventHandler onClick)
        {
            MenuItem mi = new MenuItem();
            mi.Header = header;
            mi.Click += onClick;
            AddMenuItem(mi);
        }

        private void lvItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Fragment frg = (Fragment)lvItems.SelectedItem;
            if(frg != null)
            {
                lvDetail.ItemsSource = frg.Meta;
            }
        }

        private void cmbDisplayKey_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = (string)cmbDisplayKey.SelectedItem;
            if(selected != null)
            {
                foreach (Fragment f in lvItems.Items)
                {
                    f.DisplayKey = selected;
                }
                RefreshFragmentList();
            }
        }

        private void ReviewFlaggedFragments(object sender, RoutedEventArgs e)
        {
            foreach (Fragment f in GetFragments())
            {
                if (f.IsFlagged)
                {
                    MessageBox.Show(f.ToMultiLineString());
                }
            }
        }
    }
}
