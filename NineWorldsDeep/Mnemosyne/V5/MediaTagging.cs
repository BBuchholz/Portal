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