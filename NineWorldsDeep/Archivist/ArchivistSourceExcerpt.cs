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
        public List<SourceExcerptTagging> ExcerptTaggings
        {
            get { return Taggings.Values.ToList(); }
        }
        
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
            private set
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
            if (string.IsNullOrWhiteSpace(tagging.MediaTag.MediaTagValue))
            {
                throw new Exception("cannot add tagging with empty tag value");
            }

            if (Taggings.ContainsKey(tagging.MediaTag.MediaTagValue))
            {
                Merge(tagging);
            }
            else
            {
                Taggings.Add(tagging.MediaTag.MediaTagValue, tagging);

                string newTagString;

                if(TagString.Trim() != "")
                {
                    newTagString = TagString + ", " + tagging.MediaTag.MediaTagValue;    
                }
                else
                {
                    newTagString = tagging.MediaTag.MediaTagValue;
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

        /// <summary>
        /// will return tagging if it already exists 
        /// if the tagging is not found, it will create 
        /// a new one with Excerpt property set to this
        /// excerpt and the Tag property set to a MediaTag
        /// with the mediaTagId not set. No other properties
        /// are set for any internal objects
        /// 
        /// Anywhere the Tag property is intended to persist to
        /// the database, be sure to populate the mediaTagId
        /// in some manner
        /// </summary>
        /// <param name="tagValue"></param>
        /// <returns></returns>
        public SourceExcerptTagging GetTagging(string tagValue)
        {
            SourceExcerptTagging set;

            if (Taggings.ContainsKey(tagValue))
            {
                set = Taggings[tagValue];
            }
            else
            {
                set = new SourceExcerptTagging()
                {
                    Excerpt = this,
                    MediaTag = new MediaTag() { MediaTagValue = tagValue }
                };

                Taggings[tagValue] = set;
            }

            return set;
        }

        public void SetTagsFromTagString(string newTagString)
        {
            foreach(string newTag in 
                Tags.GetAddedTagValues(TagString, newTagString))
            {
                GetTagging(newTag).Tag();
            }

            foreach(string oldTag in 
                Tags.GetRemovedTagValues(TagString, newTagString))
            {
                GetTagging(oldTag).Untag();
            }
        }
    }
}
