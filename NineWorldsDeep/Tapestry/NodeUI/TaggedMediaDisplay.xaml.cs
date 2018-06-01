using NineWorldsDeep.Core;
using NineWorldsDeep.Model;
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
using System.Collections;
using System.IO;
using NineWorldsDeep.Tapestry.Nodes;
using NineWorldsDeep.Mnemosyne.V5;
using System.Threading;
using NineWorldsDeep.Hive;
using System.Xml.Linq;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for TaggedMediaDisplay.xaml
    /// </summary>
    public partial class TaggedMediaDisplay : UserControl, IAsyncStatusResponsive
    {
        #region fields

        private TaggingMatrix taggingMatrix;
        Db.Sqlite.MediaV5SubsetDb db;

        //async related 
        private readonly SynchronizationContext syncContext;
        private DateTime previousTime = DateTime.Now;

        #endregion

        #region creation

        public TaggedMediaDisplay()
        {
            InitializeComponent();
            syncContext = SynchronizationContext.Current;

            db = new Db.Sqlite.MediaV5SubsetDb();
            
        }

        #endregion

        #region event PathSelected

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

        #endregion

        #region private helper methods

        private void LoadTags()
        {
            lvTags.ItemsSource = FilterTaggingMatrix(taggingMatrix, txtTagFilter.Text);
        }

        private void LoadPaths()
        {
            //lvPaths.ItemsSource = FilterPaths(GetPathsForSelectedTag(), txtPathFilter.Text);
            lvPaths.ItemsSource = FilterPaths(GetPathsForSelectedTags(), txtPathFilter.Text);
        }

        /// <summary>
        /// all selected tags
        /// </summary>
        /// <returns></returns>
        private List<string> GetPathsForSelectedTags()
        {
            List<string> paths = new List<string>();

            List<string> selectedTags =
                lvTags.SelectedItems.Cast<TagCountDisplayItem>()
                                    .Select(t => t.Tag)
                                    .ToList();

            foreach(string tag in selectedTags)
            {
                paths.AddRange(taggingMatrix.GetPathsForTag(tag));
            }

            //return paths;
            return paths.Distinct().OrderByDescending(s => s).ToList();
        }

        /// <summary>
        /// just a single tag
        /// </summary>
        /// <returns></returns>
        private List<string> GetPathsForSelectedTag()
        {
            List<string> paths = new List<string>();
            TagCountDisplayItem selected = (TagCountDisplayItem)lvTags.SelectedItem;

            if (taggingMatrix != null && selected != null)
            {
                paths = taggingMatrix.GetPathsForTag(selected.Tag);
            }

            return paths;
        }

        private List<string> FilterPaths(List<string> paths, string pathFilter)
        {
            return paths.Where(p => p.ToLower().Contains(pathFilter.ToLower())).ToList();            
        }

        private void RefreshTaggingMatrix()
        {
            bool includeNonLocal = chkIncludeNonLocalFiles.IsChecked.Value;

            //TODO: if this takes long, make async and update status per section
            TaggingMatrix tm = db.RetrieveTaggingMatrix(includeNonLocal);

            //tm.AddFolderAndAllSubfolders(Configuration.ImagesFolder);
            //tm.AddFolderAndAllSubfolders(Configuration.VoiceMemosFolder);
            //tm.AddFolderAndAllSubfolders(Configuration.PdfsFolder);

            foreach(string folderPath in Configuration.GetAllEcosystemMediaFolders())
            {
                tm.AddFolderAndAllSubfolders(folderPath);
            }

            taggingMatrix = tm;

            LoadTags();
        }

        private List<TagCountDisplayItem> FilterTaggingMatrix(TaggingMatrix tm, string filter)
        {
            List<TagCountDisplayItem> items = new List<TagCountDisplayItem>();

            List<string> tags = tm.Tags;

            tags = tags.Where(tag => tag.ToLower().Contains(filter.ToLower())).ToList();

            tags.Sort();

            //get counts of tagged paths
            foreach (string tag in tags)
            {
                items.Add(new TagCountDisplayItem()
                {
                    Tag = tag,
                    Count = tm.PathsForTag(tag).Count
                });
            }

            var allTag = tm.GenerateAllPathsDisplayItem();
            var untaggedTag = tm.GenerateUntaggedPathsDisplayItem();

            items = items.OrderBy(i => i.Tag).ToList();

            items.Insert(0, allTag);
            items.Insert(1, untaggedTag);

            return items;
        }

        #endregion

        #region public interface

        public void StatusDetailUpdate(string text, bool ensureDisplay = false)
        {
            var currentTime = DateTime.Now;

            if (!ensureDisplay && ((DateTime.Now - previousTime).Milliseconds <= 50)) return;

            syncContext.Post(new SendOrPostCallback(s =>
            {
                tbStatus.Text = (string)s;
            }), text);

            previousTime = currentTime;
        }

        #endregion

        #region event handlers

        private void lvPaths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //adapted from MediaMasterDisplay
            List<string> selectedPaths = lvPaths.SelectedItems.Cast<string>().ToList();

            if (selectedPaths.Count > 0)
            {
                //display first selected
                string firstPath = selectedPaths[0];

                //if (File.Exists(firstPath) && db.LocalDeviceId > 0)
                //{
                //    PathSelectedEventArgs args =
                //        new PathSelectedEventArgs(
                //            new FileSystemNode(firstPath, true, db.LocalDeviceId));

                //    OnPathSelected(args);
                //}

                var deviceId = -1;

                if (File.Exists(firstPath) && db.LocalDeviceId > 0)
                {
                    deviceId = db.LocalDeviceId;
                }
                
                PathSelectedEventArgs args =
                    new PathSelectedEventArgs(
                        new FileSystemNode(firstPath, true, deviceId));

                OnPathSelected(args);
            }
        }

        private void lvTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //from VerticalTaggerGrid (need to refactor to use V5, &c.)
            //TaggerGridController.LoadFromSelectedTag();

            LoadPaths();
        }

        private void txtTagFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadTags();
            }
        }

        private void txtPathFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                LoadPaths();
            }
        }

        private void btnRefreshTaggingMatrix_Click(object sender, RoutedEventArgs e)
        {
            RefreshTaggingMatrix();
        }

        private async void btnHashAndResyncPaths_Click(object sender, RoutedEventArgs e)
        {            
            var pathList = lvPaths.Items.Cast<string>().ToList();

            string msg = "you are about to hash and sync " + pathList.Count + 
                " files, this could take a while. Are you sure you want to proceed?";

            if (UI.Prompt.Confirm(msg, true))
            {
                await Task.Run(() =>
                {
                    List<Media> mediaList = new List<Media>();
                    foreach (string path in pathList)
                    {
                        StatusDetailUpdate("preparing " + path + " for sync");

                        var mli = new MediaListItem(path);
                        mli.HashMedia();
                        mediaList.Add(mli.Media);
                    }

                    db.SyncAsync(mediaList, this);

                    StatusDetailUpdate("finished.");
                });
            }
            else
            {
                UI.Display.Message("resync cancelled");
            }
        }

        private void MenuItemStageForExport_Click(object sender, RoutedEventArgs e)
        {
            //get selected items
            List<string> selectedPaths =
                lvPaths.SelectedItems.Cast<string>()
                                     .Select(s => s)
                                     .ToList();
            
            UtilsMnemosyneV5.StageForExportByPath(selectedPaths);

            UI.Display.Message("finished staging for export");
        }

        private async void MenuItemCopyToStagingHive_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedPaths =
            lvPaths.SelectedItems.Cast<string>()
                                 .Select(s => s)
                                 .ToList();

            await Task.Run(() =>
            {
                try
                {
                    this.StatusDetailUpdate("processing " + selectedPaths.Count() + " paths");

                    UtilsHive.CopyToStaging(selectedPaths, this);

                    //copy all tags to hive xml folders 
                    //UtilsMnemosyneV5.ExportXml(this, selectedPaths, ConfigHive.GetHiveFoldersForXmlExport());
                    
                }
                catch(Exception ex)
                {
                    var debuggingBreakpoint = ex.Message;
                }
            });

            UI.Display.Message("copied to hive staging. be sure to export xml, if desired, using other menu option, this doesn't export tags, just the media files");
        }
        
        private async void MenuItemExportHiveXml_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedPaths =
            lvPaths.SelectedItems.Cast<string>()
                                 .Select(s => s)
                                 .ToList();

            await Task.Run(() =>
            {
                try
                {
                    this.StatusDetailUpdate("processing " + selectedPaths.Count() + " paths");

                    var mediaList = new List<Media>();

                    foreach(string filePath in selectedPaths)
                    {
                        MediaListItem mli;

                        if (File.Exists(filePath))
                        {
                            this.StatusDetailUpdate("hashing: " + filePath);

                            mli = new MediaListItem(filePath);
                            mli.HashMedia();
                        }
                        else
                        {
                            this.StatusDetailUpdate("nonlocal file, retrieving hash from database: " + filePath);
                            //sync by path
                            string mediaHash = Configuration.DB.MediaSubset.GetMediaHashByPath(filePath);
                            string deviceName = Configuration.DB.MediaSubset.GetMediaDeviceNameByPath(filePath);
                            mli = new MediaListItem(filePath, deviceName, mediaHash);
                        }

                        mediaList.Add(mli.Media);
                    }

                    db.SyncAsync(mediaList, this);

                    

                    XElement mnemosyneSubsetEl = new XElement(Xml.Xml.TAG_MNEMOSYNE_SUBSET);

                    foreach (var media in mediaList)
                    {
                        //create media tag with attribute set for hash
                        XElement mediaEl = 
                            Xml.Xml.CreateMediaElement(media.MediaHash);

                        foreach (MediaTagging tag in media.MediaTaggings)
                        {
                            //create tag element and append to 
                            XElement tagEl = Xml.Xml.CreateTagElement(tag);
                            mediaEl.Add(tagEl);
                        }

                        foreach (string deviceName in media.DevicePaths.Keys)
                        {
                            XElement deviceEl = Xml.Xml.CreateDeviceElement(deviceName);

                            foreach (DevicePath path in media.DevicePaths[deviceName])
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

                    string fileName =
                        NwdUtils.GetTimeStamp_yyyyMMddHHmmss() + "-nwd-mnemosyne-v5.xml";
                    
                    //write to temp file
                    var tempFolder = Configuration.TempV5XmlFolder;
                    var xmlTempFilePath = 
                        System.IO.Path.Combine(tempFolder, fileName);

                    doc.Save(xmlTempFilePath);

                    var filePathInList = new List<string>();
                    filePathInList.Add(xmlTempFilePath);

                    //just checked, we can do this and all the hive folder magic will happen
                    //UtilsHive.CopyToStaging(filePathInList, this);
                    UtilsHive.CopyToAllActiveRoots(filePathInList);
                }
                catch (Exception ex)
                {
                    var debuggingBreakpoint = ex.Message;
                }
            });

            //UI.Display.Message("xml copied to hive staging. (On mobile device, be sure to intake Xml from stagin and then use Import Mnemosyne Hive Xml transfer option)");
            UI.Display.Message("xml copied to hive, on mobile device use Import Mnemosyne Hive Xml transfer option)");
        }

        private void MenuItemSendToTrash_Click(object sender, RoutedEventArgs e)
        {
            //get selected items
            List<string> selectedPaths =
                lvPaths.SelectedItems.Cast<string>()
                                     .Select(s => s)
                                     .ToList();

            string msg = "Are you sure you want to move these " + 
                selectedPaths.Count + " files to the trash?";

            if (UI.Prompt.Confirm(msg, true))            {

                UtilsMnemosyneV5.MoveToTrash(selectedPaths);

                UI.Display.Message("files trashed");
            }
        }

        private void MenuItemCopyFileNamesToClipboard_Click(object sender, RoutedEventArgs e)
        {
            //get selected items
            List<string> selectedPaths =
                lvPaths.SelectedItems.Cast<string>()
                                     .Select(s => System.IO.Path.GetFileName(s))
                                     .Distinct()
                                     .ToList();

            

            string pathsAsMultiLineString = string.Join(Environment.NewLine, selectedPaths);

            Clipboard.SetText(pathsAsMultiLineString);

            UI.Display.Message("copied to clipboard: " + 
                Environment.NewLine + pathsAsMultiLineString);
        }

        private async void MenuItemCopyFileNamesWithTagsInBrackets_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedPaths =
            lvPaths.SelectedItems.Cast<string>()
                                 .Select(s => s)
                                 .ToList();

            List<string> singleLineFileNamesAndTags = new List<string>();
            
            await Task.Run(() =>
            {
                try
                {
                    this.StatusDetailUpdate("processing " + selectedPaths.Count() + " paths");
                    
                    var fileNamesToMedia = new Dictionary<string, Media>();

                    foreach (string filePath in selectedPaths)
                    {
                        string mediaFileName = System.IO.Path.GetFileName(filePath);
                        MediaListItem mli;

                        if (File.Exists(filePath))
                        {
                            this.StatusDetailUpdate("hashing: " + filePath);

                            mli = new MediaListItem(filePath);
                            mli.HashMedia();
                        }
                        else
                        {
                            this.StatusDetailUpdate("nonlocal file, retrieving hash from database: " + filePath);
                            //sync by path
                            string mediaHash = Configuration.DB.MediaSubset.GetMediaHashByPath(filePath);
                            string deviceName = Configuration.DB.MediaSubset.GetMediaDeviceNameByPath(filePath);
                            mli = new MediaListItem(filePath, deviceName, mediaHash);
                        }

                        fileNamesToMedia[mediaFileName] = mli.Media;
                    }

                    db.SyncAsync(fileNamesToMedia.Values, this);
                    
                    foreach (var fileName in fileNamesToMedia.Keys)
                    {
                        var media = fileNamesToMedia[fileName];

                        //foreach (MediaTagging tag in media.MediaTaggings)
                        //{
                        //    //create tag element and append to 
                        //    XElement tagEl = Xml.Xml.CreateTagElement(tag);
                        //    mediaEl.Add(tagEl);
                        //}

                        //tags to string
                        List<string> tagList =
                            media.MediaTaggings.Select(m => m.MediaTagValue)
                                               .Distinct()
                                               .ToList();
                       
                        string tagString = string.Join(", ", tagList);

                        string singleLineFileNameAndTags = fileName + " [<[" + tagString + "]>]";

                        singleLineFileNamesAndTags.Add(singleLineFileNameAndTags);
                    }
 
                }
                catch (Exception ex)
                {
                    var debuggingBreakpoint = ex.Message;
                }
            });

            string namesAndTagsAsMultiLineString = string.Join(Environment.NewLine, singleLineFileNamesAndTags);

            Clipboard.SetText(namesAndTagsAsMultiLineString);

            UI.Display.Message("copied to clipboard: " +
                Environment.NewLine + namesAndTagsAsMultiLineString);
        }

        private async void MenuItemCopyFileNamesWithTagsTabbedRight_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedPaths =
            lvPaths.SelectedItems.Cast<string>()
                                 .Select(s => s)
                                 .ToList();

            List<string> singleLineFileNamesAndTags = new List<string>();

            await Task.Run(() =>
            {
                try
                {
                    this.StatusDetailUpdate("processing " + selectedPaths.Count() + " paths");

                    var fileNamesToMedia = new Dictionary<string, Media>();

                    foreach (string filePath in selectedPaths)
                    {
                        string mediaFileName = System.IO.Path.GetFileName(filePath);
                        MediaListItem mli;

                        if (File.Exists(filePath))
                        {
                            this.StatusDetailUpdate("hashing: " + filePath);

                            mli = new MediaListItem(filePath);
                            mli.HashMedia();
                        }
                        else
                        {
                            this.StatusDetailUpdate("nonlocal file, retrieving hash from database: " + filePath);
                            //sync by path
                            string mediaHash = Configuration.DB.MediaSubset.GetMediaHashByPath(filePath);
                            string deviceName = Configuration.DB.MediaSubset.GetMediaDeviceNameByPath(filePath);
                            mli = new MediaListItem(filePath, deviceName, mediaHash);
                        }

                        fileNamesToMedia[mediaFileName] = mli.Media;
                    }

                    db.SyncAsync(fileNamesToMedia.Values, this);

                    foreach (var fileName in fileNamesToMedia.Keys)
                    {
                        var media = fileNamesToMedia[fileName];

                        //foreach (MediaTagging tag in media.MediaTaggings)
                        //{
                        //    //create tag element and append to 
                        //    XElement tagEl = Xml.Xml.CreateTagElement(tag);
                        //    mediaEl.Add(tagEl);
                        //}

                        //tags to string
                        List<string> tagList =
                            media.MediaTaggings.Select(m => m.MediaTagValue)
                                               .Distinct()
                                               .ToList();

                        string tagString = string.Join(", ", tagList);
                        
                        string singleLineFileNameAndTags = fileName + "\t" + tagString;

                        singleLineFileNamesAndTags.Add(singleLineFileNameAndTags);
                    }

                }
                catch (Exception ex)
                {
                    var debuggingBreakpoint = ex.Message;
                }
            });

            string namesAndTagsAsMultiLineString = string.Join(Environment.NewLine, singleLineFileNamesAndTags);

            Clipboard.SetText(namesAndTagsAsMultiLineString);

            UI.Display.Message("copied to clipboard: " +
                Environment.NewLine + namesAndTagsAsMultiLineString);
        } 

        #endregion

    }
}
