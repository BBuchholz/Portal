using System.ComponentModel;

namespace NineWorldsDeep.Studio
{
    public class ArpaBetPair : INotifyPropertyChanged
    {
        private string mWord, mArpaBetValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Word
        {
            get { return mWord; }

            set
            {
                mWord = value;
                OnPropertyChanged("Word");
            }
        }

        public string ArpaBetValue
        {
            get { return mArpaBetValue; }

            set
            {
                mArpaBetValue = value;
                OnPropertyChanged("ArpaBetValue");
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