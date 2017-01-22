using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace NineWorldsDeep.Warehouse
{
    public class SyncItem : INotifyPropertyChanged
    {//TODO: LICENSE NOTES
        //OnPropertyChanged reference: https://msdn.microsoft.com/en-us/library/ms743695(v=vs.100).aspx

        private SyncMap _syncMap;

        private string _extDisplayName, _extTags, _extHash, _extPath,
                       _hostDisplayName, _hostTags, _hostHash, _hostPath;

        private bool _executed;

        private SyncAction _syncAction;
        private SyncDirection _syncDirection;

        public event PropertyChangedEventHandler PropertyChanged;

        public SyncItem(SyncMap parent)
        {
            _syncMap = parent;
            _syncAction = parent.DefaultSyncAction;
            _syncDirection = parent.SyncDirection;
            Executed = false;
        }

        public bool Executed
        {
            get { return _executed; }

            private set
            {
                _executed = value;
                OnPropertyChanged("Executed");
            }
        }

        public SyncAction SyncAction
        {
            get { return _syncAction; }

            set
            {
                _syncAction = value;
                OnPropertyChanged("SyncAction");
            }
        }

        public string ExtTags
        {
            get { return _extTags; }

            set
            {
                _extTags = value;
                OnPropertyChanged("ExtTags");
            }
        }

        public string HostTags
        {
            get { return _hostTags; }

            set
            {
                _hostTags = value;
                OnPropertyChanged("HostTags");
            }
        }

        public string ExtHash
        {
            get { return _extHash; }

            set
            {
                _extHash = value;
                OnPropertyChanged("ExtHash");
            }
        }

        public string HostHash
        {
            get { return _hostHash; }

            set
            {
                _hostHash = value;
                OnPropertyChanged("HostHash");
            }
        }

        public string ExtPath
        {
            get { return _extPath; }

            set
            {
                _extPath = value;
                OnPropertyChanged("ExtPath");
            }
        }

        public string HostPath
        {
            get { return _hostPath; }

            set
            {
                _hostPath = value;
                OnPropertyChanged("HostPath");
            }
        }

        public SyncDirection SyncDirection
        {
            get { return _syncDirection; }
        }

        public string ExtDisplayName
        {
            get { return _extDisplayName; }

            set
            {
                _extDisplayName = value;
                OnPropertyChanged("ExtDisplayName");
            }
        }

        public string HostDisplayName
        {
            get { return _hostDisplayName; }

            set
            {
                _hostDisplayName = value;
                OnPropertyChanged("HostDisplayName");
            }
        }
        
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string PreparePath(string path)
        {
            string fileName = Path.GetFileName(path);
            string toFilePath = "";

            if (SyncAction == SyncAction.MoveAndStamp ||
                SyncAction == SyncAction.CopyAndStamp)
            {
                fileName = PrepareTimeStampedPath(fileName);
            }

            toFilePath = Path.Combine(_syncMap.Destination,
                                      fileName);

            return toFilePath;
        }

        private string PrepareTimeStampedPath(string fileName)
        {
            string firstPart = fileName.Split('-')[0];

            DateTime foundDate;

            if(DateTime.TryParseExact(firstPart,
                                      "yyyyMMddHHmmss",
                                      new CultureInfo("en-US"),
                                      DateTimeStyles.None,
                                      out foundDate))
            {
                string toStrip = foundDate.ToString("yyyyMMddHHmmss") + "-";

                fileName = fileName.ReplaceFirst(toStrip, "");
            }

            string timestamp =
                        DateTime.Now.ToString("yyyyMMddHHmmss");

            fileName = timestamp + "-" + fileName;

            return fileName;
        }

        /// <summary>
        /// returns true if successful, false if ignored
        /// </summary>
        /// <returns></returns>
        public bool Prepare()
        {
            if (SyncAction != SyncAction.Ignore)
            {
                switch (SyncDirection)
                {
                    case SyncDirection.Export:

                        ExtPath = PreparePath(HostPath);
                        break;

                    case SyncDirection.Import:

                        HostPath = PreparePath(ExtPath);
                        break;
                }

                return true;
            }

            return false;
        }

        public void Revert()
        {
            switch (SyncDirection)
            {
                case SyncDirection.Export:

                    ExtPath = "";

                    break;

                case SyncDirection.Import:

                    HostPath = "";

                    break;
            }

            Executed = false;
        }

        public void RefreshDestination(SyncProfile sp, bool useXmlInsteadOfKeyVal)
        {
            switch (SyncDirection)
            {
                case SyncDirection.Export:

                    ExtHash = Hashes.Sha1ForFilePath(ExtPath);
                    ExtTags = TagsV4c.GetTagStringForHash(ExtHash);

                    break;

                case SyncDirection.Import:

                    HostHash = Hashes.Sha1ForFilePath(HostPath);
                    HostTags = TagsV4c.ImportForHash(sp, HostHash, useXmlInsteadOfKeyVal);

                    break;
            }
        }

        /// <summary>
        /// returns true if successful, false if operation 
        /// throws an exception of any sort
        /// </summary>
        /// <returns></returns>
        public bool Execute()
        {
            try
            {
                Db.Sqlite.DbAdapterSwitch db = new Db.Sqlite.DbAdapterSwitch();

                if (SyncAction != SyncAction.Ignore)
                {
                    switch (SyncDirection)
                    {
                        case SyncDirection.Export:

                            //anything but SyncAction.Ignore is a
                            //copy operation first and foremost
                            //(move ops will delete original 
                            // AFTER verification)

                            if (string.IsNullOrWhiteSpace(ExtPath))
                            {
                                return false;
                            }
                            else if (!File.Exists(ExtPath))
                            {
                                File.Copy(HostPath, ExtPath);
                            }
                            else
                            {
                                //if the hashes are the same, we can
                                //just accept this as executed,
                                //but if they are different, we
                                //have to return false for an
                                //unsuccessful operation
                                //we never want to overwrite data
                                //unless the user explicitly decides
                                //to do so.

                                string destHash = Hashes.Sha1ForFilePath(ExtPath);
                                if (!destHash.Equals(HostHash,
                                    StringComparison.CurrentCultureIgnoreCase))
                                {
                                    return false;
                                }
                            }
                            
                            TagsV4c.ExportTagsForProfile(_syncMap.Profile, HostHash);
                            DisplayNames.ExportNamesForProfile(_syncMap.Profile, HostHash);

                            break;

                        case SyncDirection.Import:

                            //anything but SyncAction.Ignore is a
                            //copy operation first and foremost
                            //(move ops will delete original 
                            // AFTER verification)

                            if (string.IsNullOrWhiteSpace(HostPath))
                            {
                                return false;
                            }
                            else if (!File.Exists(HostPath))
                            {
                                File.Copy(ExtPath, HostPath);
                            }
                            else
                            {
                                //if the hashes are the same, we can
                                //just accept this as executed,
                                //but if they are different, we
                                //have to return false for an
                                //unsuccessful operation
                                //we never want to overwrite data
                                //unless the user explicitly decides
                                //to do so.

                                string destHash = Hashes.Sha1ForFilePath(HostPath);
                                if (!destHash.Equals(ExtHash,
                                    StringComparison.CurrentCultureIgnoreCase))
                                {
                                    return false;
                                }
                            }

                            List<string> tags = TagsV4c.StringToList(ExtTags);
                            string profileName = _syncMap.Profile.Name;
                            db.StoreImport(profileName, ExtPath, HostPath, ExtHash, tags);

                            break;
                    }
                }

                Executed = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void CleanUp()
        {
            if (Executed && //ignore anything that failed
                    (SyncAction == SyncAction.Move ||
                     SyncAction == SyncAction.MoveAndStamp))
            {
                switch (SyncDirection)
                {
                    case SyncDirection.Export:

                        File.Delete(HostPath);
                        break;

                    case SyncDirection.Import:

                        File.Delete(ExtPath);
                        break;
                }
            }
        }
    }

    public enum SyncAction
    {
        MoveAndStamp,
        Move,
        CopyAndStamp,
        Copy,
        Ignore
    }
}