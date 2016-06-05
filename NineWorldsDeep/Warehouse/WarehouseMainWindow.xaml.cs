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
    public partial class WarehouseMainWindow : Window
    {
        private SyncItemCollection _syncItemCol;

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
                //LoadDefaultProfiles();
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

            SqliteDbAdapter db = new SqliteDbAdapter();
            db.Load(phone);
            db.Load(tablet);

            cmbSyncProfile.Items.Add(phone);
            cmbSyncProfile.Items.Add(tablet);
        }

        private void RefreshProfiles()
        {
            cmbSyncProfile.Items.Clear();

            SqliteDbAdapter db = new SqliteDbAdapter();

            foreach (SyncProfile sp in db.GetAllSyncProfiles())
            {
                cmbSyncProfile.Items.Add(sp);
            }
        }

        private void CollectionsViewSource_Filter(object sender, FilterEventArgs e)
        {
            //TODO: filtering, grouping, and sorting
            //some reference for datagrid grouping and sorting
            //https://msdn.microsoft.com/en-us/library/ff407126(v=vs.100).aspx
            //TODO: LICENSE NOTES
            //TODO: images for "move", "move and stamp", and "copy"
            //http://stackoverflow.com/questions/14066601/displaying-images-based-on-column-value-in-wpf-datagrid
            //TODO: LICENSE NOTES
        }

        private void MenuItemSyncMappingsWindow_Click(object sender, RoutedEventArgs e)
        {
            SyncProfile selected = (SyncProfile)cmbSyncProfile.SelectedItem;
            if (selected != null)
            {
                new SyncMapWindow(selected).Show();
            }
        }

        private void ProcessSyncDirection()
        {
            SetExecutionStatus(ExecStatus.Ready);

            if (CurrentProfile != null &&
                cmbDirection.SelectedItem != null)
            {
                SyncDirection selected = (SyncDirection)cmbDirection.SelectedItem;

                switch (selected)
                {
                    case SyncDirection.Import:
                        PopulateImports(CurrentProfile);
                        break;

                    case SyncDirection.Export:
                        PopulateExports(CurrentProfile);
                        break;
                }
            }

            ProcessActionDefault();
        }

        private void ProcessActionDefault()
        {
            var sa = (SyncAction)cmbActionDefault.SelectedItem;
            var objDirection = cmbDirection.SelectedItem;            

            foreach (var si in SyncItems)
            {
                si.SyncAction = sa;

                //by default, just copy all untagged imports
                if (cmbDirection.SelectedItem != null)
                {
                    var sd = (SyncDirection)cmbDirection.SelectedItem;

                    if (sd == SyncDirection.Import &&
                        string.IsNullOrWhiteSpace(si.ExtTags))
                    {
                        si.SyncAction = SyncAction.Copy;
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

        private void PopulateExports(SyncProfile sp)
        {
            SyncItems.Clear();

            string profileName = sp.Name;

            foreach (SyncMap sm in sp.SyncMaps)
            {
                if (sm.SyncDirection == SyncDirection.Export)
                {
                    string exportRootPath = sm.Source;

                    //get files
                    foreach (string filePath in Directory.GetFiles(exportRootPath, "*.*", SearchOption.TopDirectoryOnly))
                    {
                        string hash = Hashes.SHA1(filePath);
                        string path = filePath;
                        string tags = Tags.FromHash(sp, sm.SyncDirection, hash);
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

        private void PopulateImports(SyncProfile sp)
        {
            SyncItems.Clear();

            string profileName = sp.Name;

            foreach (SyncMap sm in sp.SyncMaps)
            {
                if (sm.SyncDirection == SyncDirection.Import)
                {
                    string importRootPath = sm.Source;

                    //get files
                    foreach (string filePath in Directory.GetFiles(importRootPath, "*.*", SearchOption.TopDirectoryOnly))
                    {
                        string hash = Hashes.SHA1(filePath);
                        string path = filePath;
                        string tags = Tags.FromHash(sp, sm.SyncDirection, hash);
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

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            List<string> failingPaths = new List<string>();

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
                si.RefreshDestination(CurrentProfile);
            }

            VerifyAllAndCleanup();

            ProcessSyncDirection(); //will reload SyncItems

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
                Display.Message("all hashes verified");
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
            SqliteDbAdapter db = new SqliteDbAdapter();

            string name = UI.Prompt.Input("Profile Name?");

            if (!string.IsNullOrWhiteSpace(name))
            {
                db.EnsureProfile(name.Trim());
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
            SqliteDbAdapter db = new SqliteDbAdapter();
            string output = db.GetErdRawSource();

            Display.Multiline("SQLite ERD Raw Source", output);
        }

        private void cmbActionDefault_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessActionDefault();         
        }
    }
}
