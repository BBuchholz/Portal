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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NineWorldsDeep.AudioBrowser
{
    /// <summary>
    /// Interaction logic for AudioBrowserMainWindow.xaml
    /// </summary>
    public partial class AudioBrowserMainWindow : Window
    {
        private string voiceMemoFolderPath;
        private string tagFilePath;

        public AudioBrowserMainWindow()
            : this(Configuration.VoiceMemosFolder,
                   Configuration.VoiceMemoTagFilePath)
        {
            //do nothing, chained constructor
        }

        public AudioBrowserMainWindow(string voiceMemoFolderPath, string tagFilePath)
        {
            InitializeComponent();
            this.voiceMemoFolderPath = voiceMemoFolderPath;
            this.tagFilePath = tagFilePath;
            tgrGrid.DoubleClickListener = new PlayAction();
            tgrGrid.AddSelectionChangedListener(new FileElementTagExtractionAction(tgrGrid));
            tgrGrid.AddSelectionChangedListener(new ConditionalPlayAction(chkPlayOnSelectionChange));
            tgrGrid.SetStatusForegroundColor(Brushes.White);
        }

        private void MenuItemSaveToXml_Click(object sender, RoutedEventArgs e)
        {
            tgrGrid.SaveToFile(tagFilePath);
        }

        private void MenuItemLoadFromXml_Click(object sender, RoutedEventArgs e)
        {
            tgrGrid.AddFolder(voiceMemoFolderPath);
            tgrGrid.LoadFromFile(tagFilePath);
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
                tgrGrid.AddFolder(voiceMemoFolderPath);
                tgrGrid.LoadFromDb(voiceMemoFolderPath);
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

        private void MenuItemOriginal_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
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

        private void MenuItemCopyToVoiceMemoStaging_Click(object sender, RoutedEventArgs e)
        {
            if (tgrGrid.SelectedFileElement != null)
            {
                FileElement fe = (FileElement)tgrGrid.SelectedFileElement;
                string vmStagingFolderPath = Configuration.VoiceMemoStagingFolder;

                //ensure directory exists
                Directory.CreateDirectory(vmStagingFolderPath);

                //create destination file path
                string fName = System.IO.Path.GetFileName(fe.Path);
                string destFilePath = System.IO.Path.Combine(vmStagingFolderPath, fName);

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
