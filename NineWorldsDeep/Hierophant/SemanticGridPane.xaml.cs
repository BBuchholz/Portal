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
        private SemanticMap CurrentSemanticMap { get; set; }

        public SemanticGridPane()
        {
            InitializeComponent();
        }

        public void RefreshFromMap()
        {
            dgrid.ItemsSource = null;

            if (CurrentSemanticMap != null)
            {
                List<string> columnNames = new List<string>();

                dgrid.ItemsSource = CurrentSemanticMap.AsDictionary();

                //get all keys in all semantic definition as one list (for column names)
                foreach (SemanticDefinition def in CurrentSemanticMap.SemanticDefinitions)
                {
                    foreach (string colName in def.ColumnNames)
                    {
                        //store distinct keys (for column names)
                        if (!columnNames.Contains(colName))
                        {
                            columnNames.Add(colName);
                        }
                    }
                }

                //add column for each columnName
                foreach (string colName in columnNames)
                {
                    if (!DataGridColumnHeaderExists(colName))
                    {
                        DataGridTextColumn col = new DataGridTextColumn();
                        col.Header = colName;
                        col.Binding = new Binding(string.Format("Value[{0}]", colName));
                        dgrid.Columns.Add(col);
                    }
                }
            }
        }

        public void DisplaySemanticMap(SemanticMap semanticMap)
        {
            CurrentSemanticMap = semanticMap;
            RefreshFromMap();

            //List<string> columnNames = new List<string>();

            //dgrid.ItemsSource = CurrentSemanticMap.AsDictionary();

            ////get all keys in all semantic definition as one list (for column names)
            //foreach (SemanticDefinition def in CurrentSemanticMap.SemanticDefinitions)
            //{
            //    foreach (string key in def.Keys)
            //    {
            //        //store distinct keys (for column names)
            //        if (!columnNames.Contains(key))
            //        {
            //            columnNames.Add(key);
            //        }
            //    }
            //}

            ////add column for each columnName
            //foreach (string colName in columnNames)
            //{
            //    if (!DataGridColumnHeaderExists(colName))
            //    {
            //        DataGridTextColumn col = new DataGridTextColumn();
            //        col.Header = colName;
            //        col.Binding = new Binding(string.Format("Value[{0}]", colName));
            //        dgrid.Columns.Add(col);
            //    }
            //}
        }

        private bool DataGridColumnHeaderExists(string header)
        {
            bool exists = false;

            foreach(var col in dgrid.Columns)
            {
                if (col.Header != null && 
                    col.Header.Equals(header))
                {
                    exists = true;
                }
            }
                
            return exists;
        }

        private void btnAddSemanticKey_Click(object sender, RoutedEventArgs e)
        {
            var keyText = txtAddSemanticKey.Text;

            if (!string.IsNullOrWhiteSpace(keyText))
            {
                var semanticKey = new SemanticKey(keyText);

                if (CurrentSemanticMap != null)
                {
                    
                    CurrentSemanticMap.Add(new SemanticDefinition(semanticKey));
                    txtAddSemanticKey.Text = "";
                    RefreshFromMap();
                }
                else
                {
                    UI.Display.Message("CurrentSemanticMap is null");
                }
            }
        }
    }
}
