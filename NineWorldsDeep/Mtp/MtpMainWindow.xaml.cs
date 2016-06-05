using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NineWorldsDeep.Parser;
using NineWorldsDeep.UI;
using Microsoft.Win32;
using NineWorldsDeep.Core;
using System.IO;
using System.Diagnostics;
using NineWorldsDeep.Db;

namespace NineWorldsDeep.Mtp
{
    /// <summary>
    /// Interaction logic for MtpMainWindow.xaml
    /// </summary>
    public partial class MtpMainWindow : Window
    {
        private NwdPortableDevice _currentDevice;
        private Stack<NwdUriProcessEntry> unprocessedDeletions;
        private Stack<NwdUriProcessEntry> unprocessedSynergyFiles;
        private Stack<NwdUriProcessEntry> unprocessedHashedMedia;
        private NwdPortableDeviceFile toBeRemoved;
        //TODO: LICENSE NOTES
        //marshalling fix: http://www.andrewt.com/blog/post/2013/06/15/Fun-with-MTP-in-C.aspx

        public MtpMainWindow()
        {
            InitializeComponent();
        }

        private string ToMultiLineString(PortableDeviceObject pdo)
        {
            string output = pdo.Name;

            if (pdo is PortableDeviceFolder)
            {
                output += Environment.NewLine;
                output += DisplayFolderContents((PortableDeviceFolder)pdo);
            }

            return output;
        }

        private string DisplayObject(PortableDeviceObject pdo)
        {
            string output = pdo.Name;
            if (pdo is PortableDeviceFolder)
            {
                output += Environment.NewLine;
                output += DisplayFolderContents((PortableDeviceFolder)pdo);
            }

            return output;
        }

        private string DisplayFolderContents(PortableDeviceFolder folder)
        {
            bool first = true;
            string output = "";

            foreach (var item in folder.Files)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    output += Environment.NewLine;
                }

                output += item.Id;
                if (item is PortableDeviceFolder)
                {
                    output +=
                        DisplayFolderContents((PortableDeviceFolder)item);
                }

                if (item is PortableDeviceFile)
                {
                    if (_currentDevice != null)
                    {
                        PortableDeviceFile pdf = (PortableDeviceFile)item;

                        _currentDevice.DownloadFile(pdf, @"c:\kindle\");
                        output += "downloaded " + pdf.Name;
                    }
                }
            }

            return output;
        }

        private void MenuItemEnumerateDevices_Click(object sender, RoutedEventArgs e)
        {//TODO: LICENSE NOTES
            //enumerating devices: https://cgeers.wordpress.com/2011/05/22/enumerating-windows-portable-devices/
            DisplayTextViewList();

            var col = new NwdPortableDeviceCollection();

            col.Refresh();

            string output = "";
            bool first = true;

            foreach (var device in col)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    output += Environment.NewLine;
                }

                device.Connect();
                output += "Friendly Name: " + device.FriendlyName;
                output += " Model: " + device.Model;
                output += " Device Type: " + device.DeviceType;

                device.Disconnect();
            }

            if (first)
            {
                output = "No devices found.";
            }

            txtBox.Text = output;

        }

        private void MessageBoxTutorialBrowse(string url)
        {
            MessageBoxTutorialBrowse(false, url);
        }

        private void MessageBoxTutorialBrowse(bool inProgress, string url)
        {
            string caption =
                "Feature Not Yet Implemented";

            if (inProgress)
            {
                caption += " (Tutorial In Progress)";
            }
            else
            {
                caption += " (Tutorial Not Started)";
            }

            if (MessageBox.Show("Browse To Tutorial?",
                                caption,
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start(url);
            }
        }


        private void MenuItemEnumerateContent_Click(object sender, RoutedEventArgs e)
        {//TODO: LICENSE NOTES
            //enumerating content: https://cgeers.wordpress.com/2011/06/05/wpd-enumerating-content/
            DisplayTextViewList();

            string msg = "This currently works, but takes FOREVER, so I " +
                "am working on borrowing some logic from jMTP to make " +
                "it quicker like my Java experiments. For now, please " +
                "only press OK if you are prepared to wait a really " +
                "long time (for testing) or you don't mind killing the " +
                "program manually...";

            if (MessageBox.Show(msg,
                                "Are you sure?",
                                MessageBoxButton.OKCancel,
                                MessageBoxImage.Exclamation) == MessageBoxResult.OK)
            {
                var col = new NwdPortableDeviceCollection();

                col.Refresh();

                string output = "";
                bool first = true;

                foreach (var device in col)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        output += Environment.NewLine;
                    }

                    device.Connect();
                    output += "Friendly Name: " + device.FriendlyName;
                    output += " Model: " + device.Model;
                    output += " Device Type: " + device.DeviceType;

                    var folder = device.GetContents();
                    foreach (var item in folder.Files)
                    {
                        output += Environment.NewLine;
                        output += DisplayObject(item);
                    }

                    device.Disconnect();
                }

                if (first)
                {
                    output = "No devices found.";
                }

                txtBox.Text = output;
            }
            else
            {
                txtBox.Text = "Operation aborted.";
            }
        }

        private void MenuItemDeletingResources_Click(object sender, RoutedEventArgs e)
        {//TODO: LICENSE NOTES
            //deleting resources: https://cgeers.wordpress.com/2012/04/15/wpd-deleting-resources/
            DisplayTextViewList();

            MessageBox.Show("This is implemented as a context menu option in my adapted NwdPortableDevice etc... " +
                "Being that the tutorial version takes too long on my device, I haven't implemented the demo code from the tutorial. Is what it is...");
        }

        private void MenuItemTransferringContent_Click(object sender, RoutedEventArgs e)
        {//TODO: LICENSE NOTES
            //transferring content: https://cgeers.wordpress.com/2011/08/13/wpd-transferring-content/
            DisplayTextViewList();

            string msg = "This currently works, but takes FOREVER, so I " +
                "am working on borrowing some logic from jMTP to make " +
                "it quicker like my Java experiments. For now, please " +
                "only press OK if you are prepared to wait a really " +
                "long time (for testing) or you don't mind killing the " +
                "program manually...";

            if (MessageBox.Show(msg,
                                "Are you sure?",
                                MessageBoxButton.OKCancel,
                                MessageBoxImage.Exclamation) == MessageBoxResult.OK)
            {
                var col = new NwdPortableDeviceCollection();

                col.Refresh();

                string output = "";
                bool first = true;

                foreach (var device in col)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        output += Environment.NewLine;
                    }

                    device.Connect();
                    output += "Friendly Name: " + device.FriendlyName;
                    output += " Model: " + device.Model;
                    output += " Device Type: " + device.DeviceType;

                    var folder = device.GetContents();
                    foreach (var item in folder.Files)
                    {
                        output += Environment.NewLine;
                        output += DisplayObject(item);
                    }

                    device.Disconnect();
                }

                if (first)
                {
                    output = "No devices found.";
                }

                txtBox.Text = output;
            }
            else
            {
                txtBox.Text = "Operation aborted.";
            }

        }

        private void MenuItemTreeViewChallenge_Click(object sender, RoutedEventArgs e)
        {
            string msg = "Tree View Challenge: Take the skills learned in 'Enumerating Devices' to populate a TreeView for all devices. " +
                "Then take the skills learned in 'Enumerating Content' to populate the items on those devices in a hierarchical representation.";

            MessageBox.Show(msg);
        }

        private void MenuItemTestToggleView_Click(object sender, RoutedEventArgs e)
        {
            if (ccParentChild.Visibility == Visibility.Collapsed)
            {
                DisplayParentChild();
                lvParent.Items.Add("Testing toggle ParentChild displayed");
            }
            else
            {
                DisplayTextViewList();
                txtBox.Text = "Testing toggle TextViewList displayed";
            }
        }

        private void DisplayTextViewList()
        {
            ccParentChild.Visibility = Visibility.Collapsed;
            ccTextBoxListView.Visibility = Visibility.Visible;
        }

        private void DisplayParentChild()
        {
            ccParentChild.Visibility = Visibility.Visible;
            ccTextBoxListView.Visibility = Visibility.Collapsed;
        }

        private void MenuItemGetDevices_Click(object sender, RoutedEventArgs e)
        {
            DisplayParentChild();

            lvParent.Items.Clear();
            lvChild.Items.Clear();

            var col = new NwdPortableDeviceCollection();

            try
            {
                col.Refresh();
            }
            catch (Exception ex)
            {
                Display.Exception(ex);
            }


            foreach (var device in col)
            {
                lvParent.Items.Add(device);
            }

        }

        private void lvParent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selected = lvParent.SelectedItem;

            if (selected is NwdPortableDevice)
            {
                lvChild.Items.Clear();

                NwdPortableDevice device = (NwdPortableDevice)selected;

                _currentDevice = device;

                if (device.RootFolder != null)
                {
                    foreach (NwdPortableDeviceObject pdo in
                            device.RootFolder.Files)
                    {
                        lvChild.Items.Add(pdo);
                    }
                }
            }
            else if (selected is NwdPortableDeviceFolder)
            {
                lvChild.Items.Clear();

                NwdPortableDeviceFolder folder =
                    (NwdPortableDeviceFolder)selected;

                folder.Refresh(_currentDevice);

                if (folder.Files != null)
                {
                    lvChild.Items.Clear();
                    foreach (NwdPortableDeviceObject pdo in folder.Files)
                    {
                        lvChild.Items.Add(pdo);
                    }
                }
                else
                {
                    MessageBox.Show("Files Null for Folder: " + folder.Name);
                }
            }
        }

        private void lvChild_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selected = lvChild.SelectedItem;

            if (selected is NwdPortableDeviceFolder)
            {
                lvParent.Items.Clear();

                NwdPortableDeviceFolder folder =
                    (NwdPortableDeviceFolder)selected;

                folder.Refresh(_currentDevice);

                if (folder.Files != null)
                {
                    lvParent.Items.Clear();
                    foreach (NwdPortableDeviceObject pdo in folder.Files)
                    {
                        lvParent.Items.Add(pdo);
                    }
                }
                else
                {
                    MessageBox.Show("Files Null for Folder: " + folder.Name);
                }
            }
        }

        private void MenuItemListViewParentDownload_Click(object sender, RoutedEventArgs e)
        {
            DownloadListViewSelectedItem(lvParent);
        }

        private void MenuItemListViewChildDownload_Click(object sender, RoutedEventArgs e)
        {
            DownloadListViewSelectedItem(lvChild);
        }

        private void DownloadListViewSelectedItem(ListView lv)
        {
            object obj = lv.SelectedItem;

            if (obj is NwdPortableDeviceFolder)
            {
                MessageBox.Show("folder download not supported yet");
            }

            if (obj is NwdPortableDeviceFile)
            {
                string folderPath = @"C:\NWD-SNDBX\MtpTesting";

                NwdPortableDeviceFile pdf = (NwdPortableDeviceFile)obj;

                _currentDevice.DownloadFile(pdf, folderPath);

                MessageBox.Show("file downloaded to " + folderPath);
            }

            if (obj == null)
            {
                MessageBox.Show("null selection");
            }
        }

        private void DeleteListViewSelectedItem(ListView lv)
        {
            object obj = lv.SelectedItem;

            if (obj is NwdPortableDeviceFolder)
            {
                MessageBox.Show("folder delete not supported yet");
            }

            if (obj is NwdPortableDeviceFile)
            {
                NwdPortableDeviceFile pdf = (NwdPortableDeviceFile)obj;

                if (MessageBox.Show("Are you sure you want to delete " + pdf.Name,
                                   "Confirm Deletion",
                                   MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _currentDevice.DeleteFile(pdf);

                    MessageBox.Show("file deleted");
                }

            }

            if (obj == null)
            {
                MessageBox.Show("null selection");
            }
        }

        private void MenuItemListViewParentDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteListViewSelectedItem(lvParent);
        }

        private void MenuItemListViewChildDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteListViewSelectedItem(lvChild);
        }

        private void MenuItemTransferringContentToDevice_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This one isn't even active, as my device uses " +
                "alphanumeric object ids with no human readable hierarchy. " +
                "It makes copying this tutorial verbatim non-functional, " +
                "but I have adapted the tutorial into a 'transfer file into " +
                "this folder' context menu option. The code in this example " +
                "won't run as I've disabled it, but its in the source for " +
                "reference sake.");

            //disabled
            if (false)
            {
                var devices = new NwdPortableDeviceCollection();
                devices.Refresh();
                var kindle = devices.First();
                kindle.Connect();

                kindle.TransferContentToDevice(
                    @"d:\temp\Kindle_Users_Guide.azw",
                    @"g:\documents");

                kindle.Disconnect();
            }
        }

        private void TransferContentToSelectedFolderOnDevice(ListView lv)
        {
            object obj = lv.SelectedItem;

            if (obj is NwdPortableDeviceFolder)
            {
                NwdPortableDeviceFolder folder = (NwdPortableDeviceFolder)obj;

                OpenFileDialog ofd = new OpenFileDialog();

                if (ofd.ShowDialog() == true)
                {
                    string msg = "Are you sure you want to transfer " + ofd.FileName +
                        " to the selected device folder " +
                        folder.Name + " (id: " + folder.Id + "?";

                    if (MessageBox.Show(msg,
                                       "Confirm Transfer",
                                       MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        _currentDevice.TransferContentToDevice(ofd.FileName, folder.Id);

                        MessageBox.Show("file transferred.");
                    }
                }
            }

            if (obj is NwdPortableDeviceFile)
            {
                MessageBox.Show("Select the folder you want to transfer" +
                    " into, cannot be a file object");

            }

            if (obj == null)
            {
                MessageBox.Show("null selection");
            }
        }

        private void MenuItemParentTransferFile_Click(object sender, RoutedEventArgs e)
        {
            TransferContentToSelectedFolderOnDevice(lvParent);
        }

        private void MenuItemChildTransferFile_Click(object sender, RoutedEventArgs e)
        {
            TransferContentToSelectedFolderOnDevice(lvChild);
        }

        private void MenuItemFindTopLevelFolder_Click(object sender, RoutedEventArgs e)
        {
            string uri = NineWorldsDeep.UI.Prompt.ForNwdUri();

            Parser.Parser p = new Parser.Parser();

            string nodeName = p.GetKeyNode(0, uri);

            MultiMap<string, NwdPortableDeviceObject> uriNodeToPdos =
                new MultiMap<string, NwdPortableDeviceObject>();

            var col = new NwdPortableDeviceCollection();

            try
            {
                col.Refresh();
            }
            catch (Exception ex)
            {
                Display.Exception(ex);
            }


            foreach (var device in col)
            {
                if (device.RootFolder != null)
                {
                    //device level
                    foreach (NwdPortableDeviceObject pdo
                        in device.RootFolder.Files)
                    {
                        //internal vs external storage level
                        if (pdo is NwdPortableDeviceFolder)
                        {
                            NwdPortableDeviceFolder folder =
                                (NwdPortableDeviceFolder)pdo;

                            folder.Refresh(device);

                            if (folder.Files != null)
                            {
                                foreach (NwdPortableDeviceObject pdoTopLevelFolderOrFile in folder.Files)
                                {
                                    //top level folder level

                                    if (pdoTopLevelFolderOrFile.Name.Equals(nodeName))
                                    {
                                        uriNodeToPdos.Add(nodeName, pdoTopLevelFolderOrFile);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            MessageBox.Show("found " + uriNodeToPdos[nodeName].Count() + " matching nodes");
        }

        private MultiMap<string, NwdPortableDeviceObject> Retrieve(List<string> nwdUris)
        {
            //TODO: this should be moved to some sort of utility class

            MultiMap<string, NwdPortableDeviceObject> found =
                new MultiMap<string, NwdPortableDeviceObject>();

            MessageBox.Show("not implemented yet, but goal is to perform Find By Uri on an entire list and return a MultiMap including all results");

            return found;
        }

        private void Retrieve(NwdPortableDevice device,
                              NwdPortableDeviceFolder folder,
                              ref NwdUri uri,
                              ref List<NwdPortableDeviceObject> found)
        {
            folder.Refresh(device);

            string nodeName = uri.PopNodeName();

            if (folder.Files != null)
            {
                foreach (NwdPortableDeviceObject pdo in folder.Files)
                {
                    if (NwdUri.MatchNodeName(nodeName, pdo.Name))
                    {
                        if (uri.HasNodeKeysInStack())
                        {
                            if (pdo is NwdPortableDeviceFolder)
                            {
                                NwdPortableDeviceFolder nextFolder =
                                    (NwdPortableDeviceFolder)pdo;

                                //uri has one level popped, pass the modified uri and 
                                //the newly found folder to the next level of recursion
                                Retrieve(device, nextFolder, ref uri, ref found);
                            }
                            else
                            {
                                //bad key (found name but not a folder and keystack isn't empty, so just return)

                                //do nothing
                            }
                        }
                        else
                        {
                            //finished
                            found.Add(pdo);
                        }
                    }
                }
            }
        }

        private void Retrieve(NwdPortableDevice device,
                              NwdPortableDeviceFolder folder,
                              ref IEnumerable<NwdUri> uris,
                              ref MultiMap<string,
                              NwdPortableDeviceObject> found)
        {
            folder.Refresh(device);

            foreach (NwdUri tempUri in uris)
            {
                NwdUri uri = tempUri;

                string nodeName = uri.PopNodeName();

                if (found.ContainsKey(uri.CurrentTraversalUri))
                {
                    //used cached lookup
                    foreach (NwdPortableDeviceObject pdo in found[uri.CurrentTraversalUri])
                    {
                        ProcessPdo(device, pdo, ref uri, ref found);
                    }
                }
                else
                {
                    //use standard lookup
                    if (folder.Files != null)
                    {
                        foreach (NwdPortableDeviceObject pdo in folder.Files)
                        {
                            if (NwdUri.MatchNodeName(nodeName, pdo.Name))
                            {
                                ProcessPdo(device, pdo, ref uri, ref found);
                            }
                        }
                    }
                }
            }
        }

        private void Retrieve(NwdPortableDevice device,
                              NwdPortableDeviceFolder folder,
                              ref NwdUri uri,
                              ref MultiMap<string, NwdPortableDeviceObject> found)
        {
            folder.Refresh(device);

            string nodeName = uri.PopNodeName();

            if (found.ContainsKey(uri.CurrentTraversalUri))
            {
                //used cached lookup
                foreach (NwdPortableDeviceObject pdo in found[uri.CurrentTraversalUri])
                {
                    ProcessPdo(device, pdo, ref uri, ref found);
                }
            }
            else
            {
                //use standard lookup
                if (folder.Files != null)
                {
                    foreach (NwdPortableDeviceObject pdo in folder.Files)
                    {
                        if (NwdUri.MatchNodeName(nodeName, pdo.Name))
                        {
                            ProcessPdo(device, pdo, ref uri, ref found);
                        }
                    }
                }
            }
        }

        private void ProcessPdo(NwdPortableDevice device,
                                NwdPortableDeviceObject pdo,
                                ref NwdUri uri,
                                ref MultiMap<string, NwdPortableDeviceObject> found)
        {
            if (uri.HasNodeKeysInStack())
            {
                if (pdo is NwdPortableDeviceFolder)
                {
                    NwdPortableDeviceFolder nextFolder =
                        (NwdPortableDeviceFolder)pdo;

                    found.Add(uri.CurrentTraversalUri, pdo);

                    //uri has one level popped, pass the modified uri and 
                    //the newly found folder to the next level of recursion
                    Retrieve(device, nextFolder, ref uri, ref found);
                }
                else
                {
                    //bad key (found name but not a folder and keystack isn't empty, so just return)

                    //do nothing
                }
            }
            else
            {
                //finished
                found.Add(uri.CurrentTraversalUri, pdo);
            }
        }

        private MultiMap<string, NwdPortableDeviceObject> FindByUriCached(IEnumerable<NwdUri> uris)
        {
            MultiMap<string, NwdPortableDeviceObject> masterMap =
                new MultiMap<string, NwdPortableDeviceObject>();

            var col = new NwdPortableDeviceCollection();


            try
            {
                col.Refresh();
            }
            catch (Exception ex)
            {
                Display.Exception(ex);
            }


            foreach (var device in col)
            {
                if (device.RootFolder != null)
                {
                    //device level
                    foreach (NwdPortableDeviceObject pdo
                        in device.RootFolder.Files)
                    {
                        //start a new multimap for each device/storage location
                        MultiMap<string, NwdPortableDeviceObject> found =
                            new MultiMap<string, NwdPortableDeviceObject>();

                        //need to reset Uris for each device/storage location
                        uris.ResetAll();

                        //internal vs external storage level
                        if (pdo is NwdPortableDeviceFolder)
                        {
                            NwdPortableDeviceFolder folder =
                                (NwdPortableDeviceFolder)pdo;

                            Retrieve(device, folder, ref uris, ref found);
                        }

                        //add found into master map
                        masterMap.AddAll(found);
                    }
                }
            }

            return masterMap;
        }

        private MultiMap<string, NwdPortableDeviceObject> FindByUriCached(string uri)
        {
            MultiMap<string, NwdPortableDeviceObject> masterMap =
                new MultiMap<string, NwdPortableDeviceObject>();

            NwdUri nwdUri = new NwdUri(uri);

            var col = new NwdPortableDeviceCollection();


            try
            {
                col.Refresh();
            }
            catch (Exception ex)
            {
                Display.Exception(ex);
            }


            foreach (var device in col)
            {
                if (device.RootFolder != null)
                {
                    //device level
                    foreach (NwdPortableDeviceObject pdo
                        in device.RootFolder.Files)
                    {
                        //start a new multimap for each device/storage location
                        MultiMap<string, NwdPortableDeviceObject> found =
                            new MultiMap<string, NwdPortableDeviceObject>();

                        //need to reset Uri for each device/storage location
                        nwdUri.ResetStack();

                        //internal vs external storage level
                        if (pdo is NwdPortableDeviceFolder)
                        {
                            NwdPortableDeviceFolder folder =
                                (NwdPortableDeviceFolder)pdo;

                            Retrieve(device, folder, ref nwdUri, ref found);
                        }

                        //add found into master map
                        masterMap.AddAll(found);
                    }
                }
            }

            return masterMap;
        }

        private List<NwdPortableDeviceObject> FindByUri(string uri)
        {
            List<NwdPortableDeviceObject> found =
                new List<NwdPortableDeviceObject>();

            NwdUri nwdUri = new NwdUri(uri);

            var col = new NwdPortableDeviceCollection();


            try
            {
                col.Refresh();
            }
            catch (Exception ex)
            {
                Display.Exception(ex);
            }


            foreach (var device in col)
            {
                if (device.RootFolder != null)
                {
                    //device level
                    foreach (NwdPortableDeviceObject pdo
                        in device.RootFolder.Files)
                    {
                        //need to reset Uri for each device/storage location
                        nwdUri.ResetStack();

                        //internal vs external storage level
                        if (pdo is NwdPortableDeviceFolder)
                        {
                            NwdPortableDeviceFolder folder =
                                (NwdPortableDeviceFolder)pdo;

                            Retrieve(device, folder, ref nwdUri, ref found);
                        }
                    }
                }
            }

            return found;
        }

        private void MenuItemFindByUri_Click(object sender, RoutedEventArgs e)
        {
            string uri = NineWorldsDeep.UI.Prompt.ForNwdUri();

            List<NwdPortableDeviceObject> pdos = FindByUri(uri);

            if (pdos.Count() == 0)
            {
                MessageBox.Show("nothing found for at " + uri);
            }
            else
            {
                int count = 0;

                foreach (NwdPortableDeviceObject pdo in pdos)
                {
                    count += 1;

                    if (pdo is NwdPortableDeviceFolder)
                    {
                        MessageBox.Show(count + " folder(s) found at " + uri);
                    }

                    if (pdo is NwdPortableDeviceFile)
                    {
                        MessageBox.Show(count + " file(s) found at " + uri);
                    }
                }
            }
        }

        private void MenuItemRetrieveByUriList_Click(object sender, RoutedEventArgs e)
        {
            Retrieve(null);
        }

        private void MenuItemFindToBeRemovedPlayList_Click(object sender, RoutedEventArgs e)
        {
            //just supporting first attached device for now, can cycle through multiples later
            NwdPortableDeviceCollection col = new NwdPortableDeviceCollection();

            try
            {
                col.Refresh();
            }
            catch (Exception ex)
            {
                Display.Exception(ex);
            }

            if (col.Count() > 0)
            {
                NwdPortableDevice device = col.First();

                if (device != null)
                {
                    //if(unprocessedDeletions == null || 
                    //    unprocessedDeletions.Count < 1)
                    if (unprocessedDeletions.IsEmptyOrNull())
                    {
                        toBeRemoved = GetFirstToBeRemovedPlaylist();

                        if (toBeRemoved == null)
                        {
                            MessageBox.Show("Playlist(s) 'to be removed[*]' not found");
                        }
                        else
                        {
                            MessageBox.Show("found [" + toBeRemoved.Name + "]");

                            //copy file to local file system
                            string localDestinationFolder = Configuration.PlaylistsFolder;
                            device.DownloadFile(toBeRemoved, localDestinationFolder);

                            string localFilePath =
                                System.IO.Path.Combine(localDestinationFolder, toBeRemoved.Name);

                            MessageBox.Show("copied file to [" + localFilePath + "]");

                            List<string> entries = File.ReadAllLines(localFilePath).ToList();

                            MessageBox.Show("found " + entries.Count + " entries");

                            List<NwdUri> lst = new List<NwdUri>();
                            List<string> invalidPaths = new List<string>();

                            foreach (string entry in entries)
                            {
                                NwdUri newUri = Configuration.NwdPathToNwdUri(entry);
                                if (newUri != null)
                                {
                                    lst.Add(newUri);
                                }
                                else
                                {
                                    invalidPaths.Add(entry);
                                }
                            }

                            var pathList = (from entry in invalidPaths
                                            select new
                                            {
                                                Path = entry
                                            }).ToList();

                            if (pathList.Count > 0)
                            {
                                Display.Grid(invalidPaths.Count + " invalid paths skipped", lst, pathList);
                            }
                            else
                            {
                                Display.Grid("0 invalid paths skipped", lst);
                            }

                            var processEntries = (from nwdUri in lst
                                                  select new NwdUriProcessEntry(nwdUri)).ToList();

                            unprocessedDeletions =
                                new Stack<NwdUriProcessEntry>(processEntries);
                        }


                    }
                    else
                    {
                        MessageBox.Show(unprocessedDeletions.Count
                            + " unprocessed entries found for ["
                            + toBeRemoved.Name + "]");
                    }

                    if (unprocessedDeletions != null)
                    {

                        List<NwdUriProcessEntry> processed =
                        new List<NwdUriProcessEntry>();

                        bool again = true;
                        string msg = "Enter processing segment size (keep in mind that " +
                            "caching works better the higher this number is, but the " +
                            "higher the number, the longer each iteration will obviously " +
                            "take, find a balance that works for your device)";

                        while (again && unprocessedDeletions.Count() > 0)
                        {
                            int repetitionSegmentSize =
                            NineWorldsDeep.UI.Prompt.ForInteger(msg);

                            IEnumerable<NwdUriProcessEntry> currentlyProcessing =
                                unprocessedDeletions.Pop(repetitionSegmentSize);

                            Stopwatch watch = Stopwatch.StartNew();
                            var res = FindByUriCached(currentlyProcessing.ToNwdUris());
                            watch.Stop(); //just profiling find by uri

                            foreach (NwdUriProcessEntry pe in currentlyProcessing)
                            {
                                if (res[pe.URI].Count > 0)
                                {
                                    pe.DeviceObject = res[pe.URI].First();
                                }

                                pe.Processed = true;
                                processed.Add(pe);
                            }

                            again =
                                NineWorldsDeep.UI.Prompt.Confirm("Processing took "
                                + watch.Elapsed.ToString() +
                                " milliseconds to complete. Process more entries?");
                        }

                        Display.Grid(processed.Count() + " processed / "
                            + unprocessedDeletions.Count() + " unprocessed", processed);

                        if (NineWorldsDeep.UI.Prompt.Confirm("Delete all processed and found files?"))
                        {
                            int deletionCount = 0;

                            foreach (NwdUriProcessEntry pe in processed)
                            {
                                if (pe.FoundOnDevice)
                                {
                                    NwdPortableDeviceObject pdo = pe.DeviceObject;

                                    if (pdo is NwdPortableDeviceFile)
                                    {
                                        try
                                        {
                                            NwdPortableDeviceFile pdf =
                                                (NwdPortableDeviceFile)pdo;

                                            device.DeleteFile(pdf);
                                            deletionCount++;
                                            MessageBox.Show(deletionCount + " files deleted");
                                        }
                                        catch(Exception ex)
                                        {
                                            Display.Exception(ex);
                                        }
                                    }
                                }
                            }


                            if (unprocessedDeletions.Count < 1 && toBeRemoved != null)
                            {
                                string confirmMsg =
                                    "No more unprocessed entries for [" +
                                    toBeRemoved.Name + "] delete file?";

                                if (NineWorldsDeep.UI.Prompt.Confirm(confirmMsg))
                                {
                                    device.DeleteFile(toBeRemoved);

                                    MessageBox.Show("[" + toBeRemoved.Name + "] deleted.");

                                    unprocessedDeletions = null;
                                    toBeRemoved = null;
                                }
                            }
                        }

                    }
                }
            }
            else
            {
                MessageBox.Show("no devices found.");
            }
        }

        private NwdPortableDeviceFile GetFirstToBeRemovedPlaylist()
        {
            //List<NwdPortableDeviceObject> lst = FindByUri("Playlists/to be removed[*]");

            //if(lst.Count > 0 && lst.First() is NwdPortableDeviceFile)
            //{
            //    return (NwdPortableDeviceFile)lst.First();
            //}

            //return null;

            return GetFirstFileByUri("Playlists/to be removed[*]");
        }

        private NwdPortableDeviceFile GetFirstFileByUri(string uri)
        {
            List<NwdPortableDeviceObject> lst = FindByUri(uri);

            if (lst.Count > 0 && lst.First() is NwdPortableDeviceFile)
            {
                return (NwdPortableDeviceFile)lst.First();
            }

            return null;
        }

        private void MenuItemFindSynergyFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //just supporting first attached device for now
                NwdPortableDeviceCollection col =
                    new NwdPortableDeviceCollection();
                col.Refresh();

                if (col.Count() > 0)
                {
                    NwdPortableDevice device = col.First();

                    if (device != null)
                    {
                        if (unprocessedSynergyFiles == null ||
                            unprocessedSynergyFiles.Count < 1)
                        {
                            IEnumerable<NwdUriProcessEntry> synergyFiles =
                                GetDeviceSynergyFiles(device);

                            unprocessedSynergyFiles =
                                new Stack<NwdUriProcessEntry>(synergyFiles);

                            Display.Grid(synergyFiles.Count() +
                                " synergy files found", unprocessedSynergyFiles);
                        }
                        else
                        {
                            string msg = unprocessedSynergyFiles.Count +
                                " unprocessed synergy file entries found";

                            Display.Message(msg);
                        }

                        if (unprocessedSynergyFiles != null)
                        {
                            List<NwdUriProcessEntry> processed =
                                new List<NwdUriProcessEntry>();

                            bool again = true;
                            string msg = "Enter processing segment size";

                            while (again && unprocessedSynergyFiles.Count() > 0)
                            {
                                int repetitionSegmentSize =
                                    NineWorldsDeep.UI.Prompt.ForInteger(msg);

                                IEnumerable<NwdUriProcessEntry> currentlyProcessing =
                                    unprocessedSynergyFiles.Pop(repetitionSegmentSize);

                                int copiedFilesCount = 0;
                                Stopwatch watch = Stopwatch.StartNew();

                                foreach (NwdUriProcessEntry pe in currentlyProcessing)
                                {
                                    if (pe.DeviceObject is NwdPortableDeviceFile)
                                    {
                                        NwdPortableDeviceFile pdf =
                                            (NwdPortableDeviceFile)pe.DeviceObject;

                                        string localFolderPath =
                                            Configuration.MtpSynergySyncPath;

                                        device.DownloadFile(pdf, localFolderPath);

                                        copiedFilesCount++;
                                    }

                                    pe.Processed = true;
                                    processed.Add(pe);
                                }

                                watch.Stop();

                                string displayMsg = processed.Count +
                                    " processed / " +
                                    unprocessedSynergyFiles.Count +
                                    " unprocessed";

                                Display.Grid(displayMsg,
                                             processed,
                                             unprocessedSynergyFiles);

                                again =
                                NineWorldsDeep.UI.Prompt.Confirm("Processing time: "
                                + watch.Elapsed.ToString() +
                                " to process " + copiedFilesCount +
                                " entries. Process more entries?");
                            }

                        }

                    }
                }
                else
                {
                    Display.Message("no devices found");
                }

            }
            catch (Exception ex)
            {
                Display.Exception(ex);
            }
        }

        private IEnumerable<NwdUriProcessEntry>
            GetDeviceSynergyFiles(NwdPortableDevice device)
        {
            List<NwdPortableDeviceObject> lst = FindByUri("NWD/synergy/[*]");
            List<NwdUriProcessEntry> output =
                new List<NwdUriProcessEntry>();

            if (lst.Count > 0)
            {
                foreach (NwdPortableDeviceObject pdo in lst)
                {
                    if (pdo is NwdPortableDeviceFile)
                    {
                        NwdUri uri = new NwdUri("NWD/synergy/" + pdo.Name);
                        NwdUriProcessEntry pe =
                            new NwdUriProcessEntry(uri);

                        pe.DeviceObject = pdo;

                        output.Add(pe);
                    }
                }
            }

            return output;
        }

        private void MenuItemFindHashedMediaFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //just supporting first attached device for now
                NwdPortableDeviceCollection col =
                    new NwdPortableDeviceCollection();

                col.Refresh();

                if (col.Count() > 0)
                {
                    NwdPortableDevice device = col.First();

                    if (device != null)
                    {
                        if (unprocessedHashedMedia.IsEmptyOrNull())
                        {
                            IEnumerable<NwdUriProcessEntry> hashedMedia =
                                GetDeviceHashedMedia(device);

                            unprocessedHashedMedia =
                                new Stack<NwdUriProcessEntry>(hashedMedia);

                            string msg = unprocessedHashedMedia.Count() +
                                " hashed media entries found";

                            Display.Grid(msg, unprocessedHashedMedia);
                        }
                        else
                        {
                            string msg = unprocessedHashedMedia.Count +
                                " unprocessed hashed media entries found";

                            Display.Message(msg);
                        }

                        if (unprocessedHashedMedia != null)
                        {
                            //MessageBox.Show("processing goes here");

                            List<NwdUriProcessEntry> processed =
                                new List<NwdUriProcessEntry>();

                            bool again = true;
                            string msg = "Enter processing segment size";

                            while (again && unprocessedHashedMedia.Count() > 0)
                            {
                                int repetitionSegmentSize =
                                    NineWorldsDeep.UI.Prompt.ForInteger(msg);

                                IEnumerable<NwdUriProcessEntry> currentlyProcessing =
                                    unprocessedHashedMedia.Pop(repetitionSegmentSize);

                                int processedFileCount = 0;

                                //TODO: make FindByUri/FindByUriCached faster, or create another method, this takes so long I am commenting it out for now and just supporting the storing of hash information
                                //Stopwatch watch = Stopwatch.StartNew();
                                //var res = FindByUriCached(currentlyProcessing.ToNwdUris());
                                //watch.Stop();

                                foreach (NwdUriProcessEntry pe in currentlyProcessing)
                                {
                                    //TODO: make FindByUri/FindByUriCached faster, or create another method, this takes so long I am commenting it out for now and just supporting the storing of hash information
                                    //if (res[pe.URI].Count > 0)
                                    //{
                                    //    pe.DeviceObject = res[pe.URI].First();
                                    //}

                                    pe.PortableDevice = device;
                                    pe.Processed = true;
                                    processed.Add(pe);

                                    processedFileCount++;
                                }



                                again =
                                NineWorldsDeep.UI.Prompt.Confirm("Processing time: " +
                                //watch.Elapsed.ToString() + //TODO: make FindByUri/FindByUriCached faster, or create another method, this takes so long I am commenting it out for now and just supporting the storing of hash information
                                " to process " + processedFileCount +
                                " entries. Process more entries?");
                            }

                            string displayMsg = processed.Count +
                                    " processed / " +
                                    unprocessedHashedMedia.Count +
                                    " unprocessed";

                            Display.Grid(displayMsg,
                                         processed,
                                         unprocessedHashedMedia);

                            if (NineWorldsDeep.UI.Prompt.Confirm("update hashes in database?"))
                            {
                                SqliteDbAdapter db = new SqliteDbAdapter();

                                db.StoreHashes(processed);
                                db.StorePaths(processed);
                                db.StoreDevice(device);
                                db.PopulateIds(processed);

                                Display.Grid("ids populated", processed);

                                db.StoreHashPathJunctions(processed);

                                Display.Message("all items propagated to db");
                            }

                            if (NineWorldsDeep.UI.Prompt.Confirm("backup all new files?"))
                            {
                                Display.Message("backup not supported until FindByUri/FindByUriCached is made faster, or replaced by something else. It takes 40 seconds to process 10 entries and there are over 1300, I think maybe indexing the object ids and just double checking them on access (cause I'm not sure if they could change so we want to be safe) might help, so the indexing could be a long running process that would only have to be run once on the large set, and then just for changes going forward. Just spit-ballin'.");
                                //check hashes in database, any not associated with a local path
                                //add to list of process entries to be downloaded
                                //also, any file not hashed should be downloaded as well
                                //better to err on the side of caution
                                //we can index our external and internal hard drives later
                                //to verify any duplicates, which can then be removed                               
                            }
                        }
                    }
                }
                else
                {
                    Display.Message("no devices found");
                }

            }
            catch (Exception ex)
            {
                Display.Exception(ex);
            }
        }

        private IEnumerable<NwdUriProcessEntry>
            GetDeviceHashedMedia(NwdPortableDevice device)
        {
            string uri = "NWD/config/FileHashIndex.txt";
            NwdPortableDeviceFile hashedMediaIndex =
                GetFirstFileByUri(uri);

            if (hashedMediaIndex == null)
            {
                Display.Message("[" + uri + "] not found");
                return new List<NwdUriProcessEntry>(); //empty list
            }
            else
            {
                Display.Message("found [" + uri + "]");

                //copy to local file system
                string localFilePath =
                    Configuration.NwdUriToLocalPath(uri);

                //Display.Message(localFilePath);

                string localDestinationFolder =
                    Path.GetDirectoryName(localFilePath);

                //Display.Message(localDestinationFolder);

                device.DownloadFile(hashedMediaIndex,
                                    localDestinationFolder);

                Display.Message("copied file to [" + localFilePath + "]");

                List<string> lineItems =
                    File.ReadAllLines(localFilePath).ToList();

                Display.Message("found " + lineItems.Count + " lineItems");

                Parser.Parser p = new Parser.Parser();
                List<NwdUri> lst = new List<NwdUri>();
                List<string> invalidPaths = new List<string>();

                foreach (string li in lineItems)
                {
                    string path = p.Extract("path", li);
                    string hash = p.Extract("sha1Hash", li);
                    List<string> whitelistedFolders =
                        new List<string>();
                    whitelistedFolders.Add("Pictures");
                    whitelistedFolders.Add("DCIM");
                    //NwdUri newUri = Configuration.NwdPathToNwdUri(path);
                    NwdUri newUri = Configuration.NwdPathToNwdUri(path, whitelistedFolders);
                    if (newUri != null)
                    {
                        newUri.Hash = hash;
                        newUri.Path = path;
                        lst.Add(newUri);
                    }
                    else
                    {
                        invalidPaths.Add(path);
                    }
                }

                var pathList = (from li in invalidPaths
                                select new
                                {
                                    Path = li
                                }).ToList();

                if (pathList.Count > 0)
                {
                    Display.Grid(invalidPaths.Count + " invalid paths skipped", lst, pathList);
                }
                else
                {
                    Display.Grid("0 invalid paths skipped", lst);
                }

                var processEntries =
                    (from nwdUri in lst
                     select new NwdUriProcessEntry(nwdUri)).ToList();

                return processEntries;
            }
        }

        private void MenuItemFindSynergyFilesDelegate_Click(object sender, RoutedEventArgs e)
        {
            string msg = "There is a lot of code repetition in the various " +
                "processing based methods, and I feel that we could reduce " +
                "this redundancy by taking just the differing processing " +
                "code and wrapping it in delegates, then having the " +
                "redundant code, like the part that handles " +
                "while(again && notEmpty()), in a common method that can " +
                "just be passed the delegate (hope that makes sense, " +
                "just spit balling here).";

            Display.Message(msg);
        }
    }
}
