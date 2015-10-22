using NineWorldsDeep.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace NineWorldsDeep
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Reflector reflector =
            new Reflector();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DisplayTypeList(reflector.Reflection1());
        }

        private void MenuItem2_Click(object sender, RoutedEventArgs e)
        {
            DisplayTypeList(reflector.Reflection2());
        }

        private void DisplayTypeList(IEnumerable<Type> ie)
        {
            lvMain.ItemsSource = ie;
        }

        private void MenuItemListViewDetail_Click(object sender, RoutedEventArgs e)
        {
            new FragmentMetaWindow().Show();
        }
    }
}
