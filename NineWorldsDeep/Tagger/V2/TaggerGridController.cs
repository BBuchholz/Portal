using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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

        private bool pendingChanges = false;
        private string lastLoadedPath;
        private TagMatrix tagMatrix = new TagMatrix();
        private List<FileElementActionSubscriber> selectionChangedListeners =
            new List<FileElementActionSubscriber>();

        public TaggerGridController(
            TextBox tagString,
            TextBox tagFilter,
            ListView tags, 
            ListView fileElements, 
            TextBlock fileCount, 
            TextBlock status)
        {
            txtTags = tagString;
            txtFilter = tagFilter;
            lvTags = tags;
            lvFileElements = fileElements;
            tbFileCount = fileCount;
            tbStatus = status;
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

            feLst.Sort((fe1, fe2) => fe1.Name.CompareTo(fe2.Name));

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

        private void PopulateTagListView()
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
