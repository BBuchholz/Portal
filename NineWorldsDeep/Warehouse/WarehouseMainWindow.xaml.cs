using NineWorldsDeep.Core;
using NineWorldsDeep.Mnemosyne.V5;
using NineWorldsDeep.Sqlite.Model;
using NineWorldsDeep.Tapestry.NodeUI;
using NineWorldsDeep.UI;
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
using System.Windows.Shapes;

namespace NineWorldsDeep.Warehouse
{
    /// <summary>
    /// Interaction logic for WarehouseMainWindow.xaml
    /// </summary>
    public partial class WarehouseMainWindow : Window, IAsyncStatusResponsive
    {
        private SyncItemCollection _syncItemCol;
        private Db.Sqlite.DbAdapterSwitch db =
            new Db.Sqlite.DbAdapterSwitch();

        private bool syncDirectionProcessingInProgress = false;

        public WarehouseMainWindow()
        {
            InitializeComponent();
            
            cmbDirection.Items.Add(SyncDirection.Import);
            cmbDirection.Items.Add(SyncDirection.Export);

            cmbActionDefault.Items.Add(SyncAction.Ignore);
            cmbActionDefault.Items.Add(SyncAction.CopyAndStamp);
            cmbActionDefault.Items.Add(SyncAction.Copy);
            cmbActionDefault.Items.Add(SyncAction.MoveAndStamp);
            cmbActionDefault.Items.Add(SyncAction.Move);

            cmbActionDefault.SelectedItem = SyncAction.Ignore;

            try
            {
                RefreshProfiles();
            }
            catch (Exception ex)
            {
                Display.Exception(ex);
            }
        }

        /// <summary>
        /// retrieves XAML defined collection with lazy instantiation
        /// </summary>
        private SyncItemCollection SyncItems
        {
            get
            {
                if (_syncItemCol == null)
                {
                    _syncItemCol = (SyncItemCollection)
                        FindResource("colSyncItems");
                }

                return _syncItemCol;
            }
        }

        public SyncProfile CurrentProfile
        {
            get
            {
                return (SyncProfile)cmbSyncProfile.SelectedItem;
            }
        }

        private void LoadDefaultProfiles()
        {
            SyncProfile phone = new SyncProfile("phone");
            SyncProfile tablet = new SyncProfile("tablet");

            db.LoadSyncProfile(phone);
            db.LoadSyncProfile(tablet);

            cmbSyncProfile.Items.Add(phone);
            cmbSyncProfile.Items.Add(tablet);
        }

        private void RefreshProfiles()
        {
            cmbSyncProfile.Items.Clear();
            
            foreach (SyncProfile sp in db.GetAllSyncProfiles())
            {
                cmbSyncProfile.Items.Add(sp);
            }
        }

        private void CollectionsViewSource_Filter(object sender, FilterEventArgs e)
        {
            //not used currently
            //some reference for datagrid grouping and sorting
            //https://msdn.microsoft.com/en-us/library/ff407126(v=vs.100).aspx
        }

        private void MenuItemSyncMappingsWindow_Click(object sender, RoutedEventArgs e)
        {
            SyncProfile selected = (SyncProfile)cmbSyncProfile.SelectedItem;
            if (selected != null)
            {
                new SyncMapWindow(selected).Show();
            }
        }

        private async void ProcessSyncDirection()
        {
            if(!syncDirectionProcessingInProgress)
            {
                syncDirectionProcessingInProgress = true;

                SetExecutionStatus(ExecStatus.Ready);

                if (CurrentProfile != null &&
                    cmbDirection.SelectedItem != null)
                {
                    SyncDirection selected = (SyncDirection)cmbDirection.SelectedItem;

                    SyncItems.Clear();

                    switch (selected)
                    {
                        case SyncDirection.Import:

                            SyncProfile sp = CurrentProfile.ShallowCopy();

                            bool tagsFromXmlNotKeyValFile = chkTagsFromXmlNotKeyVal.IsChecked.Value;

                            var importList = await Task.Run(() => 
                                PopulateImports(this, sp, tagsFromXmlNotKeyValFile)
                            );

                            SyncItems.AddAll(importList);

                            //old, synchronous version
                            //PopulateImports(CurrentProfile);

                            break;

                        case SyncDirection.Export:

                            SyncProfile sp2 = CurrentProfile.ShallowCopy();

                            var exportList = await Task.Run(() => 
                                PopulateExports(this, sp2)
                            ); 

                            SyncItems.AddAll(exportList);

                            //old, synchronous version
                            //PopulateExports(CurrentProfile);

                            break;
                    }
                }
                
                syncDirectionProcessingInProgress = false;
            }
                        
            ProcessActionDefault();
        }

        private void ProcessActionDefault()
        {
            var sa = cmbActionDefault.SelectedItem;
            var objDirection = cmbDirection.SelectedItem;            

            if(sa == null)
            {
                return;
            }

            foreach (var si in SyncItems)
            {
                si.SyncAction = (SyncAction)sa;

                //by default, just copy all untagged imports
                if (cmbDirection.SelectedItem != null)
                {
                    var sd = (SyncDirection)cmbDirection.SelectedItem;

                    if (sd == SyncDirection.Import &&
                        string.IsNullOrWhiteSpace(si.ExtTags) &&
                        chkOverrideDefaultAction.IsChecked != true)
                    {   
                        //midi files always move, used for exports from tablet software
                        if(si.ExtPath.EndsWith(".mid", StringComparison.CurrentCultureIgnoreCase)){

                            si.SyncAction = SyncAction.Move;
                        }
                        else
                        {
                            si.SyncAction = SyncAction.Copy;
                        }
                    }
                }
            }
        }

        private void cmbDirection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessSyncDirection();
        }

        private string GetProfileName()
        {
            string profileName = "";

            if (cmbSyncProfile.SelectedItem != null)
            {
                SyncProfile sp = (SyncProfile)cmbSyncProfile.SelectedItem;
                profileName = sp.Name;
            }

            return profileName;
        }

        public void StatusDetailUpdate(string text)
        {
            Dispatcher.Invoke(() =>
            {
                tbStatus.Text = text;
            });
        }

        internal List<SyncItem> PopulateExports(
            WarehouseMainWindow gui, SyncProfile sp)
        {
            List<SyncItem> lst = new List<SyncItem>();

            string profileName = sp.Name;

            int grandTotal = 0;

            foreach (SyncMap sm in sp.SyncMaps)
            {
                if (sm.SyncDirection == SyncDirection.Export)
                {
                    string exportRootPath = sm.Source;

                    var allFilePaths = Directory.GetFiles(exportRootPath, "*.*", SearchOption.TopDirectoryOnly);
                    int count = 0;
                    int total = allFilePaths.Count();
                    grandTotal += total;

                    //get files
                    foreach (string filePath in allFilePaths)
                    {
                        count++;

                        string msg = "processing " + count + " of " + total + ": " + filePath;

                        gui.StatusDetailUpdate(msg);
                        
                        string hash = Hashes.Sha1ForFilePath(filePath);
                        string path = filePath;
                        string tags = TagsV4c.GetTagStringForHash(hash);
                        string displayName = DisplayNames.FromHash(sp, sm.SyncDirection, hash);

                        lst.Add(new SyncItem(sm)
                        {
                            HostHash = hash,
                            HostPath = path,
                            HostTags = tags,
                            HostDisplayName = displayName
                        });
                    }

                    gui.StatusDetailUpdate("finished processing " + grandTotal + " files.");
                }
            }


            return lst;
        }


        private void PopulateExports(SyncProfile sp)
        {
            SyncItems.Clear();

            string profileName = sp.Name;

            foreach (SyncMap sm in sp.SyncMaps)
            {
                if (sm.SyncDirection == SyncDirection.Export)
                {
                    string exportRootPath = sm.Source;

                    var allFilePaths = Directory.GetFiles(exportRootPath, "*.*", SearchOption.TopDirectoryOnly);


                    //get files
                    foreach (string filePath in allFilePaths)
                    {
                        string hash = Hashes.Sha1ForFilePath(filePath);
                        string path = filePath;
                        string tags = TagsV4c.GetTagStringForHash(hash);
                        string displayName = DisplayNames.FromHash(sp, sm.SyncDirection, hash);

                        SyncItems.Add(new SyncItem(sm)
                        {
                            HostHash = hash,
                            HostPath = path,
                            HostTags = tags,
                            HostDisplayName = displayName
                        });
                    }
                }
            }
        }
        
        internal List<SyncItem> PopulateImports(
            IAsyncStatusResponsive gui, 
            SyncProfile sp, 
            bool tagsFromXmlNotKeyValFile)
        {
            List<SyncItem> lst = new List<SyncItem>();

            string profileName = sp.Name;

            int grandTotal = 0;

            foreach (SyncMap sm in sp.SyncMaps)
            {
                if (sm.SyncDirection == SyncDirection.Import)
                {
                    string importRootPath = sm.Source;

                    var allFilePaths = Directory.GetFiles(importRootPath, "*.*", SearchOption.TopDirectoryOnly);
                    int count = 0;
                    int total = allFilePaths.Count();
                    grandTotal += total;

                    TagStringHashIndex tagStringHashIndex = null;

                    if (tagsFromXmlNotKeyValFile)
                    {
                        tagStringHashIndex = 
                            Tags.GetTagStringHashIndexFromXml(sp, gui);
                    }

                    //get files
                    foreach (string filePath in allFilePaths)
                    {
                        if (!System.IO.Path.GetFileName(filePath).StartsWith("."))
                        {
                            count++;

                            string msg = "processing " + count + " of " + total + ": " + filePath;

                            gui.StatusDetailUpdate(msg);

                            string hash = Hashes.Sha1ForFilePath(filePath);
                            string path = filePath;
                            string tags = "";

                            if (tagsFromXmlNotKeyValFile)
                            {
                                tags = tagStringHashIndex.GetTagStringForHash(hash);
                            }
                            else
                            {
                                tags = TagsV4c.ImportForHash(sp, hash, tagsFromXmlNotKeyValFile);
                            }

                            string displayName = DisplayNames.FromHash(sp, sm.SyncDirection, hash);

                            lst.Add(new SyncItem(sm)
                            {
                                ExtHash = hash,
                                ExtPath = path,
                                ExtTags = tags,
                                ExtDisplayName = displayName
                            });
                        }
                    }

                    gui.StatusDetailUpdate("finished processing " + grandTotal + " files.");
                }
            }

            return lst;
        }


        private void PopulateImports(SyncProfile sp)
        {
            SyncItems.Clear();

            string profileName = sp.Name;

            bool tagsFromXmlNotKeyValFile = chkTagsFromXmlNotKeyVal.IsChecked.Value;

            foreach (SyncMap sm in sp.SyncMaps)
            {
                if (sm.SyncDirection == SyncDirection.Import)
                {
                    string importRootPath = sm.Source;

                    //get files
                    foreach (string filePath in Directory.GetFiles(importRootPath, "*.*", SearchOption.TopDirectoryOnly))
                    {
                        string hash = Hashes.Sha1ForFilePath(filePath);
                        string path = filePath;
                        string tags = TagsV4c.ImportForHash(sp, hash, tagsFromXmlNotKeyValFile);
                        string displayName = DisplayNames.FromHash(sp, sm.SyncDirection, hash);

                        SyncItems.Add(new SyncItem(sm)
                        {
                            ExtHash = hash,
                            ExtPath = path,
                            ExtTags = tags,
                            ExtDisplayName = displayName
                        });
                    }
                }
            }
        }

        private void cmbSyncProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SyncProfile selected = (SyncProfile)cmbSyncProfile.SelectedItem;

            if (selected != null)
            {
                if (cmbDirection.SelectedItem == null)
                {
                    cmbDirection.SelectedItem = SyncDirection.Import;
                }

                ProcessSyncDirection();
            }
        }

        private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            foreach (DataGridColumn col in dataGrid.Columns)
            {
                //only SyncAction should be modifiable, everything else should be process populated
                if (!col.Header.Equals("SyncAction"))
                {
                    col.IsReadOnly = true;
                }
            }
        }

        private void btnPrepare_Click(object sender, RoutedEventArgs e)
        {
            bool allIgnored = true;

            foreach (SyncItem si in SyncItems)
            {
                if (si.Prepare() && allIgnored)
                {
                    allIgnored = false;
                }
            }

            if (!allIgnored)
            {
                SetExecutionStatus(ExecStatus.Prepared);
            }
            else
            {
                Display.Message("all operations ignored");
            }
        }

        private void RevertNonExecuted(ExecStatus setStatusTo)
        {
            foreach (SyncItem si in SyncItems)
            {
                if (!si.Executed)
                {
                    si.Revert();
                }
            }

            SetExecutionStatus(setStatusTo);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            RevertNonExecuted(ExecStatus.Ready);
        }

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // you may want to make this async, and that's a great idea, but create a brand new method
        // and be prepared to change a lot of other SyncItem logic
        // I just wasted hours trying to do this, and decided to rollback all changes wiht git cuz 
        // it's way more of a headache than I have time for
        // As it stands, be happy I made the import/export syncItem population asynchronous
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            List<string> failingPaths = new List<string>();
            List<FileModelItem> fileModelItems = new List<FileModelItem>();

            string deviceName = Configuration.GetLocalDeviceDescription();

            foreach (SyncItem si in SyncItems)
            {
                if (!si.Execute())
                {
                    //exclude from pending verifications
                    si.SyncAction = SyncAction.Ignore;

                    switch (si.SyncDirection)
                    {
                        case SyncDirection.Export:
                            failingPaths.Add(si.HostPath);
                            break;
                        case SyncDirection.Import:
                            failingPaths.Add(si.ExtPath);
                            break;
                    }
                }
                else
                {
                    if(si.SyncDirection == SyncDirection.Export)
                    {
                        FileModelItem fmi = new FileModelItem(deviceName, si.HostPath);
                        fmi.GetHashes().Add(new HashModelItem(si.HostHash, TimeStamp.Now()));

                        foreach(string tag in TagsV4c.StringToList(si.HostTags))
                        {
                            fmi.GetTags().Add(tag);
                        }

                        fileModelItems.Add(fmi);
                    }

                }
            }

            SyncDirection currentDirection = (SyncDirection)cmbDirection.SelectedItem;
            SyncProfile sp = (SyncProfile)cmbSyncProfile.SelectedItem;

            if (sp != null &&
                currentDirection == SyncDirection.Export)
            {
                TagsV4c.ExportTagsForProfileToXml(deviceName, sp, fileModelItems);
            }

            if (failingPaths.Count == 0)
            {
                SetExecutionStatus(ExecStatus.Executed);
            }
            else
            {
                string msg = failingPaths.Count +
                    " operations failed for the following paths: " +
                    Environment.NewLine;

                foreach (var path in failingPaths)
                {
                    msg += path + Environment.NewLine;
                }

                Display.Message(msg);

                if (failingPaths.Count == SyncItems.Count)
                {
                    //all failed, nothing to verify
                    RevertNonExecuted(ExecStatus.Ready);
                }
                else
                {
                    //some succeeded, move onto verify stage (failed will be ignored)
                    RevertNonExecuted(ExecStatus.Executed);
                }
            }
        }

        private void btnVerify_Click(object sender, RoutedEventArgs e)
        {
            foreach (SyncItem si in SyncItems)
            {
                si.RefreshDestination(CurrentProfile, chkTagsFromXmlNotKeyVal.IsChecked.Value);
            }

            VerifyAllAndCleanup();
            
            SetExecutionStatus(ExecStatus.Verified);
        }

        private void VerifyAllAndCleanup()
        {
            //TODO: add tags in (just checks hashes for now)
            bool verified = true;

            foreach (var si in SyncItems)
            {
                if (si.SyncAction != SyncAction.Ignore) //don't verify ignored items
                {
                    if (!si.ExtHash.Equals(si.HostHash,
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        verified = false;
                    }
                }
            }

            if (!verified)
            {
                Display.Message("verification error: some hashes don't match");
            }
            else
            {
                Display.Message("all hashes verified. if xml tags were displayed for the imported media, use Import XML Media Utility to store tags.");
                //cleanup
                foreach (SyncItem si in SyncItems)
                {
                    si.CleanUp();
                }
            }
        }

        private void SetExecutionStatus(ExecStatus status)
        {
            switch (status)
            {
                case ExecStatus.Ready:

                    btnPrepare.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnExecute.Visibility = Visibility.Visible;
                    btnVerify.Visibility = Visibility.Collapsed;
                    btnPrepare.IsEnabled = true;
                    btnExecute.IsEnabled = false;
                    break;

                case ExecStatus.Prepared:

                    btnPrepare.Visibility = Visibility.Collapsed;
                    btnCancel.Visibility = Visibility.Visible;
                    btnExecute.Visibility = Visibility.Visible;
                    btnVerify.Visibility = Visibility.Collapsed;
                    btnCancel.IsEnabled = true;
                    btnExecute.IsEnabled = true;
                    break;

                case ExecStatus.Executed:

                    btnPrepare.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnExecute.Visibility = Visibility.Collapsed;
                    btnVerify.Visibility = Visibility.Visible;
                    btnPrepare.IsEnabled = false;
                    btnVerify.IsEnabled = true;
                    break;

                case ExecStatus.Verified:

                    btnPrepare.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnExecute.Visibility = Visibility.Visible;
                    btnVerify.Visibility = Visibility.Collapsed;
                    btnPrepare.IsEnabled = false; //post-verification, a new profile must...
                    btnExecute.IsEnabled = false; //...be reloaded to a ready status (reset)
                    break;
            }
        }

        private enum ExecStatus
        {
            Ready,
            Prepared,
            Executed,
            Verified
        }

        private void MenuItemAddProfile_Click(object sender, RoutedEventArgs e)
        {
            string name = UI.Prompt.Input("Profile Name?");

            if (!string.IsNullOrWhiteSpace(name))
            {
                db.EnsureSyncProfile(name.Trim());
                RefreshProfiles();
            }
            else
            {
                Display.Message("cancelled.");
            }
        }

        private void MenuItemRefreshProfiles_Click(object sender, RoutedEventArgs e)
        {
            RefreshProfiles();
        }

        private void MenuItemRawDef_Click(object sender, RoutedEventArgs e)
        {
            string output = db.GetErdRawSource();

            Display.Multiline("SQLite ERD Raw Source", output);
        }

        private void cmbActionDefault_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessActionDefault();         
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            ProcessSyncDirection(); //will reload SyncItems
        }

        private void chkOverrideDefaultAction_checkToggled(object sender, RoutedEventArgs e)
        {
            ProcessActionDefault();
        }

        private void chkTagsFromXmlNotKeyVal_checkToggled(object sender, RoutedEventArgs e)
        {
            ProcessSyncDirection();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UI.Utils.MaximizeMainWindow();
        }
    }
}
