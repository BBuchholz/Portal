using System.ComponentModel;

namespace NineWorldsDeep.Warehouse
{
    public class SyncMap : INotifyPropertyChanged
    {
        private SyncProfile _profile;
        private string _source, _destination;
        private SyncAction _defaultSyncAction;
        private SyncDirection _syncDirection;

        public event PropertyChangedEventHandler PropertyChanged;

        public SyncMap(SyncProfile profile,
                       SyncDirection direction,
                       SyncAction defaultAction)
        {
            this._profile = profile;
            _defaultSyncAction = defaultAction;
            _syncDirection = direction;
        }

        public SyncProfile Profile { get { return _profile; } }

        public string Source
        {
            get
            {
                return _source;
            }

            set
            {
                _source = value;
                OnPropertyChanged("Source");
            }
        }

        public SyncDirection SyncDirection
        {
            get
            {
                return _syncDirection;
            }
        }

        public SyncAction DefaultSyncAction
        {
            get { return _defaultSyncAction; }
            set
            {
                _defaultSyncAction = value;
                OnPropertyChanged("DefaultSyncAction");
            }
        }

        public string Destination
        {
            get
            {
                return _destination;
            }

            set
            {
                _destination = value;
                OnPropertyChanged("Destination");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}