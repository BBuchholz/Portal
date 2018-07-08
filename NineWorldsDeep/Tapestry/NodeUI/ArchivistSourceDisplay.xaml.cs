using NineWorldsDeep.Archivist;
using NineWorldsDeep.Tapestry.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for ArchivistSourceDisplay.xaml
    /// </summary>
    public partial class ArchivistSourceDisplay : UserControl, ISourceExcerptDisplay
    {
        #region fields

        private Db.Sqlite.ArchivistSubsetDb db;
        //private ArchivistSourceNode sourceNode;
        private ArchivistSource source;

        #endregion

        #region creation

        public ArchivistSourceDisplay()
        {
            InitializeComponent();
            db = new Db.Sqlite.ArchivistSubsetDb();
            Core.DataUpdateManager.Register(this);
        }

        #endregion

        #region public interface 

        public void Display(ArchivistSourceNode src)
        {
            //this.sourceNode = src;
            if(src != null)
            {
                this.source = src.Source;
                expanderSourceDetails.Header = this.source.ShortName;
            }
            RefreshSourceAndExcerptsFromDb();
            RefreshSourceLocationsFromDb();
            RefreshSourceLocationSubsetEntriesFromDb();
        }

        public void RefreshSourceAndExcerptsFromDb()
        {
            //if (this.sourceNode != null && this.sourceNode.Source != null)
            //{
            //    db.LoadSourceExcerptsWithTags(this.sourceNode.Source);
            //    RefreshFromObject();
            //}
            if (this.source != null && this.source.SourceId > 0)
            {
                //some components load a partial source node, need to refresh actual source from id as well
                this.source = db.GetSourceById(this.source.SourceId);

                db.LoadSourceExcerptsWithTaggedTags(this.source);
                RefreshSourceAndExcerptsFromObject();
            }
            else
            {
                UI.Display.Message("can't refresh if source is null or source id isn't set");
            }
        }

        #endregion

        #region private helper methods

        private void RefreshSourceLocationsFromDb()
        {
            List<ArchivistSourceLocation> lst = db.GetAllSourceLocations();

            cmbSourceLocations.ItemsSource = null;
            cmbSourceLocations.ItemsSource = lst;
        }

        private void RefreshSourceLocationSubsetEntriesFromDb()
        {
            if(source == null || source.SourceId < 1)
            {
                UI.Display.Message("source not set or source id less than 1");
                return;
            }

            List<ArchivistSourceLocationSubsetEntry> lst = 
                db.GetSourceLocationSubsetEntriesForSourceId(
                    source.SourceId, 
                    chkFilterExcludeMissingLocationEntries.IsChecked.Value);
           
            lvSourceLocationEntries.ItemsSource = null;
            lvSourceLocationEntries.ItemsSource = lst;
        }

        private void RefreshSourceLocationSubsetsForSelectedLocation()
        {
            cmbLocationSubsets.ItemsSource = null;

            if (cmbSourceLocations.SelectedItem is ArchivistSourceLocation location)
            {
                List<ArchivistSourceLocationSubset> subsets =
                    db.GetSourceLocationSubsetsForLocationId(location.SourceLocationId);

                cmbLocationSubsets.ItemsSource = subsets;
            }
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Core.UtilsUi.ProcessExpanderState((Expander)sender);
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            Core.UtilsUi.ProcessExpanderState((Expander)sender);
        }

        private void RefreshSourceAndExcerptsFromObject()
        {
            //ccSourceDetails.Content = sourceNode.Source;

            //lvSourceExcerpts.ItemsSource = null;
            //lvSourceExcerpts.ItemsSource = sourceNode.Source.Excerpts;

            ccSourceDetails.Content = null;
            ccSourceDetails.Content = source;

            lvSourceExcerpts.ItemsSource = null;

            if (source != null)
            {
                lvSourceExcerpts.ItemsSource = source.Excerpts;
            }
        }

        private void ProcessEntryInput()
        {
            string itemValue = txtSourceExcerptInput.Text;

            if (!string.IsNullOrWhiteSpace(itemValue))
            {
                //process entry here
                var excerpt = new ArchivistSourceExcerpt()
                {
                    ExcerptValue = itemValue,
                    //SourceId = sourceNode.Source.SourceId
                    SourceId = source.SourceId,
                    ExcerptBeginTime = txtSourceExcerptBeginTime.Text,
                    ExcerptEndTime = txtSourceExcerptEndTime.Text,
                    ExcerptPages = txtSourceExcerptPages.Text
                };

                excerpt.SourceExcerptId = db.EnsureCore(excerpt);
                

                //sourceNode.Source.Add(excerpt);
                source.Add(excerpt);

                RefreshSourceAndExcerptsFromObject();

                //for testing
                //UI.Display.Message("you entered: " + itemValue);

                txtSourceExcerptInput.Text = "";
                txtSourceExcerptBeginTime.Text = "";
                txtSourceExcerptEndTime.Text = "";
                txtSourceExcerptPages.Text = "";
            }
            else
            {
                UI.Display.Message("excerpt value empty");
            }
        }

        private void TagAllWithSourceTag()
        {
            if(source == null)
            {
                UI.Display.Message("Source is null");
                return;
            }

            if (string.IsNullOrWhiteSpace(source.SourceTag))
            {
                UI.Display.Message("SourceTag not set");
                return;
            }

            List<ArchivistSourceExcerpt> lst = lvSourceExcerpts.Items.Cast<ArchivistSourceExcerpt>().ToList();

            foreach(var ase in lst)
            {
                ase.Tag(source.SourceTag);
            }

            db.SaveExcerptTaggings(lst);

            Core.DataUpdateManager.UpdateSourceExcerptDisplays();
        }

        #endregion

        #region events

        #region SourceExcerptSelected event

        protected virtual void OnSourceExcerptSelected(SourceExcerptSelectedEventArgs args)
        {
            SourceExcerptSelected?.Invoke(this, args);
        }

        public event EventHandler<SourceExcerptSelectedEventArgs> SourceExcerptSelected;

        public class SourceExcerptSelectedEventArgs
        {
            public SourceExcerptSelectedEventArgs(ArchivistSourceExcerptNode sen)
            {
                SourceExcerptNode = sen;
            }

            public ArchivistSourceExcerptNode SourceExcerptNode { get; private set; }
        }

        #endregion

        #region HyperlinkClicked event

        //public void Hyperlink_TagClicked(object sender, RoutedEventArgs e)
        //{
        //    UI.Display.Message("clicked");
        //}


        protected virtual void OnHyperlinkClicked(HyperlinkClickedEventArgs args)
        {
            HyperlinkClicked?.Invoke(this, args);
        }

        public event EventHandler<HyperlinkClickedEventArgs> HyperlinkClicked;

        public class HyperlinkClickedEventArgs
        {
            public HyperlinkClickedEventArgs(MediaTagNode tagNode)
            {
                MediaTagNode = tagNode;
            }

            public MediaTagNode MediaTagNode { get; private set; }
        }

        #endregion

        #endregion

        #region event handlers

        private void SourceLocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshSourceLocationSubsetsForSelectedLocation();
        }

        private void lvSourceExcerpts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //mirrors ArchivistMasterDisplay
            ArchivistSourceExcerpt ase = lvSourceExcerpts.SelectedItem as ArchivistSourceExcerpt;

            if(ase != null)
            {
                ArchivistSourceExcerptNode nd = new ArchivistSourceExcerptNode(ase);

                SourceExcerptSelectedEventArgs args =
                    new SourceExcerptSelectedEventArgs(nd);

                OnSourceExcerptSelected(args);
            }
        }

        private void txtSourceExcerptInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && Keyboard.IsKeyDown(Key.Enter))
            {
                ProcessEntryInput();
                e.Handled = true;
            }
        }

        private void ButtonAddSourceExcerpt_Click(object sender, RoutedEventArgs e)
        {
            ProcessEntryInput();
        }

        private void Hyperlink_OnClick(object sender, EventArgs e)
        {
            var link = (Hyperlink)sender;

            if (link != null)
            {
                var run = link.Inlines.FirstOrDefault() as Run;
                string tag = run == null ? string.Empty : run.Text;

                //UI.Display.Message("clicked " + text);

                if (!string.IsNullOrWhiteSpace(tag))
                {
                    MediaTagNode tagNode = new MediaTagNode(tag);

                    HyperlinkClickedEventArgs args =
                        new HyperlinkClickedEventArgs(tagNode);

                    OnHyperlinkClicked(args);
                }
            }
        }

        private void ButtonEditTags_Click(object sender, RoutedEventArgs e)
        {
            //TODO: refactor to common UI utility method?
            //mimics MediaTagDisplay.ButtonEditTags_Click(...)

            TextBlock tbTagString = 
                Core.UtilsUi.GetTemplateSibling<TextBlock, Button>(
                    (Button)sender, "tbTagString");

            TextBox txtTagString =
                Core.UtilsUi.GetTemplateSibling<TextBox, Button>(
                    (Button)sender, "txtTagString");
            
            ArchivistSourceExcerpt ase = 
                (ArchivistSourceExcerpt)tbTagString.DataContext;

            txtTagString.Text = ase.TagString;

            StackPanel spTextBlock =
                Core.UtilsUi.GetTemplateSibling<StackPanel, Button>(
                    (Button)sender, "spTagStringTextBlock");

            StackPanel spTextBox =
                Core.UtilsUi.GetTemplateSibling<StackPanel, Button>(
                    (Button)sender, "spTagStringTextBox");

            spTextBox.Visibility = Visibility.Visible;
            spTextBlock.Visibility = Visibility.Collapsed;
        }

        private void ButtonVerifyPresent_Click(object sender, RoutedEventArgs e)
        {
            TextBlock tbSubsetEntryStatusDetail =
                Core.UtilsUi.GetTemplateSibling<TextBlock, Button>(
                    (Button)sender, "tbSubsetEntryStatusDetail");

            ArchivistSourceLocationSubsetEntry slse =
                (ArchivistSourceLocationSubsetEntry)tbSubsetEntryStatusDetail.DataContext;
            
            slse.VerifyPresent();

            db.UpdateSourceLocationSubsetEntryTimeStamps(slse);

            RefreshSourceLocationSubsetEntriesFromDb();
        }

        private void ButtonVerifyMissing_Click(object sender, RoutedEventArgs e)
        {
            TextBlock tbSubsetEntryStatusDetail =
                Core.UtilsUi.GetTemplateSibling<TextBlock, Button>(
                    (Button)sender, "tbSubsetEntryStatusDetail");

            ArchivistSourceLocationSubsetEntry slse =
                (ArchivistSourceLocationSubsetEntry)tbSubsetEntryStatusDetail.DataContext;
            
            slse.VerifyMissing();

            db.UpdateSourceLocationSubsetEntryTimeStamps(slse);

            RefreshSourceLocationSubsetEntriesFromDb();
        }

        private void ButtonSaveTags_Click(object sender, RoutedEventArgs e)
        {
            //TODO: refactor to common UI utility method?
            //mimics MediaTagDisplay.ButtonSaveTags_Click(...)

            TextBox txtTagString =
                Core.UtilsUi.GetTemplateSibling<TextBox, Button>(
                    (Button)sender, "txtTagString");

            ArchivistSourceExcerpt ase =
                (ArchivistSourceExcerpt)txtTagString.DataContext;
                        
            ase.SetTagsFromTagString(txtTagString.Text);
            db.SaveExcerptTaggings(ase);
            //RefreshFromDb();
            Core.DataUpdateManager.UpdateSourceExcerptDisplays();
            
            StackPanel spTextBlock =
                Core.UtilsUi.GetTemplateSibling<StackPanel, Button>(
                    (Button)sender, "spTagStringTextBlock");

            StackPanel spTextBox =
                Core.UtilsUi.GetTemplateSibling<StackPanel, Button>(
                    (Button)sender, "spTagStringTextBox");

            spTextBox.Visibility = Visibility.Collapsed;
            spTextBlock.Visibility = Visibility.Visible;
        }

        private void ButtonRefreshSource_Click(object sender, RoutedEventArgs e)
        {
            RefreshSourceAndExcerptsFromDb();
            RefreshSourceLocationsFromDb();
            RefreshSourceLocationSubsetEntriesFromDb();
            UI.Display.Message("refreshed.");
        }

        private void EnterSourceTag_Click(object sender, RoutedEventArgs e)
        {
            if (source != null)
            {
                if (!string.IsNullOrWhiteSpace(source.SourceTag))
                {
                    UI.Display.Message("Source Tag Already Set To: " + source.SourceTag);
                }
                else
                {
                    var tag = UI.Prompt.Input("Enter Source Tag (WARNING: THIS IS PERMANENT AND CANNOT BE CHANGED, LEAVE BLANK TO CANCEL):");

                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        if(source.SourceId < 1)
                        {
                            db.SyncCore(source);
                        }

                        bool successful = db.SetSourceTag(source.SourceId, tag);

                        if (successful)
                        {
                            db.SyncCore(source);
                            RefreshSourceAndExcerptsFromDb();
                        }
                        else
                        {
                            UI.Display.Message(
                                "error setting source tag, make sure tag is unique!");
                        }
                    }
                }
            }

        }

        private void btnSourceTagAll_Click(object sender, RoutedEventArgs e)
        {
            TagAllWithSourceTag();
        }

        private void btnCopySourceTag_Click(object sender, RoutedEventArgs e)
        {
            if(source == null)
            {
                UI.Display.Message("source is null");
                return;
            }

            if (string.IsNullOrWhiteSpace(source.SourceTag))
            {
                UI.Display.Message("source tag not set");
                return;
            }
                        
            Clipboard.SetText(source.SourceTag);
            UI.Display.Message(source.SourceTag + " copied to clipboard");
        }

        private void btnAddLocationSubset_Click(object sender, RoutedEventArgs e)
        {
            var subsetName = UI.Prompt.Input("Enter Subset Name");

            if (string.IsNullOrWhiteSpace(subsetName))
            {
                UI.Display.Message("subset name cannot be empty");
                return;
            }

            if (cmbSourceLocations.SelectedItem
                is ArchivistSourceLocation sourceLocation)
            {
                try
                {
                    db.EnsureSourceLocationSubset(
                        sourceLocation.SourceLocationId, subsetName);

                    RefreshSourceLocationSubsetsForSelectedLocation();
                }
                catch (Exception ex)
                {
                    UI.Display.Message("error ensuring subset: " + ex.Message);
                }
            }
            else
            {
                UI.Display.Message("select source location");
            }
        }

        private void btnAddLocationSubsetEntry_Click(object sender, RoutedEventArgs e)
        {
            //check source not null and id is set
            if (source == null || source.SourceId < 1)
            {
                UI.Display.Message("source id unavailable");
                return;
            }

            //check location subset selected
            if (cmbLocationSubsets.SelectedItem
                    is ArchivistSourceLocationSubset sourceLocationSubset)
            {

                //verify entry value is not null or whitespace
                var subsetEntryValue = UI.Prompt.Input("Enter subset entry value (most commonly, a filename):");

                if (string.IsNullOrWhiteSpace(subsetEntryValue))
                {
                    UI.Display.Message("entry cannot be empty");
                    return;
                }

                try
                {
                    db.EnsureSourceLocationSubsetEntry(
                        source.SourceId,
                        sourceLocationSubset.SourceLocationSubsetId,
                        subsetEntryValue);
                    
                    RefreshSourceLocationSubsetEntriesFromDb();
                }
                catch (Exception ex)
                {
                    UI.Display.Message("error ensuring subset: " + ex.Message);
                }

            }
            else
            {
                UI.Display.Message("select source location and subset");
            }
        }

        private void chkFilterExcludeMissingLocationEntries_CheckChanged(object sender, RoutedEventArgs e)
        {
            RefreshSourceLocationSubsetEntriesFromDb();
        }

        #endregion

    }
}
