using NineWorldsDeep.Core;
using NineWorldsDeep.Tagger;
using NineWorldsDeep.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NineWorldsDeep.ImageBrowser
{
    /// <summary>
    /// Interaction logic for ImageBrowserMainWindow.xaml
    /// </summary>
    public partial class ImageBrowserMainWindow : Window
    {
        //private string currentImageFolder = "c:\\NWD-AUX\\images";
        private string currentImageFolder = Configuration.ImagesFolder;
        private bool lastLoadedFromFileInsteadOfDb = true; //default to filesystem for now

        public ImageBrowserMainWindow()
        {
            InitializeComponent();
            tgrGrid.SetFolderLoadStrategy(new LoadImagesStrategy());
            tgrGrid.AddSelectionChangedListener(new DisplayImageAction(ImageControl));
            //LoadFolder(lastLoadedFromFileInsteadOfDb, GetFolderWithLeastNonZeroUntaggedCount());
        }

        private string GetFolderWithLeastNonZeroUntaggedCount()
        {
            return tgrGrid.GetFolderWithLeastNonZeroUntaggedCount(
                Configuration.ImagesFolder, 
                new ImageFileFilter(),
                lastLoadedFromFileInsteadOfDb);
        }

        private string TagFilePath
        {
            get
            {
                //return currentImageFolder + "\\fileTags.xml";
                return Configuration.GetTagFilePath(currentImageFolder);
            }
        }

        private void LoadFolder(bool fileInsteadOfDb, string folderPath)
        {
            currentImageFolder = folderPath;
            tbFolder.Text = currentImageFolder;
            tgrGrid.Clear();
            tgrGrid.AddFolder(currentImageFolder);

            if (fileInsteadOfDb)
            {
                tgrGrid.LoadFromFile(TagFilePath);
            }
            else
            {
                tgrGrid.LoadFromDb(currentImageFolder);
            }
        }

        [Obsolete("use LoadFolder(bool, string)")]
        private void LoadFolder(string folderPath)
        {
            currentImageFolder = folderPath;
            tgrGrid.Clear();
            tgrGrid.AddFolder(currentImageFolder);
            tgrGrid.LoadFromFile(TagFilePath);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = currentImageFolder;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selected = dlg.SelectedPath;
                LoadFolder(lastLoadedFromFileInsteadOfDb, selected);
            }
        }

        private void Rotate0Button_Click(object sender, RoutedEventArgs e)
        {
            ImageControl.RenderTransform = new RotateTransform(0);
        }

        private void Rotate90Button_Click(object sender, RoutedEventArgs e)
        {
            ImageControl.RenderTransform = new RotateTransform(90);
        }

        private void MenuItemNwdTagger_Click(object sender, RoutedEventArgs e)
        {
            //TaggerGridWindow tgw = new TaggerGridWindow();
            //tgw.Show();
            Display.Message("not included in migration");
        }

        private void MenuItemOriginalVersion_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
        }

        private void MenuItemSaveToXml_Click(object sender, RoutedEventArgs e)
        {
            tgrGrid.SaveToFile(TagFilePath);
        }

        private void MenuItemLoadFromSmallest_Click(object sender, RoutedEventArgs e)
        {
            LoadFolder(lastLoadedFromFileInsteadOfDb,
                GetFolderWithLeastNonZeroUntaggedCount());
        }

        private void MenuItemLoadFromXml_Click(object sender, RoutedEventArgs e)
        {
            lastLoadedFromFileInsteadOfDb = true;
            LoadFolder(lastLoadedFromFileInsteadOfDb, GetFolderWithLeastNonZeroUntaggedCount());
        }

        private void MenuItemSaveToSqliteDb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tgrGrid.SaveToDb();
            }
            catch (Exception ex)
            {
                Display.Exception(ex);
            }
        }

        private void MenuItemLoadFromSqliteDb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lastLoadedFromFileInsteadOfDb = false;
                LoadFolder(lastLoadedFromFileInsteadOfDb, GetFolderWithLeastNonZeroUntaggedCount());

            }
            catch (Exception ex)
            {
                Display.Exception(ex);
            }
        }

        private void MenuItemClearAll_Click(object sender, RoutedEventArgs e)
        {
            tgrGrid.Clear();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tgrGrid.HasPendingChanges)
            {
                string msg = "There are pending changes " +
                    "not yet saved, are you sure you want to close?";

                if (!Prompt.Confirm(msg, true))
                {
                    e.Cancel = true;
                }
            }
        }

        private void MenuItemCopyToImageStaging_Click(object sender, RoutedEventArgs e)
        {
            if (tgrGrid.SelectedFileElement != null)
            {
                FileElement fe = (FileElement)tgrGrid.SelectedFileElement;
                string imageStagingFolderPath = Configuration.ImageStagingFolder;

                //ensure directory exists
                Directory.CreateDirectory(imageStagingFolderPath);

                //create destination file path
                string fName = System.IO.Path.GetFileName(fe.Path);
                string destFilePath = System.IO.Path.Combine(imageStagingFolderPath, fName);

                //copy if !exists, else message                
                if (!File.Exists(destFilePath))
                {
                    File.Copy(fe.Path, destFilePath);
                    Display.Message("copied to: " + destFilePath);
                }
                else
                {
                    Display.Message("file exists: " + destFilePath);
                }
            }
            else
            {
                Display.Message("nothing selected");
            }
        }
    }
}
