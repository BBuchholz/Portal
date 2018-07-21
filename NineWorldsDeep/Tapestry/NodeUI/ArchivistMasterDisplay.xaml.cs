using NineWorldsDeep.Archivist;
using NineWorldsDeep.Core;
using NineWorldsDeep.Db.Sqlite;
using NineWorldsDeep.Tapestry.Nodes;
using NineWorldsDeep.Xml.Archivist;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Xml.Linq;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for ArchivistMasterDisplay.xaml
    /// </summary>
    public partial class ArchivistMasterDisplay : UserControl, IAsyncStatusResponsive, ISourceListDisplay
    {
        #region fields

        //////////////////////////////////////////////////////////////
        bool testing = false; // toggle this for testing
        //////////////////////////////////////////////////////////////

        private ArchivistSubsetDb db;
        private Xml.Xml xml;

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
            RefreshSourceLocations();

            Core.DataUpdateManager.Register(this);
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
        
        private async void ButtonExportXml_Click(object sender, RoutedEventArgs e)
        {
            ///////////////////////////////////////////////////////////////////
            //mimics MediaMasterDisplay.btnExportXml_Click()
            ///////////////////////////////////////////////////////////////////

            try
            {
                await Task.Run(() =>
                {

                    string detail = "starting export of mnemosyne subset";

                    StatusDetailUpdate(detail);

                    List<ArchivistSource> allSources = db.GetAllSources();

                    XElement archivistSubsetEl = new XElement(Xml.Xml.TAG_ARCHIVIST_SUBSET);

                    int totalSources = allSources.Count;
                    int sourceCount = 0;

                    Dictionary<int, string> sourceTypeIdsToValues = new Dictionary<int, string>();

                    foreach(var sourceType in db.GetAllSourceTypes())
                    {
                        sourceTypeIdsToValues[sourceType.SourceTypeId] = sourceType.SourceTypeValue;
                    }

                    foreach (var source in allSources)
                    {
                        sourceCount++;


                        /////////////////////////////////////////////////////////
                        // will only export a sampling items 
                        // leave this code here, you want this for testing
                        //
                        if (testing && sourceCount > 2)
                        {
                            break;
                        }
                        //////////////////////////////////////////////////////////



                        //create media tag with attribute set for hash
                        XElement sourceEl = Xml.Xml.CreateSourceElement(source, sourceTypeIdsToValues[source.SourceTypeId]);

                        archivistSubsetEl.Add(sourceEl);

                        detail = "source " + sourceCount + " of " +
                            totalSources + ": " + source.ShortName + " processing locations";

                        StatusDetailUpdate(detail);

                        List<ArchivistSourceLocationSubsetEntry> locationEntries =
                            db.GetSourceLocationSubsetEntriesForSourceId(source.SourceId, false);

                        foreach(var locationEntry in locationEntries)
                        {
                            sourceEl.Add(
                                Xml.Xml.CreateSourceLocationSubsetEntryElement(locationEntry));
                        }

                        detail = "source " + sourceCount + " of " +
                            totalSources + ": " + source.ShortName + " processing excerpts";
                        StatusDetailUpdate(detail);

                        List<ArchivistSourceExcerpt> sourceExcerpts =
                            db.GetSourceExcerptsForSourceId(source.SourceId);
                        
                        int totalSourceExcerpts = sourceExcerpts.Count;
                        int sourceExcerptCount = 0;

                        foreach (var sourceExcerpt in sourceExcerpts)
                        {
                            sourceExcerptCount++;
                            
                            detail = "source " + sourceCount + " of " +
                                totalSources + ": " + source.ShortName + 
                                " : processing excerpt " + sourceExcerptCount +
                                " of " + totalSourceExcerpts;
                            StatusDetailUpdate(detail);

                            var sourceExcerptEl = 
                                Xml.Xml.CreateSourceExcerptElement(sourceExcerpt);

                            sourceEl.Add(sourceExcerptEl);
                            
                            sourceExcerptEl.Add(
                                Xml.Xml.CreateSourceExcerptValueElement(sourceExcerpt.ExcerptValue));

                            List<ArchivistSourceExcerptAnnotation> excerptAnnotations =
                                db.GetSourceExcerptAnnotationsBySourceExcerptId(sourceExcerpt.SourceExcerptId);

                            foreach(var annotation in excerptAnnotations)
                            {
                                sourceExcerptEl.Add(
                                    Xml.Xml.CreateSourceExcerptAnnotationElement(annotation));
                            }

                            List<TaggingXmlWrapper> taggings =
                                db.GetSourceExcerptTaggingXmlWrappersForExcerptId(sourceExcerpt.SourceExcerptId);

                            foreach(var tagging in taggings)
                            {
                                sourceExcerptEl.Add(
                                    Xml.Xml.CreateSourceExcerptTagElement(tagging));
                            }
                        }                       
                        
                    }

                    XDocument doc =
                        new XDocument(
                            new XElement("nwd", archivistSubsetEl));

                    //here, take doc and save to all sync locations            
                    string fileName =
                        NwdUtils.GetTimeStamp_yyyyMMddHHmmss() + "-" + ConfigArchivist.ARCHIVIST_V5_SUFFIX + ".xml";


                    //COPIED FROM TaggedMediaDisplay to use new Hive utilities
                    //write to temp file
                    var tempFolder = Configuration.TempV5XmlFolder;
                    var xmlTempFilePath =
                        System.IO.Path.Combine(tempFolder, fileName);

                    doc.Save(xmlTempFilePath);

                    var filePathInList = new List<string>();
                    filePathInList.Add(xmlTempFilePath);

                    if (testing)
                    {
                        Hive.UtilsHive.CopyToTestRoot(filePathInList);
                    }
                    else
                    {
                        Hive.UtilsHive.CopyToAllActiveRoots(filePathInList);
                    }
                    
                });

                tbStatusUtilities.Text = "finished.";
            }
            catch (Exception ex)
            {
                tbStatusUtilities.Text = "Error: " + ex.Message;
            }
        }

        private async void ButtonImportXml_Click(object sender, RoutedEventArgs e)
        {
            List<string> allPaths;

            if (testing)
            {
                allPaths = Hive.ConfigHive.TestingGetHiveArchivistXmlImportFilePaths();
            }
            else
            {
                allPaths = Hive.ConfigHive.GetHiveArchivistXmlImportFilePaths();
            }
            
            var count = 0;
            var total = allPaths.Count();

            try
            {
                await Task.Run(() =>
                {
                    string detail = "starting import of archivist xml files";
                    StatusDetailUpdate(detail);

                    foreach(string path in allPaths)
                    {
                        count++;

                        if (!string.IsNullOrWhiteSpace(path))
                        {

                            string fileName = System.IO.Path.GetFileName(path);

                            List<ArchivistXmlSource> allSources =
                                Xml.Xml.RetrieveArchivistSourcesWithReaderAsync(path, this);

                            string prefix = "file " + count + " of " + total;
                            prefix += " -> ";

                            db.SaveAsync(allSources, this, prefix);

                            if (!testing)
                            {
                                File.Delete(path);
                            }
                        }
                    }

                });
            }
            catch (Exception ex)
            {
                tbStatusUtilities.Text = "Error: " + ex.Message;
            }


            tbStatusUtilities.Text = "finished importing archivist xml files";
        }

        private void AddSourceLocation_Click(object sender, RoutedEventArgs e)
        {
            //mimics add source type
            string locationName = txtAddSourceLocationName.Text;

            if (string.IsNullOrWhiteSpace(locationName))
            {
                UI.Display.Message("Name cannot be empty");
                return;
            }

            try
            {

                db.EnsureSourceLocation(locationName);
                UI.Display.Message("ensured location: " + locationName);
                txtAddSourceLocationName.Text = "";
                RefreshSourceLocations();

            }
            catch(Exception ex)
            {
                UI.Display.Message("error ensuring source location: " + ex.Message);
            }
        }

        private void AddSourceLocationSubset_Click(object sender, RoutedEventArgs e)
        {
            var subsetName = txtAddSourceLocationSubsetName.Text;

            if (string.IsNullOrWhiteSpace(subsetName))
            {
                UI.Display.Message("subset name cannot be empty");
                return;
            }

            if(cmbAddSourceLocationSubsetSourceLocation.SelectedItem
                is ArchivistSourceLocation sourceLocation)
            {
                try
                {
                    db.EnsureSourceLocationSubset(
                        sourceLocation.SourceLocationId, subsetName);

                    txtAddSourceLocationSubsetName.Text = "";
                }
                catch(Exception ex)
                {
                    UI.Display.Message("error ensuring subset: " + ex.Message);
                }
            }
            else
            {
                UI.Display.Message("select source location");
            }
        }

        private void RefreshSources_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedSourceType == null)
            {
                UI.Display.Message("Select a source type...");
                return;
            }

            LoadSources();
        }

        private void btnImportTextFiles_Click(object sender, RoutedEventArgs e)
        {
            UI.Display.Message("awaiting implementation");
        }

        private void cmbSourceTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSources();
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
        
        private void AddSourceType_Click(object sender, RoutedEventArgs e)
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

        private void AddSource_Click(object sender, RoutedEventArgs e)
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

        private void OrderBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvSources.ItemsSource);
            view.SortDescriptions.Clear();

            if (cmbOrderBy.SelectedItem is string propertyName)
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

        public void StatusDetailUpdate(string text, bool ensureDisplay = false)
        {
            var currentTime = DateTime.Now;

            if (!ensureDisplay && ((DateTime.Now - previousTime).Milliseconds <= 50)) return;

            syncContext.Post(new SendOrPostCallback(s =>
            {
                tbStatusUtilities.Text = (string)s;
            }), text);

            previousTime = currentTime;
        }

        private void RefreshSourceLocations()
        {
            List<ArchivistSourceLocation> lst = db.GetAllSourceLocations();

            cmbAddSourceLocationSubsetSourceLocation.ItemsSource = null;
            cmbAddSourceLocationSubsetSourceLocation.ItemsSource = lst;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Core.UtilsUi.ProcessExpanderState((Expander)sender);
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            Core.UtilsUi.ProcessExpanderState((Expander)sender);
        }

        private void LoadSources()
        {
            lvSources.ItemsSource = null;

            if (SelectedSourceType != null)
            {
                LoadSources(SelectedSourceType.SourceTypeId);
            }
        }

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

            cmbSourceTypes.ItemsSource = null;
            cmbAddSourceSourceType.ItemsSource = null;

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
        
        ///// <summary>
        ///// manages grid rows to share space between multiple expanded expanders
        ///// </summary>
        ///// <param name="expander"></param>
        //private void ProcessExpanderState(Expander expander)
        //{
        //    Grid parent = FindAncestor<Grid>(expander);
        //    int rowIndex = Grid.GetRow(expander);

        //    if (parent.RowDefinitions.Count > rowIndex && rowIndex >= 0)
        //        parent.RowDefinitions[rowIndex].Height =
        //            (expander.IsExpanded ? new GridLength(1, GridUnitType.Star) : GridLength.Auto);
        //}

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

        public void RefreshSourcesFromDb()
        {
            LoadSources();
        }

        #endregion

    }
}
