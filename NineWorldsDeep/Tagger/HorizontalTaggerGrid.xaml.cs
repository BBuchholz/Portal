using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NineWorldsDeep.Tagger
{
    /// <summary>
    /// Interaction logic for HorizonalTaggerGrid.xaml
    /// </summary>
    public partial class HorizontalTaggerGrid : UserControl, ITaggerGrid
    {
        private TagMatrix tagMatrix = new TagMatrix();
        private List<FileElementActionSubscriber> selectionChangedListeners =
            new List<FileElementActionSubscriber>();
        private NwdDb db = null;
        private Db.SqliteDbAdapter dbCore;

        public HorizontalTaggerGrid()
        {
            InitializeComponent();
            dbCore = new Db.SqliteDbAdapter();
            tagFile = taggerConfigFolderPath + "\\fileTags.xml";
            AddSelectionChangedListener(new FileElementTimestampExtractionAction(tagMatrix, this));
            //moved to NwdVoiceMemoBrowser
            //AddSelectionChangedListener(new FileElementTagExtractionAction(tagMatrix, this));
        }

        public Button StopAudioButton
        {
            get { return btnStopAudio; }
        }

        public void RegisterDb(NwdDb nwdDb)
        {
            this.db = nwdDb;
        }

        public void SetStatusForegroundColor(Brush b)
        {
            tbPendingChanges.Foreground = b;
            tbStatus.Foreground = b;
        }

        private string taggerConfigFolderPath = "C:\\NWD\\config\\tagger";

        public List<string> GetTagsForCurrentSelection()
        {
            return TagString.Parse(txtTags.Text);
        }

        public FileElementActionSubscriber DoubleClickListener { get; set; }

        public void AddSelectionChangedListener(FileElementActionSubscriber feas)
        {
            selectionChangedListeners.Add(feas);
        }

        private string tagFile;

        public void Clear()
        {
            tagMatrix.Clear();
            lvFileElements.ItemsSource = null;
            lvTags.ItemsSource = null;
        }
        
        public void AddFolder(string folderPath)
        {
            tagMatrix.AddFolder(folderPath);

            LoadFileElementList(tagMatrix.GetFilePaths());
        }

        public void Add(List<FileElement> lst)
        {
            tagMatrix.Add(lst);

            LoadFileElementList(tagMatrix.GetFilePaths());
        }

        private bool pendingChanges = false;

        public bool HasPendingChanges { get { return pendingChanges; } }

        public FileElement SelectedFileElement
        {
            get
            {
                return (FileElement)lvFileElements.SelectedItem;
            }
        }

        public IEnumerable<FileElement> FileElements
        {
            get
            {
                return lvFileElements.Items.Cast<FileElement>().ToList();
            }
        }

        public void LoadFileElementsFromDb()
        {
            if(db != null)
            {
                Add(db.GetFileElementsFromDb());
            }
        }

        public void EnsureFileElementsInDb()
        {
            if(db != null)
            {
                List<FileElement> inputList = ToFileElementList(tagMatrix.GetFilePaths());
                List<FileElement> dbList = db.GetFileElementsFromDb();
                List<FileElement> toBeAdded = SyncTools.CalculateElementsToBeAdded(inputList, dbList);
                db.AddFileElementsToDb(toBeAdded);
                tbStatus.Text = toBeAdded.Count + " FileElement(s) added.";
            }
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var fe = ((ListViewItem)sender).Content as FileElement;
            if (DoubleClickListener != null)
                DoubleClickListener.PerformAction(fe); 
        }

        private void lvTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string tag = (string)lvTags.SelectedItem;

            //LoadFileElementList(tagMatrix.GetFilesForTag(tag));

            LoadFromSelectedTag();
        }

        private void LoadFromSelectedTag()
        {
            string tag = (string)lvTags.SelectedItem;

            LoadFileElementList(tagMatrix.GetFilesForTag(tag));
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Update();
        }

        public void AppendTagToCurrentTagStringAndUpdate(string tag)
        {
            AppendTagToCurrentTagString(tag);

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

        public void AppendTagsToCurrentTagStringAndUpdate(List<string> tags)
        {
            foreach(string tag in tags)
            {
                AppendTagToCurrentTagString(tag);
            }

            Update();
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

        private void SetPendingChanges(bool b)
        {
            pendingChanges = b;
            if (!pendingChanges)
            {
                tbPendingChanges.Text = "";
            }
            else
            {
                tbPendingChanges.Text = "Pending Changes";
            }
        }

        private void lvFileElements_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        
        private List<FileElement> ToFileElementList(IEnumerable<string> pathList)
        {
            List<FileElement> feLst = new List<FileElement>();
            
            foreach (string file in pathList)
            {
                //feLst.Add(new FileElement()
                //{
                //    Name = System.IO.Path.GetFileName(file),
                //    Path = file
                //});

                feLst.Add(FileElement.FromPath(file, tagMatrix));
            }

            return feLst;
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

        private void PopulateTagListView()
        {            
            lvTags.ItemsSource = tagMatrix.GetTags(txtFilter.Text);
        }

        public void LoadFromFileWithPrompt()
        {            
            var dlg = new System.Windows.Forms.OpenFileDialog();

            dlg.InitialDirectory = taggerConfigFolderPath;
            dlg.Filter = "xml files (*.xml)|*.xml";
            dlg.FilterIndex = 1;

            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                LoadFromFile(dlg.FileName);
            }
        }

        public void SaveToFileWithPrompt()
        {
            string tagFileNow = "tagFile-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";

            var dlg = new System.Windows.Forms.SaveFileDialog();

            dlg.InitialDirectory = taggerConfigFolderPath;
            dlg.Filter = "xml files (*.xml)|*.xml";
            dlg.FilterIndex = 1;
            dlg.FileName = tagFileNow;

            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SaveToFile(dlg.FileName);
            }
        }

        public void LoadFromDb(string filePathTopFolder)
        {
            tbStatus.Text = "Loading...";

            tagMatrix.LoadFromDb(filePathTopFolder);

            PopulateTagListView();

            tbStatus.Text = "Loaded.";
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

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadFromFileWithPrompt();
        }

        public void SaveToFile(string saveFilePath)
        {
            tbStatus.Text = "Saving...";

            tagMatrix.SaveToXml(saveFilePath);

            tbStatus.Text = "Saved.";
            SetPendingChanges(false);
        }

        public void SaveToDb()
        {
            tbStatus.Text = "Saving...";

            tagMatrix.SaveToDb();

            tbStatus.Text = "Saved.";
            SetPendingChanges(false);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveToFileWithPrompt();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        public void SetFolderLoadStrategy(IFolderLoadStrategy fls)
        {
            tagMatrix.SetFolderLoadStrategy(fls);
        }

        public TagMatrix GetTagMatrix()
        {
            return tagMatrix;
        }

        private void txtFilter_KeyUp(object sender, KeyEventArgs e)
        {
            PopulateTagListView();
        }

        private void MenuItemOpenExternally_Click(object sender, RoutedEventArgs e)
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

        private void MenuItemSendToTrash_Click(object sender, RoutedEventArgs e)
        {
            //delete
            FileElement fe = (FileElement)lvFileElements.SelectedItem;

            string msg = "Are you sure you want to move this file to trash? " +
                "Be aware that these tags will be permanently lost even if " +
                "file is restored from trash: ";

            if (fe != null && UI.Prompt.Confirm(msg + fe.TagString, true))
            {
                StopAudioButton.RaiseEvent(
                    new RoutedEventArgs(ButtonBase.ClickEvent));

                fe.MoveToTrash(dbCore);

                //remove path from tag matrix
                tagMatrix.RemovePath(fe.Path);

                //refresh list
                LoadFromSelectedTag();
            }
        }
    }
}
