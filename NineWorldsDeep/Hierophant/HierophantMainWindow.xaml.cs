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

namespace NineWorldsDeep.Hierophant
{
    /// <summary>
    /// Interaction logic for HierophantMainWindow.xaml
    /// </summary>
    public partial class HierophantMainWindow : Window
    {
        public HierophantMainWindow()
        {
            InitializeComponent();
        }

        private void MenuItemVisualTree_Click(object sender, RoutedEventArgs e)
        {
            VisualKabbalisticTreeTestHarness testHarness =
                new VisualKabbalisticTreeTestHarness();
            testHarness.Show();
        }

        private void MenuItemFileLoad_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemFileSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemDBLoad_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemDBSave_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
