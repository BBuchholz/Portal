using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Mnemosyne.V5;

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
        
        protected Dictionary<string, SourceExcerptTagging> Taggings { get; private set; }        

        public ArchivistSourceExcerpt()
        {
            Taggings = new Dictionary<string, SourceExcerptTagging>();
        }

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

        public void Add(SourceExcerptTagging tagging)
        {
            if (string.IsNullOrWhiteSpace(tagging.Tag.MediaTagValue))
            {
                throw new Exception("cannot add tagging with empty tag value");
            }

            if (Taggings.ContainsKey(tagging.Tag.MediaTagValue))
            {
                Merge(tagging);
            }
            else
            {
                Taggings.Add(tagging.Tag.MediaTagValue, tagging);

                string newTagString;

                if(TagString.Trim() != "")
                {
                    newTagString = TagString + ", " + tagging.Tag.MediaTagValue;    
                }
                else
                {
                    newTagString = tagging.Tag.MediaTagValue;
                }

                TagString = newTagString;
            }
        }

        private void Merge(SourceExcerptTagging tagging)
        {
            //does nothing right now, stubbing out

            //should mimic MediaTagging.Merge(...)
            //create an abstract base class, Tagging, to hold
            //the TryMergeInt and TryMergeString for mutual
            //usage (change them to protected)
            //derive each from the base class
        }
    }
}
