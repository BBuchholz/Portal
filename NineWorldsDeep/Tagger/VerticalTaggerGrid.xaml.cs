﻿using NineWorldsDeep.Core;
using NineWorldsDeep.Model;
using NineWorldsDeep.Tagger.V2;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NineWorldsDeep.Tagger
{
    /// <summary>
    /// Interaction logic for VerticalTaggerGrid.xaml
    /// </summary>
    public partial class VerticalTaggerGrid : UserControl, ITaggerGrid
    {
        private TaggerGridController mController;
        private Image imageControl;

        public VerticalTaggerGrid()
        {
            InitializeComponent();
            //dbCore = new Db.Sqlite.DbAdapterSwitch();
            tagFile = taggerConfigFolderPath + "\\fileTags.xml";
            mController = new TaggerGridController(
                txtTags, 
                txtFilter, 
                lvTags, 
                lvFileElements, 
                tbFileCount, 
                tbStatus,
                chkMultiTagsAsIntersectionInsteadOfUnion);
        }
        
        //private TagMatrix tagMatrix = new TagMatrix();
        //private string lastLoadedPath;
        //private List<FileElementActionSubscriber> selectionChangedListeners =
        //    new List<FileElementActionSubscriber>();

        public string GetFolderWithLeastNonZeroUntaggedCount(string folderPath,
                                                             FilePathFilter fpf,
                                                             bool fileInsteadOfDb)
        {
            //we are going to be going through multiple folders, 
            //we don't want to load all of them to our main matrix
            TagMatrix temp = new TagMatrix();
            temp.AddFolder(folderPath);

            if (fileInsteadOfDb)
            {
                temp.LoadFromXml(Configuration.GetTagFilePath(folderPath));
            }
            else
            {
                temp.LoadFromDb(folderPath);
            }

            string leastPath = folderPath;
            int leastCount =
                temp.GetFilesForTag(TagMatrix.TAG_UNTAGGED, fpf).Count();

            foreach (string path in Directory.GetDirectories(folderPath))
            {
                temp.Clear();
                temp.AddFolder(path);
                if (fileInsteadOfDb)
                {
                    temp.LoadFromXml(Configuration.GetTagFilePath(path));
                }
                else
                {
                    temp.LoadFromDb(path);
                }
                int count =
                    temp.GetFilesForTag(TagMatrix.TAG_UNTAGGED, fpf).Count();

                if (count > 0)
                {
                    if (leastCount < 1 || count < leastCount)
                    {
                        leastCount = count;
                        leastPath = path;
                    }
                }
            }

            return leastPath;
        }

        public void SetImageControl(Image imageControl)
        {
            this.imageControl = imageControl;
        }

        //private NwdDb db = null;
        //private Db.Sqlite.DbAdapterSwitch dbCore = null;

        //public void RegisterDb(NwdDb nwdDb)
        //{
        //    //this.db = nwdDb;
        //    mController.RegisterDb(nwdDb);
        //}

        public void SetStatusForegroundColor(Brush b)
        {
            //tbStatus.Foreground = b;
            mController.SetStatusForegroundColor(b);
        }

        //TODO: this should be moved to Configuration
        private string taggerConfigFolderPath = "C:\\NWD\\config\\tagger";

        public List<string> GetTagsForCurrentSelection()
        {
            //return TagString.Parse(txtTags.Text);
            return mController.GetTagsForCurrentSelection();
        }

        public FileElementActionSubscriber DoubleClickListener { get; set; }

        public void AddSelectionChangedListener(FileElementActionSubscriber feas)
        {
            //selectionChangedListeners.Add(feas);
            mController.AddSelectionChangedListener(feas);
        }

        private string tagFile;

        public void Clear()
        {
            //tagMatrix.Clear();
            //lvFileElements.ItemsSource = null;
            //lvTags.ItemsSource = null;
            mController.Clear();
        }

        public void AddFolder(string folderPath)
        {
            //tagMatrix.AddFolder(folderPath);

            //LoadFileElementList(tagMatrix.GetFilePaths());
            mController.AddFolder(folderPath);
        }

        public void Add(List<FileElement> lst)
        {
            //tagMatrix.Add(lst);

            //LoadFileElementList(tagMatrix.GetFilePaths());
            mController.Add(lst);
        }

        public IEnumerable<FileElement> FileElements
        {
            get
            {
                //return lvFileElements.Items.Cast<FileElement>().ToList();
                return mController.FileElements;
            }
        }

        private bool pendingChanges = false;

        public bool HasPendingChanges { get { return pendingChanges; } }

        public FileElement SelectedFileElement
        {
            get
            {
                //return (FileElement)lvFileElements.SelectedItem;
                return mController.SelectedFileElement;
            }
        }

        //public void LoadFileElementsFromDb()
        //{
        //    //if (db != null)
        //    //{
        //    //    Add(db.GetFileElementsFromDb());
        //    //}
        //    mController.LoadFileElementsFromDb();
        //}

        //public void EnsureFileElementsInDb()
        //{
        //    //if (db != null)
        //    //{
        //    //    List<FileElement> inputList = ToFileElementList(tagMatrix.GetFilePaths());
        //    //    List<FileElement> dbList = db.GetFileElementsFromDb();
        //    //    List<FileElement> toBeAdded = SyncTools.CalculateElementsToBeAdded(inputList, dbList);
        //    //    db.AddFileElementsToDb(toBeAdded);
        //    //    tbStatus.Text = toBeAdded.Count + " FileElement(s) added.";
        //    //}
        //    mController.EnsureFileElementsInDb();
        //}

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var fe = ((ListViewItem)sender).Content as FileElement;
            if (DoubleClickListener != null)
                DoubleClickListener.PerformAction(fe);
        }

        private void lvTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mController.LoadFromSelectedTag();
        }

        //private void LoadFromSelectedTag()
        //{            
        //    //TagModelItem tmi = (TagModelItem)lvTags.SelectedItem;

        //    //if (tmi != null)
        //    //{
        //    //    //LoadFileElementList(tagMatrix.GetFilesForTag(tmi.Tag));
        //    //    LoadFileElementList(from fmi in tmi.Files
        //    //                        select fmi.GetPath());
        //    //}
        //    mController.Load
        //}

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
            //if (!string.IsNullOrWhiteSpace(txtTags.Text))
            //{
            //    tag = ", " + tag;
            //}

            //txtTags.Text = txtTags.Text + tag;

            mController.AppendTagToCurrentTagString(tag);
        }

        public void AppendTagsToCurrentTagStringAndUpdate(List<string> tags)
        {
            foreach (string tag in tags)
            {
                AppendTagToCurrentTagString(tag);
            }

            Update();
        }

        public void Update()
        {
            //tbStatus.Text = "Updating...";

            //FileElement fe = (FileElement)lvFileElements.SelectedItem;

            //if (fe != null)
            //{
            //    tagMatrix.UpdateTagString(fe, txtTags.Text);

            //    PopulateTagListView();

            //    tbStatus.Text = "Updated.";
            //    SetPendingChanges(true);
            //}

            mController.Update();
        }

        //private void SetPendingChanges(bool b)
        //{
        //    pendingChanges = b;
        //    if (!pendingChanges)
        //    {
        //        tbStatus.Text = "";
        //    }
        //    else
        //    {
        //        tbStatus.Text = "Pending Changes";
        //    }
            
        //}

        private void lvFileElements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //FileElement fe = (FileElement)lvFileElements.SelectedItem;

            //if (fe != null)
            //{
            //    if (string.IsNullOrWhiteSpace(fe.TagString))
            //    {
            //        fe.TagString = tagMatrix.GetTagString(fe.Path);
            //    }

            //    txtTags.Text = fe.TagString;

            //    foreach (FileElementActionSubscriber feas in selectionChangedListeners)
            //    {
            //        feas.PerformAction(fe);
            //    }
            //}

            //tbStatus.Text = "";

            mController.ProcessFileElementSelectionChanged();
        }

        //private List<FileElement> ToFileElementList(IEnumerable<string> pathList)
        //{
        //    List<FileElement> feLst = new List<FileElement>();

        //    foreach (string file in pathList)
        //    {
        //        //feLst.Add(new FileElement()
        //        //{
        //        //    Name = System.IO.Path.GetFileName(file),
        //        //    Path = file
        //        //});

        //        feLst.Add(FileElement.FromPath(file, tagMatrix));
        //    }

        //    feLst.Sort((fe1, fe2) => fe1.Name.CompareTo(fe2.Name));

        //    return feLst;
        //}

        //private void LoadFileElementList(IEnumerable<string> pathList)
        //{
        //    lvFileElements.ItemsSource = ToFileElementList(pathList);
        //    tbFileCount.Text = "Count: " + pathList.Count();
        //    if(pathList.Count() > 0)
        //    {
        //        lvFileElements.SelectedIndex = 0;
        //    }
        //}

        //private void PopulateTagListView()
        //{
        //    lvTags.ItemsSource = tagMatrix.GetTagModelItems(txtFilter.Text);                       
        //}

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

        public void LoadFromFile(string loadFilePath)
        {
            //tbStatus.Text = "Loading...";

            //tagMatrix.LoadFromXml(loadFilePath);

            //PopulateTagListView();

            //tbStatus.Text = "Loaded.";
            //SetPendingChanges(false);

            mController.LoadFromFile(loadFilePath);
        }

        public void LoadFromDb(string filePathTopFolder)
        {
            //tbStatus.Text = "Loading...";

            //tagMatrix.LoadFromDb(filePathTopFolder);

            //PopulateTagListView();

            //tbStatus.Text = "Loaded.";
            //lastLoadedPath = filePathTopFolder;
            //SetPendingChanges(false);

            mController.LoadFromDb(filePathTopFolder);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadFromFileWithPrompt();
        }

        public void SaveToDb()
        {
            //tbStatus.Text = "Saving...";

            //tagMatrix.SaveToDb();

            //tbStatus.Text = "Saved.";
            //SetPendingChanges(false);

            mController.SaveToDb();
        }

        public void SaveToFile(string saveFilePath)
        {
            //tbStatus.Text = "Saving...";

            //tagMatrix.SaveToXml(saveFilePath);

            //tbStatus.Text = "Saved.";
            //SetPendingChanges(false);

            mController.SaveToFile(saveFilePath);
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
            //tagMatrix.SetFolderLoadStrategy(fls);

            mController.SetFolderLoadStrategy(fls);
        }

        public TagMatrix GetTagMatrix()
        {
            //return tagMatrix;

            return mController.GetTagMatrix();
        }

        private void txtFilter_KeyUp(object sender, KeyEventArgs e)
        {
            mController.PopulateTagListView();
        }

        private void MenuItemSendToTrash_Click(object sender, RoutedEventArgs e)
        {
            ////delete
            //FileElement fe = (FileElement)lvFileElements.SelectedItem;

            //string msg = "Are you sure you want to move this file to trash? " +
            //    "Be aware that these tags will be permanently lost even if " +
            //    "file is restored from trash: ";

            //if (fe != null && UI.Prompt.Confirm(msg + fe.TagString, true))
            //{
            //    //imageControl.Source = null;

            //    fe.MoveToTrash(dbCore);

            //    //remove path from tag matrix
            //    tagMatrix.RemovePath(fe.Path);

            //    imageControl.Source = null;

            //    //refresh list
            //    Reload(); 
            //}

            if (mController.SendSelectedFileElementToTrash())
            {
                imageControl.Source = null;
            }
        }

        public bool SendSelectedFileElementsToTrash()
        {
            if (mController.SendSelectedFileElementsToTrash())
            {
                imageControl.Source = null;
                return true;
            }

            return false;
        }

        //private void Reload()
        //{
        //    if (lastLoadedPath != null)
        //    {
        //        string filter = txtFilter.Text;
        //        TagModelItem selectedTag = (TagModelItem)lvTags.SelectedItem;

        //        LoadFromDb(lastLoadedPath);

        //        txtFilter.Text = filter;

        //        //find selected tag
        //        TagModelItem selectedItem = null;
        //        foreach (TagModelItem item in lvTags.Items)
        //        {
        //            if (item.Tag.Equals(selectedTag.Tag, StringComparison.CurrentCultureIgnoreCase))
        //            {
        //                selectedItem = item;
        //            }
        //        }

        //        if (selectedItem != null)
        //        {
        //            lvTags.SelectedItem = selectedItem;
        //        }
        //    }
        //}

        private void MenuItemCopyConsumptionTag_Click(object sender, RoutedEventArgs e)
        {
            //FileElement fe = (FileElement)lvFileElements.SelectedItem;

            //if (fe != null)
            //{
            //    string fileName = System.IO.Path.GetFileName(fe.Path);
            //    string tag = "[consumes " + fileName + "]";
            //    Clipboard.SetText(tag);
            //    MessageBox.Show(tag + " copied to clipboard");
            //}

            mController.CopySelectedFileElementConsumptionTagToClipboard();
        }
    }
}
