﻿using NineWorldsDeep.Core;
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
            WorkbenchWindowController.Instance.Configure(this);
        }

        public Fragment Selected
        {
            get
            {
                return (Fragment)lvItems.SelectedItem;
            }
        }
                
        public IEnumerable<Core.Fragment> GetFragments()
        {
            IEnumerable<Core.Fragment> ie = 
                (IEnumerable<Core.Fragment>)lvItems.ItemsSource;

            if(ie == null)
            {
                ie = new List<Core.Fragment>();
            }

            return ie;
        }

        public void Receive(IEnumerable<Core.Fragment> fragments)
        {
            lvItems.ItemsSource = fragments;
            lvDetail.ItemsSource = null;
            RefreshMetaKeys();
        }

        public Fragment GetSelectedFragment()
        {
            return (Fragment)lvItems.SelectedItem;
        }

        public IEnumerable<KeyValuePair<string,string>> GetSelectedMeta()
        {
            return (IEnumerable<KeyValuePair<string,string>>)lvDetail.SelectedItems;
        }

        public void RefreshMetaKeys()
        {
            cmbDisplayKey.ItemsSource = GetFragments().GetMetaKeys();
        }

        public void RefreshFragmentList()
        {
            IEnumerable<Core.Fragment> frgs = GetFragments();
            lvItems.ItemsSource = null;           
            lvItems.ItemsSource = frgs.OrderBy(s => s);
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
