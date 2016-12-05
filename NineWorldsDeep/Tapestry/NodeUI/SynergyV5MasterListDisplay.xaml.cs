﻿using NineWorldsDeep.Synergy.V5;
using NineWorldsDeep.Tapestry.Nodes;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for SynergyV5MasterListDisplay.xaml
    /// </summary>
    public partial class SynergyV5MasterListDisplay : UserControl
    {
        public SynergyV5MasterListDisplay()
        {
            InitializeComponent();

            Load();
        }

        private void Load()
        {
            List<SynergyV5ListNode> lst =
                new List<SynergyV5ListNode>();

            //needs to get all lists from database (SynergyV5SubsetDb.SelectAllActiveListsDeferredLoad())
            //add property SynergyV5List.IsLoaded
            //in ListDisplay, check if loaded, and if not, load before displaying
            

            //mock, two constructors
            lst.Add(new SynergyV5ListNode(new SynergyV5List("DemoList")));
            lst.Add(new SynergyV5ListNode("DemoList2"));

            lvSynergyV5Lists.ItemsSource = lst;
        }

        private void lvSynergyV5Lists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //mirrors ChordProgressionsNodeDisplay
            SynergyV5ListNode nd = (SynergyV5ListNode)lvSynergyV5Lists.SelectedItem;

            if(nd != null)
            {
                SynergyV5ListClickedEventArgs args =
                    new SynergyV5ListClickedEventArgs(nd);

                OnSynergyV5ListClicked(args);
            }
        }

        protected virtual void OnSynergyV5ListClicked(SynergyV5ListClickedEventArgs args)
        {
            SynergyV5ListClicked?.Invoke(this, args);
        }

        public event EventHandler<SynergyV5ListClickedEventArgs> SynergyV5ListClicked;

        public class SynergyV5ListClickedEventArgs
        {
            public SynergyV5ListClickedEventArgs(SynergyV5ListNode nd)
            {
                ListNode = nd;
            }

            public SynergyV5ListNode ListNode { get; private set; }

        }
    }

}
