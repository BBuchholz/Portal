using NineWorldsDeep.Core;
using NineWorldsDeep.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NineWorldsDeep.Synergy
{
    public class SyncHandler
    {
        public List<SynergyList> Lists { get; private set; }
        public List<string> FilesToConsume { get; private set; }

        public SyncHandler()
        {
            Lists = new List<SynergyList>();
            FilesToConsume = new List<string>();
            DbSavePending = false;
        }

        public bool DbSavePending { get; set; }

        /// <summary>
        /// return number of files already existing
        /// and ignored
        /// </summary>
        /// <param name="ignoredCount"></param>
        /// <returns></returns>
        public int ExportLists(int ignoredCount, IEnumerable<SynergyList> lists)
        {
            Configuration.EnsureDirectories();

            foreach (SynergyList lst in lists)
            {
                string phoneListPath = Configuration.GetPhoneSyncSynergyFilePath(lst.Name);
                string tabletListPath = Configuration.SyncFileSynergyPath("galaxy-a", lst.Name);
                string logosListPath = Configuration.SyncFileSynergyPath("logos", lst.Name);

                WriteListToPath(lst, phoneListPath, ignoredCount);
                WriteListToPath(lst, tabletListPath, ignoredCount);
                WriteListToPath(lst, logosListPath, ignoredCount);
                                
            }

            return ignoredCount;
        }

        private void WriteListToPath(SynergyList lst, string path, int ignoredCount)
        {
            if (!File.Exists(path))
            {
                File.WriteAllLines(path, lst.Items.ToFragmentArray());                
            }
            else
            {
                ignoredCount++;
            }
        }

        private void ImportFiles(IEnumerable<string> filePaths, bool archiveIfNot)
        {            
            foreach (string filePath in filePaths)
            {
                string listName = Path.GetFileNameWithoutExtension(filePath);
                //process list
                foreach (string item in File.ReadLines(filePath))
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        SynergyItem si;

                        if (NwdParser.IsAtomic(item))
                        {
                            si = new SynergyItem()
                            {
                                Fragment = item
                            };
                        }
                        else
                        {
                            si = new SynergyItem()
                            {
                                Item = item
                            };
                        }

                        if(archiveIfNot && 
                            string.IsNullOrWhiteSpace(si.ArchivedAt))
                        {
                            si.ArchiveNow();
                        }

                        Lists.EnsureList(listName).AddItem(si);
                    }
                }

                FilesToConsume.Add(filePath);
                DbSavePending = true;
            }
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

            if (Directory.Exists(Configuration.SyncFolderSynergy("galaxy-a")))
            {
                IEnumerable<string> tabletFilePaths =
                    Directory.GetFiles(Configuration.SyncFolderSynergy("galaxy-a"),
                                       "*.txt", SearchOption.TopDirectoryOnly);

                filePaths.AddRange(tabletFilePaths);
            }

            if (Directory.Exists(Configuration.SyncFolderSynergy("logos")))
            {
                IEnumerable<string> logosFilePaths =
                    Directory.GetFiles(Configuration.SyncFolderSynergy("logos"),
                                       "*.txt", SearchOption.TopDirectoryOnly);

                filePaths.AddRange(logosFilePaths);
            }

            return filePaths;
        }

        public void ImportSyncedArchiveFiles()
        {
            ImportFiles(GetAllSyncArchiveFilePaths(), true);
        }

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

            if (Directory.Exists(Configuration.SyncFolderSynergyArchive("galaxy-a")))
            {
                List<string> galaxyFilePaths =
                    Directory.GetFiles(Configuration.SyncFolderSynergyArchive("galaxy-a"),
                                       "*.txt", SearchOption.TopDirectoryOnly).ToList();

                filePaths.AddRange(galaxyFilePaths);
            }
            
            if (Directory.Exists(Configuration.SyncFolderSynergyArchive("logos")))
            {
                List<string> logosFilePaths =
                    Directory.GetFiles(Configuration.SyncFolderSynergyArchive("logos"),
                                       "*.txt", SearchOption.TopDirectoryOnly).ToList();

                filePaths.AddRange(logosFilePaths);
            }

            return filePaths;
        }

        public void ImportSynergyFiles()
        {
            ImportFiles(GetAllSynergySyncFilePaths(), false);
        }

        public int ConsumeFiles()
        {
            int count = 0;

            foreach (string path in FilesToConsume)
            {
                File.Delete(path);
                count++;
            }

            FilesToConsume.Clear();

            return count;
        }
    }
}