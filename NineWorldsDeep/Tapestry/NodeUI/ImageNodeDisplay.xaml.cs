using NineWorldsDeep.Core;
using NineWorldsDeep.FragmentCloud;
using NineWorldsDeep.Tapestry.Nodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for ImageNodeDisplay.xaml
    /// </summary>
    public partial class ImageNodeDisplay : UserControl
    {
        private FileSystemNode fileNode;

        public ImageNodeDisplay()
        {
            InitializeComponent();            
        }

        private void Rotate0Button_Click(object sender, RoutedEventArgs e)
        {
            ImageControl.RenderTransform = new RotateTransform(0);
        }

        private void Rotate90Button_Click(object sender, RoutedEventArgs e)
        {
            ImageControl.RenderTransform = new RotateTransform(90);
        }
        
        private void ImageControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left &&
                e.ClickCount == 2 &&
                fileNode != null)
            {
                NwdUtils.OpenFileExternally(fileNode.Path);
            }
        }

        public void Display(FileSystemNode nd)
        {
            fileNode = nd;
            FileDetailsControl.Display(nd);

            BitmapImage image = new BitmapImage();

            using (FileStream stream = File.OpenRead(nd.Path))
            {
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
            }

            ImageControl.Source = image;
        }

        private void MenuItemSendToTrash_Click(object sender, RoutedEventArgs e)
        {
            UI.Display.Message("need to get tagging working first.");
            
            /////MODIFY THIS TO USE FILE NODE INSTEAD OF FILE ELEMENT
            /////NEED TO ACCOUNT FOR DB UPDATE (fe.MoveToTrash() below)
            /////WITHOUT USING FILE ELEMENT BEFORE WE CAN IMPLEMENT THIS

            //delete
            //FileElement fe = (FileElement)lvFileElements.SelectedItem;

            //string msg = "Are you sure you want to move this file to trash? " +
            //    "Be aware that these tags will be permanently lost even if " +
            //    "file is restored from trash: ";

            //if (fe != null && UI.Prompt.Confirm(msg + fe.TagString, true))
            //{
            //    //imageControl.Source = null;

            //    fe.MoveToTrash(dbCore);

            //    //remove path from tag matrix
            //    tagMatrix.RemovePath(fe.Path);

            //    imageControl.Source = null;

            //    //refresh list
            //    LoadFromSelectedTag();
            //}
        }

        private void MenuItemCopyToExportStaging_Click(object sender, RoutedEventArgs e)
        {
            if (fileNode != null)
            {
                string imageStagingFolderPath = Configuration.ImageStagingFolder;

                //ensure directory exists
                Directory.CreateDirectory(imageStagingFolderPath);

                //create destination file path
                string fName = System.IO.Path.GetFileName(fileNode.Path);
                string destFilePath = System.IO.Path.Combine(imageStagingFolderPath, fName);

                //copy if !exists, else message                
                if (!File.Exists(destFilePath))
                {
                    File.Copy(fileNode.Path, destFilePath);
                    UI.Display.Message("copied to: " + destFilePath);
                }
                else
                {
                    UI.Display.Message("file exists: " + destFilePath);
                }
            }
        }
    }
}
