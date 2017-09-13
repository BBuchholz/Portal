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

namespace NineWorldsDeep.Hierophant
{
    /// <summary>
    /// Interaction logic for SemanticMatrix.xaml
    /// </summary>
    public partial class SemanticMatrix : UserControl
    {
        public SemanticMatrix()
        {
            InitializeComponent();
            CountForTesting = 0;
        }

        private int CountForTesting { get; set; }

        private void btnAddSemanticSet_Click(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = new TabItem();
            tabItem.Header = "A new semantic set " + CountForTesting;
            SemanticGrid semanticGrid = new SemanticGrid();
            tabItem.Content = semanticGrid;
            tcSemanticSets.Items.Add(tabItem);

            CountForTesting += 1;
        }
    }
}
