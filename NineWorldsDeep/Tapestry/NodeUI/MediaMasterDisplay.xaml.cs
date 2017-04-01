using NineWorldsDeep.Mnemosyne.V5;
using NineWorldsDeep.Core;
using NineWorldsDeep.Db.Sqlite;
using NineWorldsDeep.Model;
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
    /// Interaction logic for MediaMasterDisplay.xaml
    /// </summary>
    public partial class MediaMasterDisplay : UserControl, IAsyncStatusResponsive
    {
        private MediaV5SubsetDb db;
        MultiMap<string, string> fileSystemExtensionToPaths;
        MultiMap<string, string> dataBaseExtensionToPaths;

        //async related 
        private readonly SynchronizationContext syncContext;
        private DateTime previousTime = DateTime.Now;

        public MediaMasterDisplay()
        {
            InitializeComponent();
            syncContext = SynchronizationContext.Current;

            db = new MediaV5SubsetDb();
            LoadMediaDevices();
        }

        public MediaDeviceModelItem SelectedMediaDevice
        {
            get
            {
                return (MediaDeviceModelItem)mMediaDevicesComboBox.SelectedItem;
            }
        }

        private void LoadMediaDevices()
        {
            mMediaDevicesComboBox.ItemsSource = db.GetAllMediaDevices();
        }

        private void mMediaDevicesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MediaDeviceModelItem mdmi = SelectedMediaDevice;

            mMediaRootsComboBox.ItemsSource = null;
            mFileTypesComboBox.ItemsSource = null;
            mFilePathsListView.ItemsSource = null;

            if(mdmi != null)
            {
                if(mdmi.MediaDeviceId == db.LocalDeviceId)
                {
                    RefreshMediaRoots();
                }
                else
                {
                    UI.Display.Message("External Devices not yet supported");
                }
            }
        }

        private void mAddMediaRootButton_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = UI.Prompt.ForFolder("C:\\");

            if(folderPath != null)
            {
                db.InsertMediaRoot(db.LocalDeviceId, folderPath);

                RefreshMediaRoots();
            }
        }

        private void RefreshMediaRoots()
        {
            MediaDeviceModelItem mdmi = SelectedMediaDevice;

            if (mdmi != null)
            {
                List<MediaRootModelItem> lst =
                            db.GetMediaRootsForDeviceId(mdmi.MediaDeviceId);

                mMediaRootsComboBox.ItemsSource = null;
                mMediaRootsComboBox.ItemsSource = lst;
            }
        }

        private void mMediaRootsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessSelectedMediaRoot();
        }

        private async void ProcessSelectedMediaRoot()
        {
            MediaRootModelItem root = (MediaRootModelItem)mMediaRootsComboBox.SelectedItem;

            if(mStatusTextBlock != null)
            {
                mStatusTextBlock.Text = "";
            }

            if(mFileTypesComboBox != null)
            {
                mFileTypesComboBox.ItemsSource = null;
            }

            if(mFilePathsListView != null)
            {
                mFilePathsListView.ItemsSource = null;
            }

            if (root != null)
            {
                string rootPath = root.MediaRootPath;

                if (mFileSystemRadioButton.IsChecked != null &&
                    mFileSystemRadioButton.IsChecked.Value)
                {
                    fileSystemExtensionToPaths = await Task.Run(() =>
                        GetFileSystemExtToPath(this, rootPath)
                    );

                    List<ExtensionEntry> entries = new List<ExtensionEntry>();

                    foreach (string ext in fileSystemExtensionToPaths.Keys)
                    {
                        int count = fileSystemExtensionToPaths[ext].Count;
                        entries.Add(new ExtensionEntry(ext, count));
                    }

                    mFileTypesComboBox.ItemsSource = entries;
                }

                if (mDataBaseRadioButton.IsChecked != null &&
                    mDataBaseRadioButton.IsChecked.Value &&
                    SelectedMediaDevice != null)
                {
                    int mediaDeviceId = SelectedMediaDevice.MediaDeviceId;

                    dataBaseExtensionToPaths = await Task.Run(() =>
                        GetDataBaseExtToPath(db,
                                             this, 
                                             mediaDeviceId, 
                                             rootPath)
                    );

                    List<ExtensionEntry> entries = new List<ExtensionEntry>();

                    foreach (string ext in dataBaseExtensionToPaths.Keys)
                    {
                        int count = dataBaseExtensionToPaths[ext].Count;
                        entries.Add(new ExtensionEntry(ext, count));
                    }
                    
                    mFileTypesComboBox.ItemsSource = entries;
                }
            }
            else
            {
                if(mFileTypesComboBox != null)
                {
                    mFileTypesComboBox.ItemsSource = null;
                }
            }
        }

        private MultiMap<string, string> GetDataBaseExtToPath(
            MediaV5SubsetDb v5Db, MediaMasterDisplay gui, int mediaDeviceId, string rootPath)
        {
            MultiMap<string, string> extToPaths =
                new MultiMap<string, string>(
                    StringComparer.CurrentCultureIgnoreCase);

            List<string> allFilePaths =
                v5Db.GetAllFilePathsForDeviceRoot(mediaDeviceId, rootPath);

            int count = 0;
            int total = allFilePaths.Count();

            foreach (string path in allFilePaths)
            {
                count++;

                if (count % 100 == 0)
                {
                    string msg = "processing " + count + " of " +
                        total + ": " + path;

                    gui.UpdateStatus(msg);
                }

                string ext = System.IO.Path.GetExtension(path);

                if (string.IsNullOrWhiteSpace(ext))
                {
                    //Path.GetExtension can return null or string.Empty,
                    //this is so they still get indexed with a key
                    ext = "[NULL]";
                }

                extToPaths.Add(ext, path);
            }

            gui.UpdateStatus("finished processing " + count + " files.");

            return extToPaths;
        }

        private MultiMap<string, string> GetFileSystemExtToPath(
            MediaMasterDisplay gui, string rootPath)
        {
            MultiMap<string, string> extToPaths =
                new MultiMap<string, string>(
                    StringComparer.CurrentCultureIgnoreCase);

            var allFilePaths = 
                Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories);
            int count = 0;
            int total = allFilePaths.Count();

            foreach (string path in allFilePaths)
            {
                count++;
                
                if(count % 100 == 0)
                {
                    string msg = "processing " + count + " of " +
                        total + ": " + path;

                    gui.UpdateStatus(msg);
                }

                string ext = System.IO.Path.GetExtension(path);

                if (string.IsNullOrWhiteSpace(ext))
                {
                    //Path.GetExtension can return null or string.Empty,
                    //this is so they still get indexed with a key
                    ext = "[NULL]";
                }
                
                extToPaths.Add(ext, path);
            }

            gui.UpdateStatus("finished processing " + count + " files.");

            return extToPaths;
        }

        private void UpdateStatus(string text)
        {
            Dispatcher.Invoke(() =>
            {
                mStatusTextBlock.Text = text;
            });
        }

        private void mFileTypesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExtensionEntry ext = (ExtensionEntry)mFileTypesComboBox.SelectedItem;
            
            //clear it either way
            mFilePathsListView.ItemsSource = null;

            if(ext != null)
            {
                if (!string.IsNullOrWhiteSpace(ext.Extension))
                {
                    mFilePathsListView.ItemsSource =
                        fileSystemExtensionToPaths[ext.Extension];
                }
            }
        }

        private void mUpdateDbFromFileSystemButton_Click(object sender, RoutedEventArgs e)
        {
            if(fileSystemExtensionToPaths != null && SelectedMediaDevice != null)
            {
                List<string> paths = new List<string>();
                int mediaDeviceId = SelectedMediaDevice.MediaDeviceId;

                foreach(string ext in fileSystemExtensionToPaths.Keys)
                {
                    paths.AddRange(fileSystemExtensionToPaths[ext]);
                }

                UpdateStatus("inserting paths into database...");
                
                db.InsertPathsForDeviceId(mediaDeviceId, paths);

                UpdateStatus("finished inserting paths into database.");
            }
            else
            {
                UI.Display.Message("media device must be selected and file system paths must be populated. One or both have not been done.");
            }
        }
        
        private void mFileSourceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ProcessSelectedMediaRoot();    
        }

        private class ExtensionEntry
        {
            public string Extension { get; private set; }
            public int Count { get; private set; }

            public ExtensionEntry(string ext, int count)
            {
                Extension = ext;
                Count = count;
            }

            public override string ToString()
            {
                return Extension + " (" + Count + ")";
            }
        }

        private void mFilePathsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> selectedPaths = mFilePathsListView.SelectedItems.Cast<string>().ToList();

            if(selectedPaths.Count > 0)
            {
                //display first selected
                string firstPath = selectedPaths[0];

                if (File.Exists(firstPath) && SelectedMediaDevice != null)
                {
                    PathSelectedEventArgs args =
                        new PathSelectedEventArgs(
                            new FileSystemNode(firstPath, true, SelectedMediaDevice.MediaDeviceId));

                    OnPathSelected(args);
                }
            }
        }

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

        private async void btnImportV4TagsToV5_Click(object sender, RoutedEventArgs e)
        {
            //get V4c PathTagLinks
            DbAdapterV4c v4cDb = new DbAdapterV4c();

            await Task.Run(() =>
            {
                StatusDetailUpdate("retrieving PathTagLinks...");

                var pathTagLinks = v4cDb.GetPathTagLinks("");
                
                int pathTagLinksCount = 0;
                int pathTagLinksTotal = pathTagLinks.Count;

                MultiMap<string, string> pathToTags = new MultiMap<string, string>();
                HashSet<string> allTags = new HashSet<string>();

                foreach(var link in pathTagLinks)
                {
                    pathTagLinksCount++;

                    StatusDetailUpdate("Sorting links: processing link " +
                        pathTagLinksCount + " of " + pathTagLinksTotal);

                    pathToTags.Add(link.PathValue, link.TagValue);
                    allTags.Add(link.TagValue);
                }

                StatusDetailUpdate("Ensuring media tags...");
                db.EnsureMediaTags(allTags);

                StatusDetailUpdate("Indexing media tags...");
                Dictionary<string, Tag> tagsByTagValue = db.GetAllMediaTags();

                Dictionary<string, string> pathToHash =
                    new Dictionary<string, string>();

                int localMediaDeviceId = Configuration.DB.MediaSubset.LocalDeviceId;

                int pathCountTotal = pathToTags.Keys.Count();
                int pathCountCurrent = 0;

                foreach(var path in pathToTags.Keys)
                {
                    pathCountCurrent++;

                    StatusDetailUpdate("hashing and storing path " + pathCountCurrent + " of " + pathCountTotal + ": " + path);
                    string hash = Hashes.Sha1ForFilePath(path);
                    pathToHash.Add(path, hash);
                    db.StoreHashForPath(localMediaDeviceId, path, hash);
                }

                StatusDetailUpdate("indexing media by hash...");
                Dictionary<string, Media> hashToMedia =
                    db.GetAllMedia();

                List<MediaTagging> taggings = new List<MediaTagging>();

                pathCountTotal = pathToTags.Keys.Count();
                pathCountCurrent = 0;

                foreach(string path in pathToTags.Keys)
                {
                    pathCountCurrent++;

                    StatusDetailUpdate("processing tags for path " + pathCountCurrent + " of " + pathCountTotal + ": " + path);
                    
                    if (pathToHash.ContainsKey(path))
                    {
                        string pathHash = pathToHash[path];

                        if (hashToMedia.ContainsKey(pathHash))
                        {
                            int mediaId = hashToMedia[pathHash].MediaId;

                            foreach (string tag in pathToTags[path])
                            {
                                if (tagsByTagValue.ContainsKey(tag))
                                {
                                    int mediaTagId = tagsByTagValue[tag].TagId;

                                    taggings.Add(new MediaTagging
                                    {
                                        MediaId = mediaId,
                                        MediaTagId = mediaTagId
                                    });
                                }
                            }
                        }

                    }
                }

                StatusDetailUpdate("ensuring media taggings in db...");

                db.EnsureMediaTaggings(taggings);
            });

            tbStatus.Text = "finished.";
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

        private async void btnImportXml_Click(object sender, RoutedEventArgs e)
        {
            var allPaths = Configuration.GetMnemosyneXmlImportPaths();
            var count = 0;
            var total = allPaths.Count();           

            try
            {
                await Task.Run(() =>
                {
                    string detail = "starting import of mnemosyne xml files";
                    StatusDetailUpdate(detail);

                    foreach (string path in allPaths)
                    {
                        count++;
                        
                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            string fileName = System.IO.Path.GetFileName(path);

                            //XDocument doc = Xml.Xml.DocumentFromPath(path);

                            //List<Media> allMedia = Xml.Xml.RetrieveMedia(doc);

                            List<Media> allMedia = 
                                Xml.Xml.RetrieveMediaWithReaderAsync(path, this);

                            string prefix = "file " + count + " of " + total;
                            prefix += " -> ";

                            db.SyncAsync(allMedia, this, prefix);
                                                      
                            
                            File.Delete(path);
                        }

                    }

                });

                tbStatus.Text = "finished.";
            }
            catch (Exception ex)
            {
                tbStatus.Text = "Error: " + ex.Message;
            }
        }

        private async void btnExportXml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {

                    string detail = "starting export of mnemosyne subset";

                    StatusDetailUpdate(detail);
               
                    var hashedMedia = db.GetAllMedia();

                    XElement mnemosyneSubsetEl = new XElement(Xml.Xml.TAG_MNEMOSYNE_SUBSET);

                    int total = hashedMedia.Keys.Count;
                    int count = 0;

                    foreach (string hash in hashedMedia.Keys)
                    {
                        count++;

                        //just for testing
                        //if (count > 10)
                        //{
                        //    break;
                        //}

                        detail = count + " of " + total + ":" + hash + ": processing";

                        //create media tag with attribute set for hash
                        XElement mediaEl = Xml.Xml.CreateMediaElement(hash);

                        detail = count + " of " + total + ":" + hash + ": processing tags";
                        StatusDetailUpdate(detail);

                        var taggings = db.GetTaggedMediaTaggingsForHash(hash);

                        foreach (MediaTagging tag in taggings)
                        {
                            //create tag element and append to 
                            XElement tagEl = Xml.Xml.CreateTagElement(tag);
                            mediaEl.Add(tagEl);
                        }

                        //  db.getDevicePaths(media.Hash) <- create this
                        ///////--> return a MultiMap keyed on device name, with a list of path objects (path, verified, missing)

                        detail = count + " of " + total + ":" + hash + ": processing device paths";
                        StatusDetailUpdate(detail);

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
                    }

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

                });

                tbStatus.Text = "finished.";
            }
            catch (Exception ex)
            {
                tbStatus.Text = "Error: " + ex.Message;
            }

        }
    }
}
