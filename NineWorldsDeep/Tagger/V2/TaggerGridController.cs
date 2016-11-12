using NineWorldsDeep.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NineWorldsDeep.Tagger.V2
{
    public class TaggerGridController : ITaggerGrid
    {
        private ListView lvFileElements;
        private ListView lvTags;
        private TextBox txtTags;
        private TextBox txtFilter;
        private TextBlock tbFileCount;
        private TextBlock tbStatus;
        private CheckBox chkMultiTagsAsIntersectionInsteadOfUnion;

        private bool pendingChanges = false;
        private string lastLoadedPath;

        private NwdDb db = null;
        private Db.Sqlite.DbAdapterSwitch dbCore = null;
        private TagMatrix tagMatrix = new TagMatrix();
        private List<FileElementActionSubscriber> selectionChangedListeners =
            new List<FileElementActionSubscriber>();

        public TaggerGridController(
            TextBox tagString,
            TextBox tagFilter,
            ListView tags, 
            ListView fileElements, 
            TextBlock fileCount, 
            TextBlock status,
            CheckBox multiTagsAsIntersectionInsteadOfUnion)
        {
            txtTags = tagString;
            txtFilter = tagFilter;
            lvTags = tags;
            lvFileElements = fileElements;
            tbFileCount = fileCount;
            tbStatus = status;
            chkMultiTagsAsIntersectionInsteadOfUnion =
                multiTagsAsIntersectionInsteadOfUnion;
            dbCore = new Db.Sqlite.DbAdapterSwitch();

            chkMultiTagsAsIntersectionInsteadOfUnion.Unchecked +=
                ChkMultiTagsAsIntersectionInsteadOfUnion_Toggled;
            chkMultiTagsAsIntersectionInsteadOfUnion.Checked +=
                ChkMultiTagsAsIntersectionInsteadOfUnion_Toggled;
        }

        private void ChkMultiTagsAsIntersectionInsteadOfUnion_Toggled(object sender, RoutedEventArgs e)
        {
            string msg;
            
            if (!chkMultiTagsAsIntersectionInsteadOfUnion.IsChecked.Value)
            {
                msg = "When unchecked, this should populate the file element " +
                    "list with all file elements matching at least " +
                    "one tag filter in the comma separated tag filter list";
            }
            else
            {
                msg = "When checked, this should populate the file element list with " +
                    "only those file elements matching all tag filters in the " +
                    "comma separated tag filter list";
            }

            UI.Display.Message(msg);
        }

        public void LoadFromSelectedTag()
        {
            TagModelItem tmi = (TagModelItem)lvTags.SelectedItem;

            if (tmi != null)
            {
                LoadFileElementList(from fmi in tmi.Files
                                    select fmi.GetPath());
            }
        }
        
        public void RegisterDb(NwdDb nwdDb)
        {
            this.db = nwdDb;
        }

        public void LoadFileElementsFromDb()
        {
            if (db != null)
            {
                Add(db.GetFileElementsFromDb());
            }
        }

        public void EnsureFileElementsInDb()
        {
            if (db != null)
            {
                List<FileElement> inputList = ToFileElementList(tagMatrix.GetFilePaths());
                List<FileElement> dbList = db.GetFileElementsFromDb();
                List<FileElement> toBeAdded = SyncTools.CalculateElementsToBeAdded(inputList, dbList);
                db.AddFileElementsToDb(toBeAdded);
                tbStatus.Text = toBeAdded.Count + " FileElement(s) added.";
            }
        }

        public void Add(List<FileElement> lst)
        {
            tagMatrix.Add(lst);

            LoadFileElementList(tagMatrix.GetFilePaths());
        }

        public void SetStatusForegroundColor(Brush b)
        {
            tbStatus.Foreground = b;
        }

        public IEnumerable<FileElement> FileElements
        {
            get
            {
                return lvFileElements.Items.Cast<FileElement>().ToList();
            }
        }

        public bool HasPendingChanges
        {
            get
            {
                return pendingChanges;
            }
        }

        public FileElement SelectedFileElement
        {
            get
            {
                return (FileElement)lvFileElements.SelectedItem;
            }
        }

        public void AddFolder(string folderPath)
        {
            tagMatrix.AddFolder(folderPath);

            LoadFileElementList(tagMatrix.GetFilePaths());
        }

        private void LoadFileElementList(IEnumerable<string> pathList)
        {
            lvFileElements.ItemsSource = ToFileElementList(pathList);
            tbFileCount.Text = "Count: " + pathList.Count();
            if (pathList.Count() > 0)
            {
                lvFileElements.SelectedIndex = 0;
            }
        }

        private List<FileElement> ToFileElementList(IEnumerable<string> pathList)
        {
            List<FileElement> feLst = new List<FileElement>();

            foreach (string file in pathList)
            {
                feLst.Add(FileElement.FromPath(file, tagMatrix));
            }

            feLst = feLst.OrderByDescending(f => f.ModifiedAt).ToList();

            return feLst;
        }

        public void AddSelectionChangedListener(FileElementActionSubscriber feas)
        {
            selectionChangedListeners.Add(feas);
        }

        public void AppendTagsToCurrentTagStringAndUpdate(List<string> tags)
        {
            foreach (string tag in tags)
            {
                AppendTagToCurrentTagString(tag);
            }

            Update();
        }

        public void AppendTagToCurrentTagString(string tag)
        {
            if (!string.IsNullOrWhiteSpace(txtTags.Text))
            {
                tag = ", " + tag;
            }

            txtTags.Text = txtTags.Text + tag;
        }

        public bool SendSelectedFileElementToTrash()
        {
            //delete
            FileElement fe = (FileElement)lvFileElements.SelectedItem;

            string msg = "Are you sure you want to move this file to trash? " +
                "Be aware that these tags will be permanently lost even if " +
                "file is restored from trash: ";

            if (fe != null && UI.Prompt.Confirm(msg + fe.TagString, true))
            {
                fe.MoveToTrash(dbCore);

                //remove path from tag matrix
                tagMatrix.RemovePath(fe.Path);
                
                //refresh list
                Reload();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SendSelectedFileElementsToTrash()
        {
            //delete
            //List<FileElement> fe = (List<FileElement>)lvFileElements.SelectedItems;

            List<FileElement> feLst = lvFileElements.SelectedItems.Cast<FileElement>()
                                                    .Select(fe => fe)
                                                    .ToList();

            string msg = "Are you sure you want to move these " + feLst.Count + " files to trash? " +
                "Be aware that these tags will be permanently lost even if " +
                "files are restored from trash";

            if (feLst != null && UI.Prompt.Confirm(msg, true))
            {
                foreach (FileElement fe in feLst)
                {
                    fe.MoveToTrash(dbCore);

                    //remove path from tag matrix
                    tagMatrix.RemovePath(fe.Path);

                    //refresh list
                    Reload();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void OpenSelectedFileElementExternally()
        {
            FileElement fe = (FileElement)lvFileElements.SelectedItem;

            if (fe != null)
            {
                //open externally
                Process proc = new Process();
                proc.StartInfo.FileName = fe.Path;
                proc.Start();
            }
        }

        private void Reload()
        {
            if (lastLoadedPath != null)
            {
                string filter = txtFilter.Text;
                TagModelItem selectedTag = (TagModelItem)lvTags.SelectedItem;

                LoadFromDb(lastLoadedPath);

                txtFilter.Text = filter;

                //find selected tag
                TagModelItem selectedItem = null;
                foreach (TagModelItem item in lvTags.Items)
                {
                    if (item.Tag.Equals(selectedTag.Tag, StringComparison.CurrentCultureIgnoreCase))
                    {
                        selectedItem = item;
                    }
                }

                if (selectedItem != null)
                {
                    lvTags.SelectedItem = selectedItem;
                }
            }
        }

        public void CopySelectedFileElementConsumptionTagToClipboard()
        {
            FileElement fe = (FileElement)lvFileElements.SelectedItem;

            if (fe != null)
            {
                string fileName = System.IO.Path.GetFileName(fe.Path);
                string tag = "[consumes " + fileName + "]";
                Clipboard.SetText(tag);
                MessageBox.Show(tag + " copied to clipboard");
            }
        }

        public void ProcessFileElementSelectionChanged()
        {
            FileElement fe = (FileElement)lvFileElements.SelectedItem;

            if (fe != null)
            {
                if (string.IsNullOrWhiteSpace(fe.TagString))
                {
                    fe.TagString = tagMatrix.GetTagString(fe.Path);
                }

                txtTags.Text = fe.TagString;

                foreach (FileElementActionSubscriber feas in selectionChangedListeners)
                {
                    feas.PerformAction(fe);
                }
            }

            tbStatus.Text = "";
        }

        //public void PopulateTagListView()
        //{
        //    lvTags.ItemsSource = tagMatrix.GetTagModelItems(txtFilter.Text);
        //}

        public void Update()
        {
            tbStatus.Text = "Updating...";

            FileElement fe = (FileElement)lvFileElements.SelectedItem;

            if (fe != null)
            {
                tagMatrix.UpdateTagString(fe, txtTags.Text);

                PopulateTagListView();

                tbStatus.Text = "Updated.";
                SetPendingChanges(true);
            }
        }

        private void SetPendingChanges(bool changesPending)
        {
            pendingChanges = changesPending;
            if (!pendingChanges)
            {
                tbStatus.Text = "";
            }
            else
            {
                tbStatus.Text = "Pending Changes";
            }
        }

        public void PopulateTagListView()
        {
            lvTags.ItemsSource = tagMatrix.GetTagModelItems(txtFilter.Text);
        }
        
        public void AppendTagToCurrentTagStringAndUpdate(string tag)
        {
            AppendTagToCurrentTagString(tag);

            Update();
        }

        public void Clear()
        {
            tagMatrix.Clear();
            lvFileElements.ItemsSource = null;
            lvTags.ItemsSource = null;
        }

        public TagMatrix GetTagMatrix()
        {
            return tagMatrix;
        }

        public List<string> GetTagsForCurrentSelection()
        {
            return TagString.Parse(txtTags.Text);
        }

        public void LoadFromDb(string filePathTopFolder)
        {
            tbStatus.Text = "Loading...";

            tagMatrix.LoadFromDb(filePathTopFolder);

            PopulateTagListView();

            tbStatus.Text = "Loaded.";
            lastLoadedPath = filePathTopFolder;
            SetPendingChanges(false);
        }

        public void LoadFromFile(string loadFilePath)
        {
            tbStatus.Text = "Loading...";

            tagMatrix.LoadFromXml(loadFilePath);

            PopulateTagListView();

            tbStatus.Text = "Loaded.";
            SetPendingChanges(false);
        }

        public void SaveToDb()
        {
            tbStatus.Text = "Saving...";

            tagMatrix.SaveToDb();

            tbStatus.Text = "Saved.";
            SetPendingChanges(false);
        }

        public void SaveToFile(string saveFilePath)
        {
            tbStatus.Text = "Saving...";

            tagMatrix.SaveToXml(saveFilePath);

            tbStatus.Text = "Saved.";
            SetPendingChanges(false);
        }

        public void SetFolderLoadStrategy(IFolderLoadStrategy fls)
        {
            tagMatrix.SetFolderLoadStrategy(fls);
        }

    }
}
