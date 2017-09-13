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
        public SemanticGrid()
        {
            InitializeComponent();

            this.DataContext = this;

            //Mockup();
            MockupDGrid1(dgridSemanticMapDisplayOne);
            MockupDGrid2(dgridSemanticMapDisplayTwo);

            CountForTesting = 0;
        }

        private int CountForTesting { get; set; }
        
        private void MockupDGrid1(DataGrid dGrid)
        {
            DisplaySemanticMap(dGrid, UtilsHierophant.MockMap1());
        }

        private void MockupDGrid2(DataGrid dGrid)
        {
            DisplaySemanticMap(dGrid, UtilsHierophant.MockMap2());
        }

        private void DisplaySemanticMap(DataGrid dgrid, SemanticMap semanticMap)
        {
            List<string> columnNames = new List<string>();
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
            TabItem tabItem = new TabItem();
            tabItem.Header = "A new semantic set " + CountForTesting;
            DataGrid dataGrid = new DataGrid();
            dataGrid.AutoGenerateColumns = false;

            DataGridTextColumn textColumn = new DataGridTextColumn();
            textColumn.Binding = new Binding("Key");
            dataGrid.Columns.Add(textColumn);

            tabItem.Content = dataGrid;
            tcSemanticGroups.Items.Add(tabItem);

            CountForTesting += 1;
        }
    }
}
