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
    /// Interaction logic for SemanticGridPane.xaml
    /// </summary>
    public partial class SemanticGridPane : UserControl
    {
        public SemanticGridPane()
        {
            InitializeComponent();
        }

        public void DisplaySemanticMap(string semanticGroupName, SemanticMap semanticMap)
        {
            List<string> columnNames = new List<string>();

            dgrid.ItemsSource = semanticMap.AsDictionary();

            //get all keys in all semantic definition as one list (for column names)
            foreach (SemanticDefinition def in semanticMap.SemanticDefinitions)
            {
                foreach (string key in def.Keys)
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
    }
}
