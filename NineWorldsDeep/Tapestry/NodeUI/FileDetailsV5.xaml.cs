using NineWorldsDeep.Core;
using NineWorldsDeep.Db.Sqlite;
using NineWorldsDeep.Mnemosyne.V5;
using NineWorldsDeep.Tapestry.Nodes;
using NineWorldsDeep.Warehouse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Xml.Linq;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for FileDetailsV5.xaml
    /// </summary>
    public partial class FileDetailsV5 : UserControl
    {
        //private bool tagStringChanged = false;
        private string oldTagString;
        private MediaV5SubsetDb db;
        private FileSystemNode fileNode;
        private MediaListItem currentMediaListItem;
        
        //async related 
        private readonly SynchronizationContext syncContext;
        private DateTime previousTime = DateTime.Now;

        public FileDetailsV5()
        {
            InitializeComponent();
            syncContext = SynchronizationContext.Current;

            db = new MediaV5SubsetDb();
        }

        public string TagString
        {
            get
            {
                return tbTagString.Text;
            }

            set
            {
                oldTagString = tbTagString.Text;
                tbTagString.Text = value;
            }
        }

        #region event handlers

        private void OpenExternallyButton_Click(object sender, RoutedEventArgs e)
        {
            if (fileNode != null)
            {
                NwdUtils.OpenFileExternally(fileNode.Path);
            }
        }

        private async void ExportXmlButton_Click(object sender, RoutedEventArgs e)
        {
            string hash = currentMediaListItem.Media.MediaHash;

            try
            {
                await Task.Run(() =>
                {
                    XElement mnemosyneSubsetEl = new XElement(Xml.Xml.TAG_MNEMOSYNE_SUBSET);

                    //create media tag with attribute set for hash
                    XElement mediaEl = Xml.Xml.CreateMediaElement(hash);

                    var taggings = db.GetMediaTaggingsForHash(hash);

                    foreach (MediaTagging tag in taggings)
                    {
                        //create tag element and append to 
                        XElement tagEl = Xml.Xml.CreateTagElement(tag);
                        mediaEl.Add(tagEl);
                    }

                    //  db.getDevicePaths(media.Hash) <- create this
                    ///////--> return a MultiMap keyed on device name, with a list of path objects (path, verified, missing)

                    MultiMap<string, DevicePath> devicePaths = db.GetDevicePaths(hash);

                    foreach (string deviceName in devicePaths.Keys)
                    {
                        XElement deviceEl = Xml.Xml.CreateDeviceElement(deviceName);

                        foreach (DevicePath path in devicePaths[deviceName])
                        {
                            XElement pathEl = Xml.Xml.CreatePathElement(path);
                            deviceEl.Add(pathEl);
                        }

                        mediaEl.Add(deviceEl);
                    }

                    mnemosyneSubsetEl.Add(mediaEl);


                    XDocument doc =
                        new XDocument(
                            new XElement("nwd", mnemosyneSubsetEl));

                    //here, take doc and save to all sync locations            
                    string fileName =
                        NwdUtils.GetTimeStamp_yyyyMMddHHmmss() + "-nwd-mnemosyne-v5.xml";

                    var allFolders =
                        Configuration.GetActiveSyncProfileIncomingXmlFolders();

                    allFolders.AddRange(Configuration.GetHiveFoldersForXmlExport());

                    foreach (string xmlIncomingFolderPath in allFolders)
                    {
                        //Ensure the directory
                        Directory.CreateDirectory(xmlIncomingFolderPath);

                        string fullFilePath =
                            System.IO.Path.Combine(xmlIncomingFolderPath, fileName);

                        doc.Save(fullFilePath);
                    }

                    StatusDetailUpdate("exported to xml.");
                });
            }
            catch (Exception ex)
            {
                UI.Display.Exception(ex);
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            currentMediaListItem.SetTagsFromTagString(TagString);
            Sync();

            tbStatus.Text = "tags updated.";

            UpdateButton.IsEnabled = false;
        }

        private void TagStringTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (oldTagString != TagString)
            {
                UpdateButton.IsEnabled = true;
                tbStatus.Text = "tag string has unsaved changes";
            }
        }


        #endregion

        private void LoadTags()
        {
            TagString = currentMediaListItem.GetTagString();
        }

        public void Display(FileSystemNode nd)
        {
            fileNode = nd;
            MultiLineTextBox.Text = nd.ToMultiLineDetail();
            //SetCurrentMediaListItem(new MediaListItem(fileNode.Path));
            SetCurrentMediaListItemByPath(fileNode.Path);

            //if file does not exists locally, disable editing on tag box and display notification
            if (!File.Exists(fileNode.Path))
            {
                tbTagString.IsReadOnly = true;
                tbFileIsNonLocalLabel.Visibility = Visibility.Visible;
            }
            else
            {
                tbTagString.IsReadOnly = false;
                tbFileIsNonLocalLabel.Visibility = Visibility.Hidden;
            }

        }

        private void SetCurrentMediaListItemByPath(string filePath)
        {
            if (File.Exists(filePath))
            {
                currentMediaListItem = new MediaListItem(filePath);
                currentMediaListItem.HashMedia();
                Sync();
            }
            else
            {
                string mediaHash = Configuration.DB.MediaSubset.GetMediaHashByPath(filePath);
                currentMediaListItem = new MediaListItem(filePath, mediaHash);
                Sync();
            }
        }

        //private void SetCurrentMediaListItem(MediaListItem mli)
        //{
        //    currentMediaListItem = mli;
        //    currentMediaListItem.HashMedia();
        //    Sync();
        //}

        private void Sync()
        {
            Configuration.DB.MediaSubset.Sync(currentMediaListItem.Media);
            LoadTags();
        }

        public void StatusDetailUpdate(string text)
        {
            var currentTime = DateTime.Now;

            if ((DateTime.Now - previousTime).Milliseconds <= 50) return;

            syncContext.Post(new SendOrPostCallback(s =>
            {
                tbStatus.Text = (string)s;
            }), text);

            previousTime = currentTime;
        }
    }
}
