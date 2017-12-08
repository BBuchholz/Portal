using NineWorldsDeep.Archivist;
using NineWorldsDeep.Db.Sqlite;
using NineWorldsDeep.Tapestry.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for ArchivistMasterDisplay.xaml
    /// </summary>
    public partial class ArchivistMasterDisplay : UserControl
    {
        #region fields

        private ArchivistSubsetDb db;

        //async related 
        private readonly SynchronizationContext syncContext;
        private DateTime previousTime = DateTime.Now;

        #endregion

        #region creation

        public ArchivistMasterDisplay()
        {
            InitializeComponent();
            syncContext = SynchronizationContext.Current;

            db = new ArchivistSubsetDb();
            LoadSourceTypes();
            LoadOrderBy();
        }

        #endregion

        #region properties

        private ArchivistSourceType SelectedSourceType
        {
            get
            {
                return (ArchivistSourceType)cmbSourceTypes.SelectedItem;
            }
        }

        private ArchivistSourceType SelectedSourceTypeForSourceAdd
        {
            get
            {
                return (ArchivistSourceType)cmbAddSourceSourceType.SelectedItem;
            }
        }

        #endregion

        #region event handlers

        private void btnImportTextFiles_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmbSourceTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SelectedSourceType != null)
            {
                LoadSources(SelectedSourceType.SourceTypeId);
            }
        }

        private void lvSources_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //mirrors SynergyV5MasterListDisplay
            ArchivistSource src = (ArchivistSource)lvSources.SelectedItem;

            if(src != null)
            {
                ArchivistSourceNode nd = new ArchivistSourceNode(src);

                SourceSelectedEventArgs args =
                    new SourceSelectedEventArgs(nd);

                OnSourceSelected(args);
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

        private void btnAddSourceType_Click(object sender, RoutedEventArgs e)
        {
            string typeName = UI.Prompt.Input("Enter Source Type Name");

            if (!string.IsNullOrWhiteSpace(typeName))
            {
                db.EnsureSourceType(typeName);
                UI.Display.Message("ensured type: " + typeName);
                LoadSourceTypes();
            }
            else
            {
                UI.Display.Message("aborted.");
            }
        }

        private void btnAddSource_Click(object sender, RoutedEventArgs e)
        {
            /*
             * Validate needed fields based on source type
             * 
             * Book needs title and author at least
             * Movie needs title at least
             * Website needs Url and Title at least
             * all others, need title at least
            */

            //var sType = (ArchivistSourceType)cmbAddSourceSourceType.SelectedItem;
            var sType = SelectedSourceTypeForSourceAdd;

            if (sType == null)
            {
                UI.Display.Message("source type not specified, aborting.");
                return;
            }

            string sourceTypeValue = sType.SourceTypeValue.ToLower();

            bool valid = false;

            switch (sourceTypeValue)
            {
                case "book":

                    valid = Validate(txtAddSourceTitle)
                        && Validate(txtAddSourceAuthor);

                    break;

                case "movie":

                    valid = Validate(txtAddSourceTitle)
                        && Validate(txtAddSourceDirector);

                    break;

                case "website":

                    valid = Validate(txtAddSourceTitle)
                        && Validate(txtAddSourceUrl);

                    break;

                default:

                    valid = Validate(txtAddSourceTitle);

                    break;
            }

            if (valid)
            {
                ArchivistSource src = new ArchivistSource()
                {
                    SourceTypeId = sType.SourceTypeId,
                    Title = txtAddSourceTitle.Text,
                    Author = txtAddSourceAuthor.Text,
                    Director = txtAddSourceDirector.Text,
                    Year = txtAddSourceYear.Text,
                    Url = txtAddSourceUrl.Text,
                    RetrievalDate = txtAddSourceRetrievalDate.Text
                };

                db.SyncCore(src);

                ClearSourceEntryFields();
                LoadSources(sType.SourceTypeId);
            }
        }

        private void cmbOrderBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var propertyName = cmbOrderBy.SelectedItem as string;
            
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvSources.ItemsSource);
            view.SortDescriptions.Clear();

            if (propertyName != null)
            {
                view.SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Ascending));
            }
        }

        #endregion
        
        #region SourceSelected event

        protected virtual void OnSourceSelected(SourceSelectedEventArgs args)
        {
            SourceSelected?.Invoke(this, args);
        }

        public event EventHandler<SourceSelectedEventArgs> SourceSelected;

        public class SourceSelectedEventArgs
        {
            public SourceSelectedEventArgs(ArchivistSourceNode sn)
            {
                SourceNode = sn;
            }

            public ArchivistSourceNode SourceNode { get; private set; }
        }

        #endregion

        #region private helper methods

        private void LoadOrderBy()
        {
            List<string> stringPropertiesNameList = new List<string>();

            foreach (var propertyInfo in typeof(ArchivistSource).GetProperties())
            {
                if(propertyInfo.PropertyType == typeof(string))
                {
                    if (!stringPropertiesNameList.Contains(propertyInfo.Name))
                    {
                        stringPropertiesNameList.Add(propertyInfo.Name);
                    }
                }
            }            

            cmbOrderBy.ItemsSource = stringPropertiesNameList;
        }

        private void LoadSourceTypes()
        {
            List<ArchivistSourceType> lst = db.GetAllSourceTypes();
            cmbSourceTypes.ItemsSource = lst;
            cmbAddSourceSourceType.ItemsSource = lst;
        }

        private void LoadSources(int sourceTypeId)
        {
            List<ArchivistSource> lst =
                db.GetSourceCoresForSourceTypeId(sourceTypeId);
            lvSources.ItemsSource = lst;
        }

        private void ClearSourceEntryFields()
        {
            txtAddSourceTitle.Text = "";
            txtAddSourceAuthor.Text = "";
            txtAddSourceDirector.Text = "";
            txtAddSourceYear.Text = "";
            txtAddSourceUrl.Text = "";
            txtAddSourceRetrievalDate.Text = "";
        }

        private bool Validate(TextBox txt)
        {
            if (txt.Text.Length == 0)
            {
                txt.Background = Brushes.Red;
                return false;
            }
            else
            {
                txt.Background = Brushes.White;
                return true;
            }
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

        #endregion

        #region public interface

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

        #endregion
        
    }
}
