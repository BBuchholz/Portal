using NineWorldsDeep.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace NineWorldsDeep.FragmentCloud
{
    public class FileSystemFragment : TapestryNode
    {
        private bool initialized = false;

        public FileSystemFragment(string uri, bool lazyLoadChildren, params TapestryNode[] children)
            : base(uri, children)
        {
            Path = Converter.NwdUriToFileSystemPath(uri);
            if (!lazyLoadChildren)
            {
                InitializeChildren();
            }
        }

        public string Path { get; private set; }

        protected override IEnumerable<TapestryNode> GetChildren()
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

            return "[[[FILESYSTEM OBJECT NOT FOUND]]]";
        }

        private string GetMultiLineFileDetails()
        {
            string detail = "";

            //short name
            detail += "Short Name: " +
                GetShortName() +
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

            return detail;
        }

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
                    TapestryNode f = PathToFileSystemFragment(dirPath);
                    AddChild(f);
                }

                //files
                foreach (string filePath in Directory.EnumerateFiles(Path))
                {
                    TapestryNode f = PathToFileSystemFragment(filePath);
                    AddChild(f);
                }
            }

            initialized = true;
        }

        private TapestryNode PathToFileSystemFragment(string path)
        {
            string fileOrDirName = System.IO.Path.GetFileName(path);
            string frgUri = this.URI;

            if (!frgUri.EndsWith("/"))
            {
                frgUri += "/";
            }

            frgUri += fileOrDirName;

            TapestryNode f = new FileSystemFragment(frgUri, true);

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
                Display.Message("open containing folder in explorer goes here");
            }
        }

        public override string GetShortName()
        {
            int shortNameLength = 15;
            string name = Converter.NwdUriNodeName(URI);

            name = System.IO.Path.GetFileNameWithoutExtension(name);

            if (name.Length > shortNameLength)
            {
                name = name.Substring(0, shortNameLength - 6) + "..." +
                    name.Substring(name.Length - 3, 3);
            }

            return name;
        }
    }
}