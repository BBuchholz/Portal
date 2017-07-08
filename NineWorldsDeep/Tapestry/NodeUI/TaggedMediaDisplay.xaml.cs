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

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for TaggedMediaDisplay.xaml
    /// </summary>
    public partial class TaggedMediaDisplay : UserControl, IAsyncStatusResponsive
    {
        private TaggingMatrix taggingMatrix;
        Db.Sqlite.MediaV5SubsetDb db;

        //async related 
        private readonly SynchronizationContext syncContext;
        private DateTime previousTime = DateTime.Now;

        public TaggedMediaDisplay()
        {
            InitializeComponent();
            syncContext = SynchronizationContext.Current;

            db = new Db.Sqlite.MediaV5SubsetDb();
            RefreshTaggingMatrix();
        }
        
        private void lvPaths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //adapted from MediaMasterDisplay
            List<string> selectedPaths = lvPaths.SelectedItems.Cast<string>().ToList();

            if (selectedPaths.Count > 0)
            {
                //display first selected
                string firstPath = selectedPaths[0];

                if (File.Exists(firstPath) && db.LocalDeviceId > 0)
                {
                    PathSelectedEventArgs args =
                        new PathSelectedEventArgs(
                            new FileSystemNode(firstPath, true, db.LocalDeviceId));

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

        private void lvTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //from VerticalTaggerGrid (need to refactor to use V5, &c.)
            //TaggerGridController.LoadFromSelectedTag();
            
            LoadPaths();
        }

        private void txtTagFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                LoadTags();
            }
        }

        private void LoadTags()
        {
            lvTags.ItemsSource = FilterTaggingMatrix(taggingMatrix, txtTagFilter.Text);
        }

        private void LoadPaths()
        {
            lvPaths.ItemsSource = FilterPaths(GetPathsForSelectedTag(), txtPathFilter.Text);
        }

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
            //TODO: implement
            return paths;
        }

        private void RefreshTaggingMatrix()
        {
            //TODO: if this takes long, make async and update status per section
            TaggingMatrix tm = db.RetrieveLocalDeviceTaggingMatrix();

            tm.AddFolderAndAllSubfolders(Configuration.ImagesFolder);
            tm.AddFolderAndAllSubfolders(Configuration.VoiceMemosFolder);

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
    }
}
