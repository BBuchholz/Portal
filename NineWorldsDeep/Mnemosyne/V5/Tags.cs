using NineWorldsDeep.Core;
using NineWorldsDeep.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Mnemosyne.V5
{
    public class Tags
    {
        public static void UpdateTagStringForHash(string hash, string oldTagString, string newTagString)
        {
            int mediaIdForHash = Hashes.MediaIdForHash(hash);

            List<MediaTagging> taggings = 
                TagStringChangesToTaggings(mediaIdForHash, oldTagString, newTagString);

            Configuration.DB.MediaSubset.EnsureMediaTaggings(taggings);
        }

        private static List<MediaTagging> TagStringChangesToTaggings(int mediaIdForHash, string oldTagString, string newTagString)
        {
            //get tags that changed (in one not the other, check both for new and removed)
            string[] seps = { "," };

            //var oldTags = oldTagString
            //    .ToLower()
            //    .Split(seps, StringSplitOptions.RemoveEmptyEntries)
            //    .Select(x => x.Trim())
            //    .Where(x => !string.IsNullOrWhiteSpace(x));

            //var newTags = newTagString
            //    .ToLower()
            //    .Split(seps, StringSplitOptions.RemoveEmptyEntries)
            //    .Select(x => x.Trim())
            //    .Where(x => !string.IsNullOrWhiteSpace(x));

            var oldTags = oldTagString
                .Split(seps, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x));

            var newTags = newTagString
                .Split(seps, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x));

            List<Tag> tagsToBeTagged = new List<Tag>();
            List<Tag> tagsToBeUntagged = new List<Tag>();

            foreach (string tag in newTags)
            {
                if (!oldTags.Contains(tag))
                {
                    //its a new one
                    tagsToBeTagged.Add(new Tag() { TagValue = tag });
                }
            }

            foreach (string tag in oldTags)
            {
                if (!newTags.Contains(tag))
                {
                    //its been removed
                    tagsToBeUntagged.Add(new Tag() { TagValue = tag });
                }
            }

            var db = Configuration.DB.MediaSubset;

            //pass the list into the db.PopulateTagIds(List<Tag>)
            db.PopulateTagIds(tagsToBeTagged);
            db.PopulateTagIds(tagsToBeUntagged);

            //create media taggings for just the changed ones
            //each needs MediaId and MediaTagId set, also TaggedAt and UntaggedAt
            //should be set if removed or added accordingly
            List<MediaTagging> taggings = new List<MediaTagging>();

            foreach (Tag tag in tagsToBeTagged)
            {
                var mt = new MediaTagging()
                {
                    MediaId = mediaIdForHash,
                    MediaTagId = tag.TagId
                };

                mt.Tag();
                taggings.Add(mt);
            }

            foreach (Tag tag in tagsToBeUntagged)
            {
                var mt = new MediaTagging()
                {
                    MediaId = mediaIdForHash,
                    MediaTagId = tag.TagId
                };

                mt.Untag();
                taggings.Add(mt);
            }

            return taggings;
        }

        //public static List<MediaTagging> GetTaggingsForHash(string hash)
        //{
        //    //exactly what it says
        //    return Configuration.DB.MediaSubset.GetMediaTaggingsForHash(hash);
        //}

        //public static string GetTagStringForHash(string hash)
        //{
        //    return ToTagString(GetTaggingsForHash(hash));
        //}
        
        public static string ToTagString(List<MediaTagging> list)
        {
            return string.Join(", ", 
                list.Where(y => y.IsTagged())
                    .Select(x => x.MediaTagValue));
        }

        public static List<string> ToTagList(string tagString)
        {
            List<string> tags = new List<string>();

            foreach(string tag in tagString.Split(','))
            {
                tags.Add(tag.Trim());
            }

            return tags;
        }
    }
}
