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

        private void btnImportSemanticSets_Click(object sender, RoutedEventArgs e)
        {
            UI.Display.Message("Needs to import all Semantic Sets from xml");
        }

        private void btnExportSemanticSets_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<SemanticMap> maps = GetSemanticMaps();
            var fileName = UtilsHierophant.ExportToXml(maps);
            UI.Display.Message("semantic maps exported as " + fileName);
        }

        private IEnumerable<SemanticMap> GetSemanticMaps()
        {
            List<SemanticMap> maps = new List<SemanticMap>();

            foreach(SemanticGrid semanticGrid in
                semanticSetNamesToSemanticGrids.Values)
            {
                if(semanticGrid.CurrentSemanticMap != null)
                {
                    maps.Add(semanticGrid.CurrentSemanticMap);
                }
            }

            return maps;
        }
    }
}
