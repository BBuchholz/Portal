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

        public void Add(MediaTagging mt)
        {
            MediaTaggings.Add(mt);
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

            Add(newMt);

            return newMt;
        }
    }
}
