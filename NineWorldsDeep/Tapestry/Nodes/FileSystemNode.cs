using NineWorldsDeep.Core;
using NineWorldsDeep.FragmentCloud;
using NineWorldsDeep.Warehouse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    public class FileSystemNode : Tapestry.TapestryNode
    {
        private bool initialized = false;

        public FileSystemNode(string uri, bool lazyLoadChildren, int mediaDeviceId, params Tapestry.TapestryNode[] children)
            : base(uri, children)
        {
            //Path = Converter.NwdUriToFileSystemPath(uri);
            Path = uri;
            MediaDevicePathId = -1;
            MediaId = -1;
            MediaDeviceId = mediaDeviceId;
            MediaPathId = -1;

            if (!lazyLoadChildren)
            {
                InitializeChildren();
            }
        }

        public string Path { get; private set; }
        public string Hash { get; private set; }
        public int MediaDevicePathId { get; set; }
        public int MediaId { get; set; }
        public int MediaDeviceId { get; set; }
        public int MediaPathId { get; set; }

        protected override IEnumerable<Tapestry.TapestryNode> GetChildren()
        {
            //if (base.GetChildren().Count() == 0)
            if (!initialized)
            {
                InitializeChildren();
            }

            return base.GetChildren();
        }

        public override string ToMultiLineDetail()
        {
            if (Directory.Exists(Path))
            {
                return base.ToMultiLineDetail();
            }

            if (File.Exists(Path))
            {
                return GetMultiLineFileDetails();
            }

            //return "[[[FILESYSTEM OBJECT NOT FOUND]]]";

            return GetNonlocalMultilineFileDetails();
        }

        private string GetNonlocalMultilineFileDetails()
        {
            string detail = "";

            //created at
            detail += "Nonlocal File Name: " +
                System.IO.Path.GetFileName(Path) +
                Environment.NewLine +
                Environment.NewLine;

            //path
            detail += "Nonlocal File Path: " + Path +
                Environment.NewLine;

            return detail;
        }

        private string GetMultiLineFileDetails()
        {
            string detail = "";

            //short name
            detail += "Long Name: " +
                GetLongName() +
                Environment.NewLine;

            //created at
            detail += "Created: " +
                File.GetCreationTime(Path).ToShortDateString() +
                Environment.NewLine;

            //last modified
            detail += "Modified: " +
                File.GetLastWriteTime(Path).ToShortDateString() +
                Environment.NewLine;

            //size
            detail += "Size: " +
                GetSizePrettyPrint(new FileInfo(Path).Length) +
                Environment.NewLine;

            //mime type
            detail += "Mime Type: " +
                GetMimeType(Path) +
                Environment.NewLine;

            //file extension
            detail += "Extension: " +
                System.IO.Path.GetExtension(Path) +
                Environment.NewLine;

            //hash and hash status
            //detail += "Hash: " +
            //    Hashes.Sha1ForFilePath(Path) +
            //    Environment.NewLine;
            detail += "Hash: " + 
                CalculateAndStoreHash() + 
                Environment.NewLine;

            return detail;
        }

        public string CalculateAndStoreHash()
        {
            string newestHash = Hashes.Sha1ForFilePath(Path);

            //only hit the database if we have to
            if (!newestHash.Equals(Hash, StringComparison.CurrentCultureIgnoreCase)
                 && MediaDeviceId > 0 && File.Exists(Path))
            {
                Hash = newestHash;

                Configuration.DB.MediaSubset.StoreHashForPath(
                    MediaDeviceId, Path, Hash);
            }

            return Hash;
        }

        //private string GetHashAndStatus(string path)
        //{
        //    string hash = Hashes.Sha1ForFilePath(path);

        //    string msg = hash + " (Error)";

        //    try
        //    {
        //        //go to media table in db
        //        //if hash already exists, msg = hash + " (Confirmed)"
        //        //else create/store and msg = hash + " (Stored)"
        //    }
        //    catch (Exception)
        //    {
        //        //leave as default
        //    }

        //    return msg;
        //}

        private string GetSizePrettyPrint(long bytes)
        {
            double kb = bytes / 1024f;
            double mb = kb / 1024f;
            double gb = mb / 1024f;

            if (gb > 1)
            {
                return String.Format("{0:0.00} GB", gb);
            }

            if (mb > 1)
            {
                return String.Format("{0:0.00} MB", mb);
            }

            if (kb > 1)
            {
                return String.Format("{0:0.00} KB", kb);
            }

            return String.Format("{0:0.00} bytes", bytes);
        }

        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        private void InitializeChildren()
        {
            ClearChildren();

            //process children if folder
            if (Directory.Exists(Path))
            {
                //directories
                foreach (string dirPath in Directory.EnumerateDirectories(Path))
                {
                    Tapestry.TapestryNode f = LocalPathToFileSystemNode(dirPath);
                    AddChild(f);
                }

                //files
                foreach (string filePath in Directory.EnumerateFiles(Path))
                {
                    Tapestry.TapestryNode f = LocalPathToFileSystemNode(filePath);
                    AddChild(f);
                }
            }

            initialized = true;
        }

        public override TapestryNodeType NodeType
        {
            get
            {
                if (Configuration.FileIsAudio(Path))
                {
                    return TapestryNodeType.Audio;
                }
                else if (Configuration.FileIsImage(Path))
                {
                    return TapestryNodeType.Image;
                }
                else
                {
                    return base.NodeType;
                }
            }
        }

        private Tapestry.TapestryNode LocalPathToFileSystemNode(string path)
        {
            string fileOrDirName = System.IO.Path.GetFileName(path);
            string frgUri = this.URI;

            if (!frgUri.EndsWith("/"))
            {
                frgUri += "/";
            }

            frgUri += fileOrDirName;

            if(MediaDeviceId < 1)
            {
                //throw new Exception("invalid MediaDeviceId: " + MediaDeviceId);
                UI.Display.Message("Warning: MediaDeviceId is not set, be advised that it has the potential to cause issues");
            }

            Tapestry.TapestryNode f = new FileSystemNode(frgUri, true, MediaDeviceId);

            return f;
        }

        public override void PerformSelectionAction()
        {
            if (Directory.Exists(Path))
            {
                //do nothing
            }

            if (File.Exists(Path))
            {
                //Display.Message("open containing folder in explorer goes here");
            }
        }

        public override string GetShortName()
        {
            int shortNameLength = 15;

            string name = GetLongName();

            if (name.Length > shortNameLength)
            {
                name = name.Substring(0, shortNameLength - 7) + "..." +
                    name.Substring(name.Length - 4, 4);
            }

            return name;
        }

        public override string GetLongName()
        {
            string name = Converter.NwdUriNodeName(URI);
            name = System.IO.Path.GetFileName(name);
            return name;
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }
    }
}
