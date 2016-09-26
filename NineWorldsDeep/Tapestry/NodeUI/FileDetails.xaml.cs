using NineWorldsDeep.Core;
using NineWorldsDeep.FragmentCloud;
using NineWorldsDeep.Tapestry.Nodes;
using NineWorldsDeep.Warehouse;
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
    /// Interaction logic for FileDetails.xaml
    /// </summary>
    public partial class FileDetails : UserControl
    {
        private bool tagStringChanged = false;
        private FileSystemNode fileNode;

        public FileDetails()
        {
            InitializeComponent();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string path = fileNode.Path;
            string tagString = TagStringTextBox.Text;

            Tags.UpdateTagStringForCurrentDevicePath(path, tagString);

            LoadTags();

            UpdateButton.IsEnabled = false;
            tagStringChanged = false;
        }

        private void TagStringTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!tagStringChanged)
            {
                tagStringChanged = true;
                UpdateButton.IsEnabled = true;
            }
        }

        private void LoadTags()
        {
            TagStringTextBox.Text = Tags.GetTagStringForCurrentDevicePath(fileNode.Path); 
        }

        public void Display(FileSystemNode nd)
        {
            fileNode = nd;
            MultiLineTextBox.Text = nd.ToMultiLineDetail();
            LoadTags();
        }

        private void OpenExternallyButton_Click(object sender, RoutedEventArgs e)
        {
            if(fileNode != null)
            {
                NwdUtils.OpenFileExternally(fileNode.Path);
            }
        }
    }
}
