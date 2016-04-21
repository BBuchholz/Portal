﻿using System;
using System.Collections.Generic;
using System.IO;

namespace NineWorldsDeep.FragmentCloud
{
    public class FileSystemFragment : Fragment
    {
        private bool initialized = false;

        public FileSystemFragment(string uri, bool lazyLoadChildren, params Fragment[] children)
            : base(uri, children)
        {
            Path = Converter.NwdUriToFileSystemPath(uri);
            if (!lazyLoadChildren)
            {
                InitializeChildren();
            }
        }

        protected override IEnumerable<Fragment> GetChildren()
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

        public string Path { get; private set; }

        private void InitializeChildren()
        {
            ClearChildren();

            //process children if folder
            if (Directory.Exists(Path))
            {
                //directories
                foreach (string dirPath in Directory.EnumerateDirectories(Path))
                {
                    Fragment f = PathToFileSystemFragment(dirPath);
                    AddChild(f);
                }

                //files
                foreach (string filePath in Directory.EnumerateFiles(Path))
                {
                    Fragment f = PathToFileSystemFragment(filePath);
                    AddChild(f);
                }
            }

            initialized = true;
        }

        private Fragment PathToFileSystemFragment(string path)
        {
            string fileOrDirName = System.IO.Path.GetFileName(path);
            string frgUri = this.URI;

            if (!frgUri.EndsWith("/"))
            {
                frgUri += "/";
            }

            frgUri += fileOrDirName;

            Fragment f = new FileSystemFragment(frgUri, true);

            return f;
        }
    }
}