using NineWorldsDeep.Core;
using NineWorldsDeep.Mnemosyne.V5;
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
    /// Interaction logic for FileDetailsV5.xaml
    /// </summary>
    public partial class FileDetailsV5 : UserControl
    {
        //private bool tagStringChanged = false;
        private string oldTagString;
        private FileSystemNode fileNode;

        public FileDetailsV5()
        {
            InitializeComponent();
        }

        public string TagString
        {
            get
            {
                return TagStringTextBox.Text;
            }

            set
            {
                oldTagString = TagStringTextBox.Text;
                TagStringTextBox.Text = value;
            }
        }
        
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string hash = Hashes.Sha1ForFilePath(fileNode.Path);
            
            Tags.UpdateTagStringForHash(hash, oldTagString, TagString);

            LoadTags();

            UpdateButton.IsEnabled = false;
        }

        private void TagStringTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (oldTagString != TagString)
            {
                UpdateButton.IsEnabled = true;
            }
        }

        private void LoadTags()
        {
            TagString = 
                Tags.GetTagStringForHash(
                    Hashes.Sha1ForFilePath(fileNode.Path));
        }

        public void Display(FileSystemNode nd)
        {
            fileNode = nd;
            MultiLineTextBox.Text = nd.ToMultiLineDetail();
            LoadTags();
        }

        private void OpenExternallyButton_Click(object sender, RoutedEventArgs e)
        {
            if (fileNode != null)
            {
                NwdUtils.OpenFileExternally(fileNode.Path);
            }
        }

    }
}
