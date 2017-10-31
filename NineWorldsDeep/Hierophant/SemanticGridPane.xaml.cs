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
        List<string> columnNames = new List<string>();

        #region properties

        public SemanticMap CurrentSemanticMap { get; private set; }
        public string GroupName { get; set; }

        #endregion

        #region creation

        public SemanticGridPane()
        {
            InitializeComponent();
        }

        #endregion

        #region public interface

        public void RefreshFromMap()
        {
            dgrid.ItemsSource = null;

            if (CurrentSemanticMap != null)
            {
                
                /* 
                 * 
                 * TODO: mod here to speed up column load
                 * 
                 * move column names to a property and if columnNames doesn't contain, add to newColumns, then just cycle
                 * 
                 * 
                 * NB: currently learning MVVM and I'm nearly certain it will make this a non-issue, so
                 * feel free to leave this as is for now, it's really slow but it works and
                 * we will replace it in the future
                 * 
                 */

                List<string> newColumns = new List<string>();

                dgrid.ItemsSource = CurrentSemanticMap.AsDictionary().ToList();

                //get all keys in all semantic definition as one list (for column names)
                foreach (SemanticDefinition def in CurrentSemanticMap.SemanticDefinitions)
                {
                    foreach (string colName in def.ColumnNames)
                    {
                        //store distinct keys (for column names)
                        if (!columnNames.Contains(colName))
                        {
                            newColumns.Add(colName);
                        }
                    }
                }

                //add column for each columnName
                foreach (string colName in newColumns)
                {
                    //if (!DataGridColumnHeaderExists(colName))
                    //{
                    //    DataGridTextColumn col = new DataGridTextColumn();
                    //    col.Header = colName;
                    //    col.Binding = new Binding(string.Format("Value[{0}]", colName));
                    //    dgrid.Columns.Add(col);
                    //}

                    AddColumn(colName);                
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

        #endregion

        #region private helper methods

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
        
        /// <summary>
        /// checks where column already exists, and ignores if column name is null or whitespace
        /// </summary>
        /// <param name="columnName"></param>
        private void AddColumn(string columnName)
        {
            if (!DataGridColumnHeaderExists(columnName) &&
                !string.IsNullOrWhiteSpace(columnName))
            {
                columnNames.Add(columnName);
                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = columnName;
                col.Binding = new Binding(string.Format("Value[{0}]", columnName));
                dgrid.Columns.Add(col);
            }
        }

        #endregion

        #region event handlers

        private void btnAddColumn_Click(object sender, RoutedEventArgs e)
        {
            AddColumn(txtAddColumn.Text);
        }

        private void btnAddSemanticKey_Click(object sender, RoutedEventArgs e)
        {
            var keyText = txtAddSemanticKey.Text;

            if (!string.IsNullOrWhiteSpace(keyText))
            {
                var semanticKey = new SemanticKey(keyText);

                if (CurrentSemanticMap != null)
                {
                    if (UtilsHierophant.IsAllKeysGroup(GroupName))
                    {
                        CurrentSemanticMap.Add(new SemanticDefinition(semanticKey));
                    }
                    else
                    {
                        CurrentSemanticMap.AddTo(GroupName, new SemanticDefinition(semanticKey));
                    }
                    
                    txtAddSemanticKey.Text = "";
                    RefreshFromMap();
                }
                else
                {
                    UI.Display.Message("CurrentSemanticMap is null");
                }
            }
        }

        #endregion

    }
}
