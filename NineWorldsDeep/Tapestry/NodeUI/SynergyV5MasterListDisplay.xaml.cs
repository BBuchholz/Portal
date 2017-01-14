using NineWorldsDeep.Synergy.V5;
using NineWorldsDeep.Tapestry.Nodes;
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

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for SynergyV5MasterListDisplay.xaml
    /// </summary>
    public partial class SynergyV5MasterListDisplay : UserControl
    {
        Db.Sqlite.SynergyV5SubsetDb db;

        public SynergyV5MasterListDisplay()
        {
            InitializeComponent();

            db = new Db.Sqlite.SynergyV5SubsetDb();

            Load();            
        }

        private void Load()
        {
            List<SynergyV5ListNode> lst =
                new List<SynergyV5ListNode>();
                        
            foreach(string listName in db.GetAllActiveListNames())
            {
                lst.Add(new SynergyV5ListNode(listName));
            }

            lvSynergyV5Lists.ItemsSource = null; //reset value
            lvSynergyV5Lists.ItemsSource = lst;
        }

        private void lvSynergyV5Lists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //mirrors ChordProgressionsNodeDisplay
            SynergyV5ListNode nd = (SynergyV5ListNode)lvSynergyV5Lists.SelectedItem;

            if(nd != null)
            {
                SynergyV5ListClickedEventArgs args =
                    new SynergyV5ListClickedEventArgs(nd);

                OnSynergyV5ListClicked(args);
            }
        }

        protected virtual void OnSynergyV5ListClicked(SynergyV5ListClickedEventArgs args)
        {
            SynergyV5ListClicked?.Invoke(this, args);
        }

        public event EventHandler<SynergyV5ListClickedEventArgs> SynergyV5ListClicked;

        public class SynergyV5ListClickedEventArgs
        {
            public SynergyV5ListClickedEventArgs(SynergyV5ListNode nd)
            {
                ListNode = nd;
            }

            public SynergyV5ListNode ListNode { get; private set; }

        }

        private void btnCreateList_Click(object sender, RoutedEventArgs e)
        {
            string listName = 
                Synergy.SynergyUtils.ProcessListName(txtListNameEntry.Text);

            if (!string.IsNullOrWhiteSpace(listName))
            {
                SynergyV5List synLst = new SynergyV5List(listName);
                synLst.Save(db);

                Load();

                txtListNameEntry.Text = "";
            }
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            ProcessExpanderState((Expander)sender);
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            ProcessExpanderState((Expander)sender);
        }

        /// <summary>
        /// manages grid rows to share space between multiple expanded expanders
        /// </summary>
        /// <param name="expander"></param>
        private void ProcessExpanderState(Expander expander)
        {
            Grid parent = FindAncestor<Grid>(expander);
            int rowIndex = Grid.GetRow(expander);

            if (parent.RowDefinitions.Count > rowIndex && rowIndex >= 0)
                parent.RowDefinitions[rowIndex].Height =
                    (expander.IsExpanded ? new GridLength(1, GridUnitType.Star) : GridLength.Auto);
        }

        public static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            // Need this call to avoid returning current object if it is the 
            // same type as parent we are looking for
            current = VisualTreeHelper.GetParent(current);

            while (current != null)
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            };
            return null;
        }

        private void btnImportXml_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExportXml_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
