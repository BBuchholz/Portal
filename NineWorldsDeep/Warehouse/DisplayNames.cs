using System;

namespace NineWorldsDeep.Warehouse
{
    public class DisplayNames
    {
        public static string FromHash(SyncProfile sp, SyncDirection direction, string hash)
        {
            ////////////////////TAKEN FROM pre-absorbtion NwdSynergy.ObjectGridWindow/////////////
            //
            ////load displayNameIndex.txt
            //List<DisplayNameIndexEntry> displayNames = new List<DisplayNameIndexEntry>();
            //string path = Configuration.GetPhoneSyncConfigFilePath("displayNameIndex");
            //foreach (string line in File.ReadAllLines(path))
            //{
            //    displayNames.Add(new DisplayNameIndexEntry()
            //    {
            //        DisplayName = p.Extract("displayName", line),
            //        DevicePath = p.Extract("path", line)
            //    });
            //}

            ////load fileHashIndex.txt
            //List<FileHashIndexEntry> fileHashes = new List<FileHashIndexEntry>();
            //path = Configuration.GetPhoneSyncConfigFilePath("fileHashIndex");
            //foreach (string line in File.ReadAllLines(path))
            //{
            //    fileHashes.Add(new FileHashIndexEntry()
            //    {
            //        DevicePath = p.Extract("path", line),
            //        SHA1Hash = p.Extract("sha1Hash", line)
            //    });
            //}

            //IEnumerable<SynergyRowObject> joinResult =
            //    from displayName in displayNames
            //    join fileHash in fileHashes
            //    on displayName.DevicePath equals fileHash.DevicePath
            //    select new SynergyRowObject
            //    {
            //        SHA1Hash = fileHash.SHA1Hash,
            //        DisplayName = displayName.DisplayName,
            //        DevicePath = fileHash.DevicePath
            //    };

            return "";
        }

        public static void ExportNamesForProfile(SyncProfile profile, string hash)
        {
            //needs implementation
        }
    }
}