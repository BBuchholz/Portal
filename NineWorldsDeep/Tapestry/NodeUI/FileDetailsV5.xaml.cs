using NineWorldsDeep.Core;
using NineWorldsDeep.Db.Sqlite;
using NineWorldsDeep.Mnemosyne.V5;
using NineWorldsDeep.Tapestry.Nodes;
using NineWorldsDeep.Warehouse;
using System;
using System.Collections.Generic;
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
            //string hash = Hashes.Sha1ForFilePath(fileNode.Path);

            //Tags.UpdateTagStringForHash(hash, oldTagString, TagString);

            //LoadTags();

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

        private void LoadTags()
        {
            //TagString = 
            //    Tags.GetTagStringForHash(
            //        Hashes.Sha1ForFilePath(fileNode.Path));

            TagString = currentMediaListItem.GetTagString();
        }

        public void Display(FileSystemNode nd)
        {
            fileNode = nd;
            MultiLineTextBox.Text = nd.ToMultiLineDetail();
            SetCurrentMediaListItem(new MediaListItem(fileNode.Path));
        }

        private void SetCurrentMediaListItem(MediaListItem mli)
        {
            currentMediaListItem = mli;
            currentMediaListItem.HashMedia();
            Sync();
        }

        private void Sync()
        {
            Configuration.DB.MediaSubset.Sync(currentMediaListItem.Media);
            LoadTags();
        }

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

                    foreach (string xmlIncomingFolderPath in allFolders)
                    {
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
