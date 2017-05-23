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
    public partial class ArchivistSourceDisplay : UserControl
    {
        private Db.Sqlite.ArchivistSubsetDb db;
        private ArchivistSourceNode sourceNode;

        public ArchivistSourceDisplay()
        {
            InitializeComponent();
        }

        public void Display(ArchivistSourceNode src)
        {
            this.sourceNode = src;
            Refresh();            
        }

        public void Refresh()
        {
            ccSourceDetails.Content = sourceNode.Source;

            //mimic SynergyV5ListDisplay user control for async load of listview
            
            lvSourceExcerpts.ItemsSource = 
                Mock.Utils.MockArchivistSourceExcerpts();            
        }

        private void lvSourceExcerpts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
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

        public void Hyperlink_TagClicked(object sender, RoutedEventArgs e)
        {
            UI.Display.Message("clicked");
        }

        private void ProcessEntryInput()
        {
            string itemValue = txtSourceExcerptInput.Text;

            if (!string.IsNullOrWhiteSpace(itemValue))
            {
                //process entry here

                UI.Display.Message("you entered: " + itemValue);
            }
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

        private void ButtonEditTags_Click(object sender, RoutedEventArgs e)
        {
            TextBlock tbTagString = 
                Core.UiUtils.GetTemplateSibling<TextBlock, Button>(
                    (Button)sender, "tbTagString");

            TextBox txtTagString =
                Core.UiUtils.GetTemplateSibling<TextBox, Button>(
                    (Button)sender, "txtTagString");


            ArchivistSourceExcerpt ase = 
                (ArchivistSourceExcerpt)tbTagString.DataContext;

            //UI.Display.Message(ase.TagString);
            txtTagString.Text = ase.TagString;

            StackPanel spTextBlock =
                Core.UiUtils.GetTemplateSibling<StackPanel, Button>(
                    (Button)sender, "spTagStringTextBlock");

            StackPanel spTextBox =
                Core.UiUtils.GetTemplateSibling<StackPanel, Button>(
                    (Button)sender, "spTagStringTextBox");

            spTextBox.Visibility = Visibility.Visible;
            spTextBlock.Visibility = Visibility.Collapsed;
        }
        
        private void ButtonSaveTags_Click(object sender, RoutedEventArgs e)
        {
            TextBox txtTagString =
                Core.UiUtils.GetTemplateSibling<TextBox, Button>(
                    (Button)sender, "txtTagString");

            ArchivistSourceExcerpt ase =
                (ArchivistSourceExcerpt)txtTagString.DataContext;

            ase.TagString = txtTagString.Text;

            //UI.Display.Message(txtTagString.Text);     

            StackPanel spTextBlock =
                Core.UiUtils.GetTemplateSibling<StackPanel, Button>(
                    (Button)sender, "spTagStringTextBlock");

            StackPanel spTextBox =
                Core.UiUtils.GetTemplateSibling<StackPanel, Button>(
                    (Button)sender, "spTagStringTextBox");

            spTextBox.Visibility = Visibility.Collapsed;
            spTextBlock.Visibility = Visibility.Visible;
        }


    }
}
