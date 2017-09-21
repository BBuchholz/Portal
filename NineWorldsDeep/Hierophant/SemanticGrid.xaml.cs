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
    /// Interaction logic for SemanticGrid.xaml
    /// </summary>
    public partial class SemanticGrid : UserControl
    {
        private Dictionary<string, DataGrid> semanticGroupNamesToDataGrids =
            new Dictionary<string, DataGrid>();
        
        public SemanticGrid()
        {
            InitializeComponent();

            this.DataContext = this;
            
            CountForTesting = 0;
        }

        private int CountForTesting { get; set; }
        
        public void DisplaySemanticMap(SemanticMap semanticMap)
        {
            DisplaySemanticMap("[[ALL]]", semanticMap);

            foreach(string semanticGroupName in semanticMap.SemanticGroupNames)
            {
                DisplaySemanticMap(semanticGroupName, semanticMap.SemanticGroup(semanticGroupName));
            }
        }

        private void DisplaySemanticMap(string semanticGroupName, SemanticMap semanticMap)
        {
            List<string> columnNames = new List<string>();
            EnsureSemanticGroupGrid(semanticGroupName);
            var dgrid = semanticGroupNamesToDataGrids[semanticGroupName];
            dgrid.ItemsSource = semanticMap;

            //get all keys in all semantic definition as one list (for column names)
            foreach (SemanticDefinition def in semanticMap.Values)
            {
                foreach (string key in def.Keys)
                {
                    //store distinct keys (for column names)
                    if (!columnNames.Contains(key)) {

                        columnNames.Add(key);
                    }
                }
            }

            //add column for each columnName
            foreach (string colName in columnNames)
            {
                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = colName;
                col.Binding = new Binding(string.Format("Value[{0}]", colName));
                dgrid.Columns.Add(col);                 
            }
        } 
        
        private void btnAddSemanticGroup_Click(object sender, RoutedEventArgs e)
        {
            EnsureSemanticGroupGrid("Semantic Group " + CountForTesting);
            CountForTesting += 1;
        }

        private void EnsureSemanticGroupGrid(string semanticGroupName)
        {
            //prevent overwrite of existing groups
            if (!semanticGroupNamesToDataGrids.ContainsKey(semanticGroupName))
            {
                TabItem tabItem = new TabItem();
                tabItem.Header = semanticGroupName;
                DataGrid dataGrid = new DataGrid();
                dataGrid.AutoGenerateColumns = false;

                semanticGroupNamesToDataGrids[semanticGroupName] = dataGrid;

                DataGridTextColumn textColumn = new DataGridTextColumn();
                textColumn.Binding = new Binding("Key");
                dataGrid.Columns.Add(textColumn);

                tabItem.Content = dataGrid;
                tcSemanticGroups.Items.Add(tabItem);
            }
        }

        private void chkHighlightActiveGroup_checkToggled(object sender, RoutedEventArgs e)
        {
            bool? isChecked = chkHighlightActiveGroup.IsChecked;

            if (isChecked.HasValue && 
                isChecked.Value == true) //intentionally redundant for clarity
            {
                UI.Display.Message("do stuff here");
            }
        }
    }
}
