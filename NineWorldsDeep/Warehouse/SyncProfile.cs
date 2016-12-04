using NineWorldsDeep.Core;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NineWorldsDeep.Warehouse
{
    public class SyncProfile
    {
        private string _name;
        private string _syncRoot;
        private SyncMapCollection _syncMapCol;

        public SyncProfile(string name)
        {
            // remove all non-alphanumeric characters
            // other than hyphens 
            // (profile name is used for folder name)

            Regex rgx = new Regex("[^a-zA-Z0-9-]");
            _name = rgx.Replace(name, string.Empty);

            EnsureSyncRoot();

            _syncMapCol = new SyncMapCollection();
        }

        private void EnsureSyncRoot()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new Exception("SyncProfile.Name is empty");
            }

            //for any node to be a proper NWD node,
            //it must have an NWD/ root and an NWD-MEDIA/intake 
            //folder for warehouse support

            _syncRoot = Configuration.SyncRoot(Name);

            //NWD/ should always have a config folder (will create sync root also)
            Directory.CreateDirectory(Configuration.SyncRootConfigFolder(Name));

            //create NWD-MEDIA root and media folders
            Directory.CreateDirectory(Configuration.SyncFolderMedia(Name, "audio"));
            Directory.CreateDirectory(Configuration.SyncFolderMedia(Name, "images"));
            Directory.CreateDirectory(Configuration.SyncFolderMedia(Name, "pdfs"));
        }

        public SyncMapCollection SyncMaps
        { get { return _syncMapCol; } }

        public string SyncRoot
        {
            get
            {
                return _syncRoot;
            }
        }

        public string Name { get { return _name; } }

        public override string ToString()
        {
            return "SyncProfile: " + _name;
        }

        internal SyncProfile ShallowCopy()
        {
            return (SyncProfile)this.MemberwiseClone();
        }
    }
}