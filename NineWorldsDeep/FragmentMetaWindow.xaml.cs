using System;
using System.Collections.Generic;
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
        }

        private void DemoFragments()
        {
            List<Fragment> lst = new List<Fragment>();

            for (int i = 1; i < 10; i++)
            {
                Fragment f = new Fragment("frg" + i);
                f.SetMeta("DemoKey", "demo value for frg" + i);
                lst.Add(f);
            }

            SetItemsSource(lst);
        }

        public void SetItemsSource(IEnumerable<Fragment> fragments)
        {
            lvItems.ItemsSource = fragments;
            lvDetail.ItemsSource = null;
            cmbDisplayKey.ItemsSource = GetMetaKeys(fragments);
        }

        public void RefreshFragmentList()
        {
            IEnumerable<Fragment> frgs = (IEnumerable<Fragment>)lvItems.ItemsSource;
            lvItems.ItemsSource = null;
            lvItems.ItemsSource = frgs;
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

        private void MenuItemDemo_Click(object sender, RoutedEventArgs e)
        {
            DemoFragments();
            DemoDynamicMenus();
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Testing");
        }

        private void DemoDynamicMenus()
        {
            AddMenuItem("Dynamic Menu Item", Test_Click);
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
    }
}
