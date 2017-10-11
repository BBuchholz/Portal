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

        private Dictionary<string, SemanticGridPane> semanticGroupsToPanes =
            new Dictionary<string, SemanticGridPane>();

        public SemanticMap CurrentSemanticMap { get; private set; }
        
        public SemanticGrid()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        //private int CountForTesting { get; set; }
        
        public void DisplaySemanticMap(SemanticMap semanticMap)
        {
            CurrentSemanticMap = semanticMap;

            DisplaySemanticMap(
                ConfigHierophant.ALL_KEYS_GROUP_NAME, 
                CurrentSemanticMap);

            foreach(string semanticGroupName in CurrentSemanticMap.SemanticGroupNames)
            {
                DisplaySemanticMap(semanticGroupName, CurrentSemanticMap.SemanticGroup(semanticGroupName));
            }
        }

        private void DisplaySemanticMap(string semanticGroupName, SemanticMap semanticMap)
        {
            EnsureSemanticGroupGridPane(semanticGroupName);
            var gridPane = semanticGroupsToPanes[semanticGroupName];

            gridPane.DisplaySemanticMap(semanticMap);            
        }
        
        public void DisplaySemanticMapToDataGrid(SemanticMap semanticMap)
        {
            CurrentSemanticMap = semanticMap;

            DisplaySemanticMap(
                ConfigHierophant.ALL_KEYS_GROUP_NAME, 
                CurrentSemanticMap);

            foreach (string semanticGroupName in CurrentSemanticMap.SemanticGroupNames)
            {
                DisplaySemanticMap(
                    semanticGroupName, 
                    CurrentSemanticMap.SemanticGroup(semanticGroupName));
            }
        }

        private void DisplaySemanticMapToDataGrid(string semanticGroupName, SemanticMap semanticMap)
        {
            List<string> columnNames = new List<string>();
            EnsureSemanticGroupGrid(semanticGroupName);
            var dgrid = semanticGroupNamesToDataGrids[semanticGroupName];
            dgrid.ItemsSource = semanticMap.AsDictionary();

            //get all keys in all semantic definition as one list (for column names)
            foreach (SemanticDefinition def in semanticMap.SemanticDefinitions)
            {
                foreach (string key in def.ColumnNames)
                {
                    //store distinct keys (for column names)
                    if (!columnNames.Contains(key))
                    {

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
            if(CurrentSemanticMap == null)
            {
                CurrentSemanticMap = new SemanticMap();
            }

            int i = 0;
            string autoGenName;

            do
            {
                i++;
                autoGenName = "Semantic Group " + i;
            }
            while (CurrentSemanticMap.HasGroup(autoGenName));

            var name = UI.Prompt.Input("enter a group name: ", autoGenName);
            
            if (!string.IsNullOrWhiteSpace(name))
            {
                //creates it if it doesn't exist
                CurrentSemanticMap.SemanticGroup(name);
                DisplaySemanticMap(CurrentSemanticMap);
            }
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
        
        private void EnsureSemanticGroupGridPane(string semanticGroupName)
        {
            //prevent overwrite of existing groups
            if (!semanticGroupsToPanes.ContainsKey(semanticGroupName))
            {
                TabItem tabItem = new TabItem();
                tabItem.Header = semanticGroupName;

                //DataGrid dataGrid = new DataGrid();
                //dataGrid.AutoGenerateColumns = false;

                //DataGridTextColumn textColumn = new DataGridTextColumn();
                //textColumn.Binding = new Binding("Key");
                //dataGrid.Columns.Add(textColumn);

                var gridPane = new SemanticGridPane();

                semanticGroupsToPanes[semanticGroupName] = gridPane;
                gridPane.GroupName = semanticGroupName;

                tabItem.Content = gridPane;
                tcSemanticGroups.Items.Add(tabItem);

                tcSemanticGroups.SelectedItem = tabItem;
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

        private void tcSemanticGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem selectedTab = e.AddedItems[0] as TabItem;  // Gets selected tab

            if (selectedTab != null)
            {
                SemanticGridPane pane =
                    Core.UtilsUi.FindChildren<SemanticGridPane>(selectedTab).First();

                if (pane != null)
                {
                    pane.RefreshFromMap();
                }
            }
        }
    }
}
