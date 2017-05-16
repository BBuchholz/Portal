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
            Refresh();
        }

        public void Refresh()
        {
            ccMediaTagDetails.Content = mediaTagNode.MediaTag;

            //mimic SynergyV5ListDisplay user control for async load of listviews
            
            Mock.Utils.PopulateTestMedia(mediaTagNode.MediaTag);
            Mock.Utils.PopulateTestExcerpts(mediaTagNode.MediaTag);

            lvMediaItems.ItemsSource = mediaTagNode.MediaTag.Media;
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
    }
}
