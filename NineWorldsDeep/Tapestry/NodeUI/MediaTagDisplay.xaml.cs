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
using NineWorldsDeep.Mnemosyne.V5;
using NineWorldsDeep.Tapestry.Nodes;
using NineWorldsDeep.Archivist;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for MediaTagDisplay.xaml
    /// </summary>
    public partial class MediaTagDisplay : UserControl
    {
        private Db.Sqlite.MediaV5SubsetDb db;
        private MediaTagNode mediaTagNode;

        public MediaTagDisplay()
        {
            InitializeComponent();
        }

        internal void Display(MediaTagNode tagNode)
        {
            this.mediaTagNode = tagNode;

            //LOAD HERE, ONCE
            //filter can be applied without having to hit the db again
            //mimic SynergyV5ListDisplay user control for async load of listviews
            Mock.Utils.PopulateTestMedia(mediaTagNode.MediaTag);
            Mock.Utils.PopulateTestExcerpts(mediaTagNode.MediaTag);

            Refresh(txtDeviceNameFilter.Text);
        }

        public void Refresh(string deviceNameFilter)
        {
            ccMediaTagDetails.Content = mediaTagNode.MediaTag;

            //lvMediaItems.ItemsSource = mediaTagNode.MediaTag.Media;
            lvMediaItems.ItemsSource =
                mediaTagNode.MediaTag.Media
                    .Where(m => m.IsDeviceNameFilterMatch(deviceNameFilter));

            lvSourceExcerpts.ItemsSource = mediaTagNode.MediaTag.SourceExcerpts;
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
            Grid parent = Core.UiUtils.FindAncestor<Grid>(expander);
            int rowIndex = Grid.GetRow(expander);

            if (parent.RowDefinitions.Count > rowIndex && rowIndex >= 0)
                parent.RowDefinitions[rowIndex].Height =
                    (expander.IsExpanded ? new GridLength(1, GridUnitType.Star) : GridLength.Auto);
        }

        //public static T FindAncestor<T>(DependencyObject current)
        //    where T : DependencyObject
        //{
        //    // Need this call to avoid returning current object if it is the 
        //    // same type as parent we are looking for
        //    current = VisualTreeHelper.GetParent(current);

        //    while (current != null)
        //    {
        //        if (current is T)
        //        {
        //            return (T)current;
        //        }
        //        current = VisualTreeHelper.GetParent(current);
        //    };
        //    return null;
        //}
        
        private void txtDeviceNameFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                Refresh(txtDeviceNameFilter.Text);
                e.Handled = true;
            }
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
