using NineWorldsDeep.Core;
using NineWorldsDeep.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace NineWorldsDeep.Synergy
{
    public class GauntletMenuController
    {
        private IListMatrix ilm;
        //private IGauntletDbAdapter mysqlDb;
        private IGauntletDbAdapter sqliteDb;
        private StoryboardStatusBar statusBar;
        private Parser.Parser p = new Parser.Parser();

        private List<string> filesToConsume = new List<string>();
        //private bool mysqlDbSavePending = false;
        private bool sqliteDbSavePending = false;
        
        public void Configure(MenuController mc,
                              IListMatrix ilm,
                              IGauntletDbAdapter sqliteDb,
                              StoryboardStatusBar statusBar)
        {
            this.ilm = ilm;
            this.sqliteDb = sqliteDb;
            this.statusBar = statusBar;
            mc.AddMenuItem("Gauntlet", "Export Active SQLite Lists To FileSystem Sync Folder", ExportActiveSqliteLists);
            //mc.AddMenuItem("Gauntlet", "Export Active MySQL Lists To FileSystem Sync Folder", ExportActiveMySqlLists);
            mc.AddMenuItem("Gauntlet", "Active List Management", ShowActiveListManagement);
            mc.AddMenuItem("Gauntlet", "Import Synced Files", ImportSyncedFiles);
            mc.AddMenuItem("Gauntlet", "Import Synced Archive Files", ImportSyncedArchiveFiles);
            mc.AddMenuItem("Gauntlet", "Consume All Imported Files", ConsumeAllImportedFiles);
            mc.AddMenuItem("Tools", "FileMatrix...", LaunchFileMatrix);
            mc.AddMenuItem("Tools", "ObjectGrid...", LaunchObjectGrid);
        }

        private void LaunchObjectGrid(object sender, RoutedEventArgs e)
        {
            new ObjectGridWindow().Show();
        }

        private void ConsumeAllImportedFiles(object sender, RoutedEventArgs e)
        {
            //if (mysqlDbSavePending)
            //{
            //    MessageBox.Show("MySQL Db Save Pending, Consume Aborted.");
            //}
            //else 
            if (sqliteDbSavePending)
            {
                MessageBox.Show("Sqlite Db Save Pending, Consume Aborted.");
            }
            else
            {
                //TODO:  use Display.Grid(filesToConsume) but verify that we can receive a boolean (add OK/Cancel to Display.Grid()?), for now, just displaying and then prompting
                Display.Grid("files to consume", filesToConsume.ToListItems());

                string msg = "The following files will be consumed, proceed?";

                foreach (string path in filesToConsume)
                {
                    msg += Environment.NewLine + path;
                }

                if (NineWorldsDeep.UI.Prompt.Confirm(msg, true))
                {
                    int count = 0;

                    foreach (string path in filesToConsume)
                    {
                        File.Delete(path);
                        count++;
                    }

                    filesToConsume.Clear();

                    statusBar.StatusBarText = count + " files consumed.";
                }
                else
                {
                    statusBar.StatusBarText = "Consume Aborted.";
                }
            }
        }

        public void ConfigureWithoutUI(IListMatrix ilm, IGauntletDbAdapter db)
        {
            this.ilm = ilm;
            //this.mysqlDb = db;
            this.statusBar = new StoryboardStatusBar(); //this prevents null exceptions where status is logged
        }

        private void LaunchFileMatrix(object sender, RoutedEventArgs e)
        {
            new FileMatrixWindow().Show();
        }

        private void ShowActiveListManagement(object sender, RoutedEventArgs e)
        {
            new ActiveListManagementWindow(sqliteDb).Show();
        }

        private void ExportActiveLists(IGauntletDbAdapter db)
        {
            int foundCount = 0;

            Configuration.EnsureDirectories();

            //foreach (ToDoList lst in db.GetActiveListItems())
            foreach (ToDoList lst in db.GetLists(true))
            {
                string phoneListPath = Configuration.GetPhoneSyncSynergyFilePath(lst.Name);
                string tabletListPath = Configuration.GetTabletSyncSynergyFilePath(lst.Name);

                if (!File.Exists(phoneListPath))
                {
                    File.WriteAllLines(phoneListPath, lst.Items.ToStringArray());
                }
                else
                {
                    foundCount++;
                }

                if (!File.Exists(tabletListPath))
                {
                    File.WriteAllLines(tabletListPath, lst.Items.ToStringArray());
                }
                else
                {
                    foundCount++;
                }
            }

            if (foundCount > 0)
            {
                MessageBox.Show(foundCount +
                    " files already existed and were ignored. " +
                    "To export fresh versions of all lists, first" +
                    " consume all files then export again.");
            }

            statusBar.StatusBarText = "Files Exported.";
        }

        private void ExportActiveSqliteLists(object sender, RoutedEventArgs e)
        {
            if (sqliteDb != null)
            {
                ExportActiveLists(sqliteDb);
            }
        }

        private void ExportActiveMySqlLists(object sender, RoutedEventArgs e)
        {
            Display.Message("not included in migration");
            //ExportActiveLists(mysqlDb);
        }

        private void ImportAndConsumeFiles(object sender, RoutedEventArgs e)
        {
            string msg = "You should use the new import, save, " +
                "then consume sequence. Are you sure you want to continue?";
            if (NineWorldsDeep.UI.Prompt.Confirm(msg, true))
            {
                ImportActiveSynergyFiles(true);
            }
            else
            {
                statusBar.StatusBarText = "Import And Consume Files Cancelled.";
            }
        }

        private void ImportAndConsumeArchives(object sender, RoutedEventArgs e)
        {
            string msg = "You should use the new import, save, " +
                "then consume sequence. Are you sure you want to continue?";
            if (NineWorldsDeep.UI.Prompt.Confirm(msg, true))
            {
                ImportSynergyArchiveFiles(true);
            }
            else
            {
                statusBar.StatusBarText = "Import And Consume Archives Cancelled.";
            }
            //statusBar.StatusBarText = Environment.NewLine + "Synergy Archives Imported.";
            //statusBar.StatusBarText += Environment.NewLine + "Imported Files Consumed, Be Sure To Save Before Exiting!";
        }

        private void ImportSyncedFiles(object sender, RoutedEventArgs e)
        {
            try
            {
                ImportSynergyFilesWithoutConsume();
            }
            catch (Exception ex)
            {
                Display.Exception(ex);
            }
        }

        private void ImportSyncedArchiveFiles(object sender, RoutedEventArgs e)
        {
            ImportSynergyArchiveFilesWithoutConsume();
        }

        public void ImportSynergyFilesWithoutConsume()
        {
            ImportActiveSynergyFiles(false);
        }

        public void ImportSynergyArchiveFilesWithoutConsume()
        {
            ImportSynergyArchiveFiles(false);
        }

        //TODO: cleanup this whole file, was cowboy-coded, and works, but could be more elegant

        private IEnumerable<string> GetAllSyncArchiveFilePaths()
        {
            List<string> filePaths = new List<string>();

            if (Directory.Exists(Configuration.PhoneSyncSynergyArchivedFolder))
            {
                List<string> phoneFilePaths =
                    Directory.GetFiles(Configuration.PhoneSyncSynergyArchivedFolder,
                                       "*.txt", SearchOption.TopDirectoryOnly).ToList();

                filePaths.AddRange(phoneFilePaths);
            }

            if (Directory.Exists(Configuration.TabletSyncSynergyArchivedFolder))
            {
                List<string> tabletFilePaths =
                    Directory.GetFiles(Configuration.TabletSyncSynergyArchivedFolder,
                                       "*.txt", SearchOption.TopDirectoryOnly).ToList();

                filePaths.AddRange(tabletFilePaths);
            }

            return filePaths;
        }

        public void ImportSynergyArchiveFiles(bool consumeAfterImport = true)
        {
            //IEnumerable<string> filePaths =
            //    Directory.GetFiles(Configuration.PhoneSyncSynergyArchivedFolder,
            //                       "*.txt", SearchOption.TopDirectoryOnly);

            IEnumerable<string> filePaths = GetAllSyncArchiveFilePaths();

            foreach (string filePath in filePaths)
            {
                string listName = Path.GetFileNameWithoutExtension(filePath);

                foreach (string item in File.ReadLines(filePath))
                {
                    ProcessItem(listName, item, true);
                }

                if (consumeAfterImport)
                {
                    File.Delete(filePath);
                }
                else
                {
                    filesToConsume.Add(filePath);
                    //mysqlDbSavePending = true;
                    sqliteDbSavePending = true;
                }
            }

            statusBar.StatusBarText = "Synergy Archive Files Imported.";

            if (consumeAfterImport)
            {
                statusBar.StatusBarText += Environment.NewLine + "Imported Files Consumed, Be Sure To Save Before Exiting!";
            }
        }


        [Obsolete("use LoadFromMySql()")]
        public void LoadFromDb()
        {
            LoadFromMySql();
        }

        public void LoadFromMySql()
        {
            Display.Message("not included in migration");
            //statusBar.StatusBarText = mysqlDb.Load(ilm);
        }

        public void LoadFromMySql(bool loadAllStatuses)
        {
            Display.Message("not included in migration");
            //statusBar.StatusBarText = mysqlDb.Load(ilm, loadAllStatuses);
        }

        public void LoadFromSqlite()
        {
            string msg = "sqlite db not found for load";

            if (sqliteDb != null)
            {
                msg = sqliteDb.Load(ilm);
            }

            statusBar.StatusBarText = msg;
        }

        [Obsolete("use SaveToMySql()")]
        public void SaveToDb()
        {
            SaveToMySql();
        }

        public void SaveToMySql()
        {
            Display.Message("not included in migration");
            //statusBar.StatusBarText = mysqlDb.Save(ilm);
            //mysqlDbSavePending = false;
        }

        public void SaveToSqlite()
        {
            string msg = "sqlite db not found for save";

            if (sqliteDb != null)
            {
                msg = sqliteDb.Save(ilm);
            }
            //set this either way, because if db is null, we don't need to listen for pending saves
            sqliteDbSavePending = false;

            statusBar.StatusBarText = msg;
        }

        private void ProcessItem(string listName, string item, bool archived)
        {
            try
            {

                string updateListName = listName;
                string updateItem = item;
                bool updateCompleted = false;
                bool updateArchived = archived;

                if (item.ExtractByKey("completed") != null)
                {
                    updateItem = item.ExtractByKey("completed");
                    updateCompleted = true;
                }

                if (!string.IsNullOrWhiteSpace(
                    item.ExtractLastByKey("completedAt")))
                {
                    updateCompleted = true;

                    if (!string.IsNullOrWhiteSpace(item.ExtractByKey("item")))
                    {
                        updateItem = item.ExtractByKey("item");
                    }
                    else
                    {
                        updateItem = p.TrimLastKeyVal("completedAt", item);
                    }

                }

                if (GauntletUtils.IsCategorizedItem(updateItem))
                {
                    updateListName = GauntletUtils.ParseCategory(item);
                    updateItem = GauntletUtils.TrimCategory(item);
                }

                Update(updateListName,
                       updateItem,
                       updateCompleted,
                       updateArchived);

            }
            catch (Exception)
            {
                Display.Message("error processing listName: [" + listName + "] item: [" + item + "]. ignoring.");
            }
        }

        private void Update(string listName, string item, bool completed, bool archived)
        {
            ilm.EnsureList(listName).UpdateStatus(item, completed, archived);
        }

        private IEnumerable<string> GetAllSynergySyncFilePaths()
        {
            List<string> filePaths = new List<string>();

            if (Directory.Exists(Configuration.PhoneSyncSynergyFolder))
            {
                IEnumerable<string> phoneFilePaths =
                    Directory.GetFiles(Configuration.PhoneSyncSynergyFolder,
                                       "*.txt", SearchOption.TopDirectoryOnly);

                filePaths.AddRange(phoneFilePaths);
            }

            if (Directory.Exists(Configuration.TabletSyncSynergyFolder))
            {
                IEnumerable<string> tabletFilePaths =
                Directory.GetFiles(Configuration.TabletSyncSynergyFolder,
                                   "*.txt", SearchOption.TopDirectoryOnly);

                filePaths.AddRange(tabletFilePaths);
            }

            return filePaths;
        }

        public void ImportActiveSynergyFiles(bool consumeAfterImport)
        {
            //foreach list in synergy folder, update status (completed == whatever the file says)
            //IEnumerable<string> filePaths =
            //    Directory.GetFiles(Configuration.PhoneSyncSynergyFolder, 
            //                       "*.txt", SearchOption.TopDirectoryOnly);

            IEnumerable<string> filePaths = GetAllSynergySyncFilePaths();

            foreach (string filePath in filePaths)
            {
                string listName = Path.GetFileNameWithoutExtension(filePath);
                //process list
                foreach (string item in File.ReadLines(filePath))
                {

                    ProcessItem(listName, item, false);
                }

                if (consumeAfterImport)
                {
                    File.Delete(filePath);
                }
                else
                {
                    filesToConsume.Add(filePath);
                    //mysqlDbSavePending = true;
                    sqliteDbSavePending = true;
                }
            }

            statusBar.StatusBarText = "Active Synergy Files Imported.";

            if (consumeAfterImport)
            {
                statusBar.StatusBarText += Environment.NewLine + "Imported Files Consumed, Be Sure To Save Before Exiting!";
            }
        }
    }
}