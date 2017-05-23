using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Archivist
{
    public class ArchivistSourceExcerpt :INotifyPropertyChanged
    {
        private string _tagString = "";

        public int SourceExcerptId { get; set; }
        public int SourceId { get; set; }
        public string ExcerptValue { get; set; }
        public string ExcerptPages { get; set; }
        public string ExcerptBeginTime { get; set; }
        public string ExcerptEndTime { get; set; }

        public string TagString
        {
            get
            {
                return _tagString;
            }
            set
            {
                if (_tagString != value)
                {
                    _tagString = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
