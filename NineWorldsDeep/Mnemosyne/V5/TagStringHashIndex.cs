using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Mnemosyne.V5
{
    public class TagStringHashIndex
    {
        private Dictionary<string, Media> hashToMedia = 
            new Dictionary<string, Media>();

        public string GetTagStringForHash(string hash)
        {
            Media media = GetMediaByHash(hash);
            return Tags.ToTagString(media.MediaTaggings);
        }

        public Media GetMediaByHash(string hash)
        {
            if (!hashToMedia.ContainsKey(hash.ToLower()))
            {
                hashToMedia.Add(hash.ToLower(), new Media());
            }

            return hashToMedia[hash.ToLower()];
        }

        public void Add(Media m)
        {
            GetMediaByHash(m.MediaHash).Merge(m);
        }
    }
}
