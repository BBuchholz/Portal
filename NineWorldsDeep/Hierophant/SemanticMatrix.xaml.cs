using NineWorldsDeep.Core;
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
        #region fields

        private Dictionary<string, SemanticGrid> semanticSetNamesToSemanticGrids =
            new Dictionary<string, SemanticGrid>();

        #endregion
        
        #region creation

        public SemanticMatrix()
        {
            InitializeComponent();

            DisplaySemanticMap(UtilsHierophant.MockMapWithGroups("demo"));

            CountForUnnamedMaps = 0;
        }

        #endregion
        
        #region private properties

        private int CountForUnnamedMaps { get; set; }

        #endregion
        
        #region public interface
        
        public void DisplaySemanticMap(SemanticMap semanticMap)
        {
            if (string.IsNullOrWhiteSpace(semanticMap.Name))
            {
                semanticMap.Name = AutoGenerateSemanticMapName();
            }

            DisplaySemanticMap(semanticMap.Name, semanticMap);
        }
        
        public void DisplaySemanticMap(string semanticSetName, SemanticMap semanticMap)
        {
            EnsureSemanticGrid(semanticSetName);
            var grid = semanticSetNamesToSemanticGrids[semanticSetName];
            grid.DisplaySemanticMap(semanticMap);
        }

        public void AddAsGroupToSelectedSemanticSet(SemanticMap semanticMap)
        {
            SemanticGrid semGrid = tcSemanticSets.SelectedContent as SemanticGrid;

            if (semGrid != null)
            {
                semGrid.AddAsGroupToCurrentSemanticMap(semanticMap);
            }
        }

        #endregion
        
        #region private helper methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="incrementCount">set to false if you want to defer incrementing CountForUnnamedMaps</param>
        /// <returns></returns>
        private string AutoGenerateSemanticMapName(bool incrementCount = true)
        {
            var name = "Semantic Set " + CountForUnnamedMaps;

            if (incrementCount)
            {
                CountForUnnamedMaps++;
            }

            return name;

        }

        private IEnumerable<SemanticMap> GetSemanticMaps()
        {
            List<SemanticMap> maps = new List<SemanticMap>();

            foreach (SemanticGrid semanticGrid in
                semanticSetNamesToSemanticGrids.Values)
            {
                if (semanticGrid.CurrentSemanticMap != null)
                {
                    maps.Add(semanticGrid.CurrentSemanticMap);
                }
            }

            return maps;
        }
        
        private void EnsureSemanticGrid(string semanticSetName, bool selectEnsuredGrid = true)
        {
            //prevent overwrite of existing sets
            if (!semanticSetNamesToSemanticGrids.ContainsKey(semanticSetName))
            {
                TabItem tabItem = new TabItem();
                tabItem.Header = semanticSetName;
                SemanticGrid semanticGrid = new SemanticGrid();
                semanticGrid.AssignNewMap(semanticSetName);

                semanticSetNamesToSemanticGrids[semanticSetName] = semanticGrid;

                tabItem.Content = semanticGrid;
                tcSemanticSets.Items.Add(tabItem);

                if (selectEnsuredGrid)
                {
                    tcSemanticSets.SelectedItem = tabItem;
                }
            }
        }

        #endregion
        
        #region event handlers
        
        private void btnAddSemanticSet_Click(object sender, RoutedEventArgs e)
        {
            var defaultName = AutoGenerateSemanticMapName(false);
            var mapName = UI.Prompt.Input("Enter Name For Map", defaultName);

            if(!string.IsNullOrWhiteSpace(mapName))
            {
                if(mapName.Equals(defaultName, StringComparison.CurrentCultureIgnoreCase))
                {
                    CountForUnnamedMaps += 1;
                }
                
                EnsureSemanticGrid(mapName, true);
            }

        }

        private void btnImportSemanticSets_Click(object sender, RoutedEventArgs e)
        {
            var maps = UtilsHierophant.ImportXml();

            foreach(SemanticMap map in maps)
            {
                DisplaySemanticMap(map);
            }
        }

        private void btnExportSemanticSets_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<SemanticMap> maps = GetSemanticMaps();
            var fileName = UtilsHierophant.ExportToXml(maps);
            UI.Display.Message("semantic maps exported as " + fileName);
        }
        
        private void btnClearSemanticSets_Click(object sender, RoutedEventArgs e)
        {
            if (UI.Prompt.Confirm("Are you sure? This is non-reversible", true))
            {
                tcSemanticSets.Items.Clear();
                semanticSetNamesToSemanticGrids.Clear();
                CountForUnnamedMaps = 0;
            }
        }

        private void MenuItemChangeName_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                var tabItem = 
                    ((ContextMenu)menuItem.Parent).PlacementTarget as TabItem;
                
                if (tabItem != null)
                {
                    string header = tabItem.Header.ToString();
                    string newName = UI.Prompt.Input("Enter new name", header);

                    if (!string.IsNullOrWhiteSpace(newName) && !newName.Equals(header))
                    {
                        if (semanticSetNamesToSemanticGrids.ContainsKey(header))
                        {
                            SemanticGrid semGrid = semanticSetNamesToSemanticGrids[header];
                            semGrid.CurrentSemanticMap.Name = newName;

                            //set tabItem header to new name
                            if (tabItem != null)
                            {
                                tabItem.Header = newName;
                            }
                        }
                    }
                }
            }
        }

        private void MenuItemRemoveSet_Click(object sender, RoutedEventArgs e)
        {
            //looked it over, this should be all we need to do, however we choose to do it:
            //1)remove grid from semanticSetNamesToSemanticGrids
            //2)delete tab item (ui tools find parent?)

            MenuItem menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                var tabItem =
                    ((ContextMenu)menuItem.Parent).PlacementTarget as TabItem;

                if (tabItem != null)
                {
                    string semanticSetName = tabItem.Header.ToString();

                    if (semanticSetNamesToSemanticGrids.ContainsKey(semanticSetName))
                    {
                        semanticSetNamesToSemanticGrids.Remove(semanticSetName);
                    }

                    this.tcSemanticSets.Items.Remove(tabItem);
                }
            }
        }

        #endregion

    }
}
