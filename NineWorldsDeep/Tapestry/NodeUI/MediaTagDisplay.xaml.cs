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
using System.IO;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for MediaTagDisplay.xaml
    /// </summary>
    public partial class MediaTagDisplay : UserControl, ISourceExcerptDisplay
    {
        #region fields

        private Db.Sqlite.MediaV5SubsetDb dbMediaV5;
        private Db.Sqlite.ArchivistSubsetDb dbArchivist;
        private MediaTagNode mediaTagNode;

        #endregion

        #region creation

        public MediaTagDisplay()
        {
            InitializeComponent();
            dbMediaV5 = new Db.Sqlite.MediaV5SubsetDb();
            dbArchivist = new Db.Sqlite.ArchivistSubsetDb();
            Core.DataUpdateManager.Register(this);
        }

        #endregion

        #region public interface

        public void Display(MediaTagNode tagNode)
        {
            this.mediaTagNode = tagNode;
            RefreshFromDb();
        }

        public void RefreshFromDb()
        {
            //load here once, then filter can be applied 
            //without having to hit the db again
            
            if (this.mediaTagNode != null && this.mediaTagNode.MediaTag != null)
            {
                if(this.mediaTagNode.MediaTag.MediaTagId < 1)
                {
                    dbMediaV5.PopulateTagIdForTagValue(
                        this.mediaTagNode.MediaTag);
                }

                //TODO: mimic SynergyV5ListDisplay user control for async load of listviews
                
                dbMediaV5.LoadMediaWithDevicePathsForTag(this.mediaTagNode.MediaTag);
                dbArchivist.LoadSourceExcerptsWithTags(this.mediaTagNode.MediaTag);
                
                RefreshFromObject(txtDeviceNameFilter.Text);
            }
        }

        #endregion

        #region private helper methods

        private void RefreshFromObject(string deviceNameFilter)
        {
            ccMediaTagDetails.Content = mediaTagNode.MediaTag;

            //lvMediaItems.ItemsSource = mediaTagNode.MediaTag.Media;
            lvMediaItems.ItemsSource =
                mediaTagNode.MediaTag.Media
                    .Where(m => m.IsDevicePathFilterMatch(deviceNameFilter));

            lvSourceExcerpts.ItemsSource = mediaTagNode.MediaTag.SourceExcerpts;
        }


        /// <summary>
        /// manages grid rows to share space between multiple expanded expanders
        /// </summary>
        /// <param name="expander"></param>
        private void ProcessExpanderState(Expander expander)
        {
            Grid parent = Core.UtilsUi.FindAncestor<Grid>(expander);
            int rowIndex = Grid.GetRow(expander);

            if (parent.RowDefinitions.Count > rowIndex && rowIndex >= 0)
                parent.RowDefinitions[rowIndex].Height =
                    (expander.IsExpanded ? new GridLength(1, GridUnitType.Star) : GridLength.Auto);
        }

        #endregion

        #region event handlers

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            ProcessExpanderState((Expander)sender);
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            ProcessExpanderState((Expander)sender);
        }

        private void txtDeviceNameFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                RefreshFromObject(txtDeviceNameFilter.Text);
                e.Handled = true;
            }
        }


        private void ButtonEditTags_Click(object sender, RoutedEventArgs e)
        {
            //TODO: refactor to common UI utility method?
            //mimics ArchivistSourceDisplay.ButtonEditTags_Click(...)

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
            //mimics ArchivistSourceDisplay.ButtonSaveTags_Click(...)

            TextBox txtTagString =
                Core.UtilsUi.GetTemplateSibling<TextBox, Button>(
                    (Button)sender, "txtTagString");

            ArchivistSourceExcerpt ase =
                (ArchivistSourceExcerpt)txtTagString.DataContext;

            ase.SetTagsFromTagString(txtTagString.Text);
            dbArchivist.SaveExcerptTaggings(ase);

            ////may need testing (mirroring ArchivistSourceDisplay for now)
            //RefreshFromObject(txtDeviceNameFilter.Text);

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

        private void ListViewMediaPaths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO: all of this is convoluted cuz I haven't learned MVVM yet, but I'm learning it and when I'm good with it, we can make all of this clean
            //link that has a way to handle commands MVVM style: https://msdn.microsoft.com/en-us/magazine/dd419663.aspx

            if (sender != null)
            {
                ListView listViewContainingSelectedItem = sender as ListView;

                if (listViewContainingSelectedItem != null)
                {
                    DevicePath devicePath =
                        listViewContainingSelectedItem.SelectedItem as DevicePath;

                    if (devicePath != null)
                    {
                        if (File.Exists(devicePath.DevicePathValue) && dbMediaV5.LocalDeviceId > 0)
                        {
                            PathSelectedEventArgs args =
                                new PathSelectedEventArgs(
                                    new FileSystemNode(devicePath.DevicePathValue, true, dbMediaV5.LocalDeviceId));

                            OnPathSelected(args);
                        }
                        else
                        {
                            UI.Display.Message(devicePath.DevicePathValue + " does not exist on this device or local device id is not set");
                        }


                        

                    }
                }
            }
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshFromDb();
        }

        #endregion
        
        #region event PathSelected

        protected virtual void OnPathSelected(PathSelectedEventArgs args)
        {
            PathSelected?.Invoke(this, args);
        }

        public event EventHandler<PathSelectedEventArgs> PathSelected;

        public class PathSelectedEventArgs
        {
            public PathSelectedEventArgs(FileSystemNode f)
            {
                FileSystemNode = f;
            }

            public FileSystemNode FileSystemNode { get; private set; }
        }

        #endregion



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

    }
}
