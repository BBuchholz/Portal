using System.ComponentModel;

namespace NineWorldsDeep.Studio
{
    public class ArpaBetPair : INotifyPropertyChanged
    {
        private string mWord, mStrippedArpaBetValue;

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

        public string StrippedArpaBetValue
        {
            get { return mStrippedArpaBetValue; }

            set
            {
                mStrippedArpaBetValue = value;
                OnPropertyChanged("StrippedArpaBetValue");
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