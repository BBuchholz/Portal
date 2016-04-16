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
using System.Windows.Shapes;

namespace NineWorldsDeep.Tapestry
{
    /// <summary>
    /// Interaction logic for TapestryMainWindow.xaml
    /// </summary>
    public partial class TapestryMainWindow : Window
    {
        public TapestryMainWindow()
        {
            InitializeComponent();
        }

        private void MenuItemOriginalWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow(this);
            mw.Show();
        }

        private void MenuItemNwdFragmentCloud_Click(object sender, RoutedEventArgs e)
        {
            FragmentCloud.FragmentCloudMainWindow fcmw =
                new FragmentCloud.FragmentCloudMainWindow();

            fcmw.Show();
        }
    }
}
