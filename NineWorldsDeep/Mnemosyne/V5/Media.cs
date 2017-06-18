using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Mnemosyne.V5
{
    public class Media
    {
        public int MediaId { get; set; }
        public string MediaFileName { get; set; }
        public string MediaDescription { get; set; }
        public string MediaHash { get; set; }
        public List<MediaTagging> MediaTaggings { get; private set; }
        public MultiMap<string, DevicePath> DevicePaths { get; private set; }

        public Media()
        {
            MediaTaggings = new List<MediaTagging>();
            DevicePaths = new MultiMap<string, DevicePath>();
        }

        public List<DevicePath> AllDevicePaths
        {
            get
            {
                return DevicePaths.AllValues();
            }
        }

        public List<string> DeviceNames()
        {
            return DevicePaths.Keys.ToList();
        }

        //public void Add(MediaTagging mt)
        //{
        //    MediaTaggings.Add(mt);
        //}
        
        public void Add(MediaTagging mt)
        {
            if (!string.IsNullOrWhiteSpace(mt.MediaTagValue))
            {
                GetTag(mt.MediaTagValue).Merge(mt);
            }
        }

        internal bool IsDevicePathFilterMatch(string filter)
        {
            return IsDeviceNameFilterMatch(filter) || IsDevicePathValueFilterMatch(filter);
        }

        internal bool IsDeviceNameFilterMatch(string filter)
        {
            //TODO: combine IsDeviceNameFilterMatch and IsDevicePathFilterMatch into a single method with an OR in the WHERE clause
            return DevicePaths.Keys
                .Where(k => k.ToLower().Contains(filter.ToLower()))
                .ToList()
                .Count > 0;
        }

        internal bool IsDevicePathValueFilterMatch(string filter)
        {
            //TODO: combine IsDeviceNameFilterMatch and IsDevicePathFilterMatch into a single method with an OR in the WHERE clause
            return DevicePaths.AllValues()
                .Where(p => p.DevicePathValue.ToLower().Contains(filter.ToLower()))
                .ToList()
                .Count > 0;
        }

        /// <summary>
        /// MediaFileName, MediaDescription, and MediaHash will default to non-empty, non-null value.
        /// MediaId will default to value greater than zero.
        /// 
        /// if values are set for both objects on any of the above properties, 
        /// and differ, an error will be thrown
        /// 
        /// MediaTaggings will be Merged with any existing taggings
        /// 
        /// DevicePaths will simply be added, without regard to duplication
        /// </summary>
        /// <param name="m"></param>
        public void Merge(Media m)
        {
            MediaFileName = TryMergeString(MediaFileName, m.MediaFileName);
            MediaDescription = TryMergeString(MediaDescription, m.MediaDescription);
            MediaHash = TryMergeString(MediaHash, m.MediaHash);

            MediaId = TryMergeInt(MediaId, m.MediaId);

            foreach(MediaTagging mt in m.MediaTaggings)
            {
                GetTag(mt.MediaTagValue).Merge(mt);
            }

            foreach(DevicePath dp in m.DevicePaths.AllValues())
            {
                Add(dp);
            }
        }

        private int TryMergeInt(int int1, int int2)
        {
            if (int1 > 0 && int2 > 0)
            {
                throw new Exception("unable to merge MediaTagging, conflicting values set on an exclusive property");
            }

            if (int1 > 0)
            {
                return int1;
            }

            return int2;
        }

        private string TryMergeString(string string1, string string2)
        {
            if (!string.IsNullOrWhiteSpace(string1) &&
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

        public void Add(DevicePath dp)
        {
            DevicePaths.Add(dp.DeviceName, dp);
        }

        public MediaTagging GetTag(string tag)
        {
            foreach(MediaTagging mt in MediaTaggings)
            {
                if(mt.MediaTagValue.Equals(tag))
                {
                    return mt;
                }
            }

            MediaTagging newMt = new MediaTagging()
            {
                MediaTagValue = tag
            };

            MediaTaggings.Add(newMt);

            return newMt;
        }
    }
}
