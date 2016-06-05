using System.Collections.Generic;
using System.ComponentModel;

namespace NineWorldsDeep.Synergy
{//TODO: LICENSE NOTES
    //from: http://stackoverflow.com/questions/1315621/implementing-inotifypropertychanged-does-a-better-way-exist
    internal class SynergyRowObject : INotifyPropertyChanged
    {
        private string _devicePath;
        private string _displayName;
        private string _fileName;
        private string _sha1Hash;
        private int _fileNameMatchCount;

        public string DevicePath
        {
            get { return _devicePath; }
            set { SetField(ref _devicePath, value, "DevicePath"); }
        }
        public string DisplayName
        {
            get { return _displayName; }
            set { SetField(ref _displayName, value, "DisplayName"); }
        }
        public string FileName
        {
            get { return _fileName; }
            set { SetField(ref _fileName, value, "FileName"); }
        }

        public int FileNameMatchCount
        {
            get { return _fileNameMatchCount; }
            set { SetField(ref _fileNameMatchCount, value, "FileNameMatchCount"); }
        }

        public string SHA1Hash
        {
            get { return _sha1Hash; }
            set { SetField(ref _sha1Hash, value, "SHA1Hash"); }
        }

        // boiler-plate
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}