using NineWorldsDeep.Core;
using System;

namespace NineWorldsDeep.Mnemosyne.V5
{
    public class MediaTagging
    {
        public int MediaTagId { get; set; }
        public string MediaTagValue { get; set; }
        public int MediaTaggingId { get; set; }
        public int MediaId { get; set; }
        public DateTime? TaggedAt { get; private set; }
        public DateTime? UntaggedAt { get; private set; }
        public string MediaHash { get; set; }

        public bool IsTagged()
        {
            if(UntaggedAt != null && TaggedAt != null)
            {
                return DateTime.Compare(UntaggedAt.Value, TaggedAt.Value) < 0;
            }

            if(UntaggedAt == null)
            {
                //TaggedAt is either null or greater
                //in either case, its considered "tagged"
                return true;
            }

            //if we reach here, UntaggedAt is not null, and
            //TaggedAt is null
            return false;
        }
        
        /// <summary>
        /// MediaTagValue and MediaHash will default to non-empty, non-null value.
        /// MediaTagId, MediaTaggingId, and MediaId will default to value 
        /// greater than zero.
        /// 
        /// if values are set for both objects on any of the above properties, 
        /// and differ, an error will be thrown
        /// 
        /// TimeStamps will be processed with SetTimeStamps(x, y), which will
        /// default to the most recent timestamp for each parameter.
        /// </summary>
        /// <param name="mt"></param>
        public void Merge(MediaTagging mt)
        {
            MediaTagValue = TryMergeString(MediaTagValue, mt.MediaTagValue);
            MediaHash = TryMergeString(MediaHash, mt.MediaHash);
            MediaTagId = TryMergeInt(MediaTagId, mt.MediaTagId);
            MediaTaggingId = TryMergeInt(MediaTaggingId, mt.MediaTaggingId);
            MediaId = TryMergeInt(MediaId, mt.MediaId);

            SetTimeStamps(mt.TaggedAt, mt.UntaggedAt);
        }

        private int TryMergeInt(int int1, int int2)
        {
            if(int1 > 0 && int2 > 0)
            {
                throw new Exception("unable to merge MediaTagging, conflicting values set on an exclusive property");
            }

            if(int1 > 0)
            {
                return int1;
            }

            return int2;
        }

        private string TryMergeString(string string1, string string2)
        {
            if(!string.IsNullOrWhiteSpace(string1) &&
                !string.IsNullOrWhiteSpace(string2) &&
                !string1.Equals(string2))
            {
                throw new Exception("unable to merge MediaTagging, conflicting values set on an exclusive property");
            }

            if (!string.IsNullOrWhiteSpace(string1))
            {
                return string1;
            }

            return string2;
        }

        /// <summary>
        /// 
        /// will resolve conflicts, newest date will always take precedence
        /// passing null values allowed as well to just set one or two
        /// null values always resolve to the non-null value(unless both null)
        /// 
        /// NOTE: THIS CONVERTS TO UTC, WHICH IS DEPENDENT ON DateTime.Kind
        /// This property defaults to local, so if you are passing a UTC time, make
        /// sure that the Kind is set to UTC or the conversion will
        /// be off. 
        /// 
        /// </summary>
        /// <param name="newTaggedAt"></param>
        /// <param name="newUntaggedAt"></param>
        public void SetTimeStamps(DateTime? newTaggedAt,
                                  DateTime? newUntaggedAt)
        {
            if (newTaggedAt != null)
            {
                if (newTaggedAt.Value.Kind != DateTimeKind.Utc)
                {
                    newTaggedAt = newTaggedAt.Value.ToUniversalTime();
                }

                if (TaggedAt == null ||
                   DateTime.Compare(TaggedAt.Value, newTaggedAt.Value) < 0)
                {
                    //TaggedAt is older or null
                    TaggedAt = newTaggedAt;
                }
            }

            if (newUntaggedAt != null)
            {
                if (newUntaggedAt.Value.Kind != DateTimeKind.Utc)
                {
                    newUntaggedAt = newUntaggedAt.Value.ToUniversalTime();
                }

                if (UntaggedAt == null ||
                   DateTime.Compare(UntaggedAt.Value, newUntaggedAt.Value) < 0)
                {
                    //UntaggedAt is older or null
                    UntaggedAt = newUntaggedAt;
                }
            }

        }

        internal void Tag()
        {
            TaggedAt = TimeStamp.NowUTC();
        }

        internal void Untag()
        {
            UntaggedAt = TimeStamp.NowUTC();
        }
    }
}