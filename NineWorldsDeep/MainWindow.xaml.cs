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
        private string @namespace = "NineWorldsDeep";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == @namespace && t.IsPublic && !t.IsSubclassOf(typeof(Application))
                    select t;
            q.ToList().ForEach(t => lvMain.Items.Add(t.Name));
        }

        private void MenuItem2_Click(object sender, RoutedEventArgs e)
        {
            AppDomain.CurrentDomain.GetAssemblies()
                                   .SelectMany(t => t.GetTypes())
                                   .Where(t => t.IsClass && t.Namespace == @namespace)
                                   .ToList()
                                   .ForEach(t => lvMain.Items.Add(t.Name));
        }
    }
}
