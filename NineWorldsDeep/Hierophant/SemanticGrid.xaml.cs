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
        /*
         * SemanticMatrix Mockup With Underlying Model Definitions
         *
         * _____________________
         * |Z|Z|Z|Z|Z|Z|Z|Z| X |
         * |_|_|_|_|_|_|_|_|___|
         * |Z|Z|Z|Z|Z|Z|Z|Z| X |
         * |_|_|_|_|_|_|_|_|___|
         * |Z|Z|Z|Z|Z|Z|Z|Z| X |
         * |_|_|_|_|_|_|_|_|___|
         * |Z|Z|Z|Z|Z|Z|Z|Z|
         * |_|_|_|_|_|_|_|_|
         * |Z|Z|Z|Z|Z|Z|Z|Z|
         * |_|_|_|_|_|_|_|_|
         * |Z|Z|Z|Z|Z|Z|Z|Z|
         * |_|_|_|_|_|_|_|_|
         * | Y | Y | Y | 
         * |___|___|___|
         *
         * Above is a datagrid (Z) within a tab control (Y) within a tab control (X)
         * 
         * The entire ui control is a SemanticMatrix.
         *
         * Our model associates in the following manner:
         *
         * "X" is a SemanticSet (model item), a SemanticSet has one or more SemanticGroups
         *
         * "Y" is a SemanticGroup (model item), a SemanticGroup is 
         *      a named SemanticMap with an underlying commonality
         *
         * "Z" is a SemanticGrid (UI element), a SemanticGrid displays SemanticGroups
         *
         * A SemanticMap is a Dictionary<SemanticKey, Dictionary<string, string>>(), 
         *      it binds a SemanticKey to a Dictionary of Key/Value pairs (eg. "Planet":"Mercury", 
         *      "Alchemical Element":"Sulphur", "Zodiacal Sign":"Aries", &c.)
         *
         */


        private Dictionary<SemanticKey, Dictionary<string, string>> allRows;

        public Dictionary<SemanticKey, Dictionary<string, string>> AllRows
        {
            get { return allRows; }
            set { allRows = value; }
        }

        public SemanticGrid()
        {
            InitializeComponent();

            this.DataContext = this;

            //Mockup();
            MockupWithDictionary();
        }

        private void MockupWithDictionary()
        {
            //adapted from: https://stackoverflow.com/a/24361223/670768 

            AllRows = new Dictionary<SemanticKey, Dictionary<string, string>>();

            var row = new Dictionary<string, string>();

            row.Add("Planet", "Mercury");
            row.Add("Alchemical Element", "Sulphur");
            row.Add("Astrological Sign", "Aries");

            AllRows.Add(new SemanticKey("testKey"), row);

            RunDictExample(dgridSemanticMapDisplayOne); //modding this one
        }

        private void RunDictExample(DataGrid dgrid)
        {
            List<string> columnNames = new List<string>();

            //get all column names
            foreach (Dictionary<string, string> thisRow in AllRows.Values)
            {
                foreach (string key in thisRow.Keys)
                {
                    //store distinct keys (for column names)
                    if (!columnNames.Contains(key)) {

                        columnNames.Add(key);
                    }
                }
            }

            //add column for each name
            foreach (string colName in columnNames)
            {
                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = colName;
                col.Binding = new Binding(string.Format("Value[{0}]", colName));
                dgrid.Columns.Add(col);                 
            }
        } 
                

        private void Mockup()
        {
            List<ElementalsRow> elementals = new List<ElementalsRow>();
            
            elementals.Add(new ElementalsRow()
            {
                Path = "Chokmah-Geburah",
                Letter = "Heh",
                Sign = "Aries"
            });

            elementals.Add(new ElementalsRow()
            {
                Path = "Binah-Chesed",
                Letter = "Cheth",
                Sign = "Cancer"
            });
            
            dgridSemanticMapDisplayOne.ItemsSource = elementals;
            dgridSemanticMapDisplayTwo.ItemsSource = elementals;
            dgridSemanticMapDisplayThree.ItemsSource = elementals;
        }
    }
}
