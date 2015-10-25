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
        private MenuController _menu;

        public FragmentMetaWindow()
        {
            InitializeComponent();
            _menu = new MenuController();
            _menu.Configure(mainMenu);
            new FragmentMenuController().Configure(this);
            new WorkbenchController().Configure(this);
        }

        public Fragment Selected
        {
            get
            {
                return (Fragment)lvItems.SelectedItem;
            }
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

        public MenuController Menu { get { return _menu; } }

        [Obsolete("Please use FragmentMetaWindow.Menu.AddMenuItem")]
        public void AddMenuItem(MenuItem mi)
        {
            Menu.AddMenuItem("Misc", mi);
        }

        [Obsolete("Please use FragmentMetaWindow.Menu.AddMenuItem")]
        public void AddMenuItem(string header, RoutedEventHandler onClick)
        {
            Menu.AddMenuItem("Misc", header, onClick);
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
    }
}
