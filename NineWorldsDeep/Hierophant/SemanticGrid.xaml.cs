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
         * "Z" is a SemanticGrid (UI element), a SemanticGrid displays a SemanticMap
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

            RunListExample(dgridSemanticMapDisplayOne); //only works partially, is for list, generates columns with names but doesn't actually load values cuz dictionary...
            RunDictExample(dgridSemanticMapDisplayTwo); //modding this one
        }

        private void RunDictExample(DataGrid dgrid)
        {
            int count = 0;

            foreach (Dictionary<string, string> thisRow in AllRows.Values)
            {
                //asdsf;//this needs to change, count was for List<> in example
                if (thisRow.Keys.Count > count)
                {
                    for (int i = count; i < thisRow.Keys.Count; i++)
                    {
                        DataGridTextColumn col = new DataGridTextColumn();
                        col.Header = "testing " + i;
                        col.Binding = new Binding(string.Format("Value[{0}]", i)); //asdf;//this needs to change, count was for List<> in example
                        dgrid.Columns.Add(col);
                    }
                    count = thisRow.Keys.Count;
                }
            }
        }


        private void RunListExample(DataGrid dgrid)
        {
            int count = 0;

            foreach (Dictionary<string, string> thisRow in AllRows.Values)
            {
                if (thisRow.Keys.Count > count)
                {
                    for (int i = count; i < thisRow.Keys.Count; i++)
                    {
                        DataGridTextColumn col = new DataGridTextColumn();
                        col.Header = "testing " + i;
                        col.Binding = new Binding(string.Format("Value[{0}]", i)); 
                        dgrid.Columns.Add(col);
                    }
                    count = thisRow.Keys.Count;
                }
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
