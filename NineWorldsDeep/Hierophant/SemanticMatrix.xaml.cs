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
        private Dictionary<string, SemanticGrid> semanticSetNamesToSemanticGrids =
            new Dictionary<string, SemanticGrid>();

        public SemanticMatrix()
        {
            InitializeComponent();

            DisplaySemanticMap(UtilsHierophant.MockMapWithGroups("demo"));

            CountForTesting = 0;
        }

        private int CountForTesting { get; set; }

        private void btnAddSemanticSet_Click(object sender, RoutedEventArgs e)
        {
            EnsureSemanticGrid("Semantic Set " + CountForTesting);
            CountForTesting += 1;
        }

        public void DisplaySemanticMap(SemanticMap semanticMap)
        {
            if(string.IsNullOrWhiteSpace(semanticMap.Name))
            {
                throw new Exception("Display of SemanticMap requires Name property to be set");
            }

            DisplaySemanticMap(semanticMap.Name, semanticMap);
        }

        public void DisplaySemanticMap(string semanticSetName, SemanticMap semanticMap)
        {
            EnsureSemanticGrid(semanticSetName);
            var grid = semanticSetNamesToSemanticGrids[semanticSetName];
            grid.DisplaySemanticMap(semanticMap);
        }

        private void EnsureSemanticGrid(string semanticSetName)
        {
            //prevent overwrite of existing sets
            if (!semanticSetNamesToSemanticGrids.ContainsKey(semanticSetName))
            {
                TabItem tabItem = new TabItem();
                tabItem.Header = semanticSetName;
                SemanticGrid semanticGrid = new SemanticGrid();

                semanticSetNamesToSemanticGrids[semanticSetName] = semanticGrid;

                tabItem.Content = semanticGrid;
                tcSemanticSets.Items.Add(tabItem);
            }
        }
    }
}
