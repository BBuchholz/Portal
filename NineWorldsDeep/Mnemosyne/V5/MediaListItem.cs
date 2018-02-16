using NineWorldsDeep.Core;
using NineWorldsDeep.Warehouse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Mnemosyne.V5
{
    public class MediaListItem
    {
        public Media Media { get; private set; }
        private string localPath = "";

        public MediaListItem(string path)
        {
            Media = new Media();
            localPath = path;
            AddPath(path);
        }

        public MediaListItem(string path, string deviceName, string mediaHash)
        {
            Media = new Media()
            {
                MediaHash = mediaHash
            };

            localPath = path;
            AddPath(path, deviceName);
        }

        public void AddPath(string filePath, string deviceName)
        {
            var dp = new DevicePath()
            {
                DeviceName = deviceName,
                DevicePathValue = filePath
            };
            
            Media.Add(dp);
        }

        public void AddPath(string filePath)
        {
            var dp = new DevicePath()
            {
                DeviceName = Configuration.GetLocalDeviceDescription(),
                DevicePathValue = filePath
            };

            if (File.Exists(filePath))
            {
                dp.VerifyPresent();
            }
            else
            {
                dp.VerifyMissing();
            }

            Media.Add(dp);
        }

        public void HashMedia()
        {
            if (File.Exists(localPath))
            {
                Media.MediaHash = Hashes.Sha1ForFilePath(localPath);
            }
        }

        public string GetTagString()
        {
            return Tags.ToTagString(Media.MediaTaggings);
        }

        public void SetTagsFromTagString(string tagString)
        {
            List<string> existingTags = Tags.ToTagList(GetTagString());
            List<string> newTags = Tags.ToTagList(tagString);

            foreach(string tag in existingTags)
            {
                if (!newTags.Contains(tag))
                {
                    Media.GetTag(tag).Untag();
                }
            }

            foreach(string tag in newTags)
            {
                if (!existingTags.Contains(tag))
                {
                    Media.GetTag(tag).Tag();
                }
            }
        }
    }
}
