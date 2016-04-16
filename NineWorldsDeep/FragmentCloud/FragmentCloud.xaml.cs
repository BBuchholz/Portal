﻿using System;
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

namespace NineWorldsDeep.FragmentCloud
{
    /// <summary>
    /// Interaction logic for FragmentCloud.xaml
    /// </summary>
    public partial class FragmentCloud : UserControl
    {
        Dictionary<Button, Fragment> mappings =
            new Dictionary<Button, Fragment>();

        public FragmentCloud()
        {
            InitializeComponent();
        }

        public void Display(Fragment frg)
        {
            ClearMappings();

            List<ElementCloudItem> lst =
                new List<ElementCloudItem>();

            foreach (Fragment f in frg.Children)
            {
                lst.Add(ConvertToEci(f));
            }

            RestartCloud(lst);
        }

        private void ClearMappings()
        {
            mappings.Clear();
        }

        private void RestartCloud(List<ElementCloudItem> lst)
        {
            elementCloud.Stop();
            elementCloud = new ElementCloud();
            grid.Children.Clear();
            grid.Children.Add(elementCloud);
            elementCloud.ElementsCollection = lst;
            elementCloud.Run();
        }

        private ElementCloudItem ConvertToEci(Fragment f)
        {
            ElementCloudItem eci = new ElementCloudItem();
            eci.Height = 60;
            eci.MinWidth = 60;
            eci.Width = double.NaN; //equivalent to XAML Width="Auto"
            Button btn = new Button();
            TextBlock tb = new TextBlock();
            tb.Text = f.GetShortName();
            tb.TextWrapping = TextWrapping.WrapWithOverflow;
            tb.Height = double.NaN; //equivalent to XAML Height="Auto"
            btn.Content = tb;
            btn.Click += Button_Click;
            eci.Children.Add(btn);
            mappings[btn] = f;
            return eci;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            if (btn != null && mappings.ContainsKey(btn))
            {
                Fragment f = mappings[btn];
                FragmentClickedEventArgs args =
                    new FragmentClickedEventArgs(f);
                OnFragmentClicked(args);
            }
        }

        protected virtual void OnFragmentClicked(FragmentClickedEventArgs e)
        {
            EventHandler<FragmentClickedEventArgs> handler = FragmentClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<FragmentClickedEventArgs> FragmentClicked;

    }
}
