using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Core
{
    public abstract class TaggingBase
    {
        public DateTime? TaggedAt { get; private set; }
        public DateTime? UntaggedAt { get; private set; }

        protected int TryMergeInt(int int1, int int2)
        {
            if (int1 > 0 && int2 > 0)
            {
                throw new Exception("unable to merge tagging, conflicting values set on an exclusive property");
            }

            if (int1 > 0)
            {
                return int1;
            }

            return int2;
        }

        protected string TryMergeString(string string1, string string2)
        {
            if (!string.IsNullOrWhiteSpace(string1) &&
                !string.IsNullOrWhiteSpace(string2) &&
                !string1.Equals(string2))
            {
                throw new Exception("unable to merge tagging, conflicting values set on an exclusive property");
            }

            if (!string.IsNullOrWhiteSpace(string1))
            {
                return string1;
            }

            return string2;
        }

        public bool IsTagged()
        {
            if (UntaggedAt != null && TaggedAt != null)
            {
                return DateTime.Compare(UntaggedAt.Value, TaggedAt.Value) < 0;
            }

            if (UntaggedAt == null)
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

        public void Tag()
        {
            TaggedAt = TimeStamp.NowUTC();
        }

        public void Untag()
        {
            UntaggedAt = TimeStamp.NowUTC();
        }

    }
}
