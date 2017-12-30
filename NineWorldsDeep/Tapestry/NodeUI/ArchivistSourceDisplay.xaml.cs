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
            }
            RefreshFromDb();
        }

        public void RefreshFromDb()
        {
            //if (this.sourceNode != null && this.sourceNode.Source != null)
            //{
            //    db.LoadSourceExcerptsWithTags(this.sourceNode.Source);
            //    RefreshFromObject();
            //}
            if (this.source != null)
            {
                db.LoadSourceExcerptsWithTags(this.source);
                RefreshFromObject();
            }
        }

        #endregion

        #region private helper methods

        private void RefreshFromObject()
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
                    SourceId = source.SourceId
                };

                excerpt.SourceExcerptId = db.EnsureCore(excerpt);

                //sourceNode.Source.Add(excerpt);
                source.Add(excerpt);

                RefreshFromObject();

                //for testing
                //UI.Display.Message("you entered: " + itemValue);
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

        private void btnAddSourceExcerpt_Click(object sender, RoutedEventArgs e)
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
            RefreshFromDb();

            UI.Display.Message("refreshed.");
        }

        private void btnEnterSourceTag_Click(object sender, RoutedEventArgs e)
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
                            RefreshFromDb();
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

        #endregion

    }
}
