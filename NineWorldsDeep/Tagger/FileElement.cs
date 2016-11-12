using NineWorldsDeep.Core;
using NineWorldsDeep.Db;
using NineWorldsDeep.UI;
using System;
using System.IO;

namespace NineWorldsDeep.Tagger
{
    public class FileElement
    {
        //intentionally private, use static methods
        private FileElement(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(path);
            ModifiedAt = File.GetLastWriteTime(path); 
        }

        /// <summary>
        /// will return a file element with tag string, 
        /// path, and name populated from supplied
        /// TagMatrix
        /// </summary>
        /// <param name="path"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static FileElement FromPath(string path, TagMatrix matrix)
        {
            //return new FileElement
            //{
            //    Path = path,
            //    Name = System.IO.Path.GetFileName(path),
            //    TagString = matrix.GetTagString(path)
            //};

            FileElement fe = new FileElement(path);
            fe.TagString = matrix.GetTagString(path);

            return fe;
        }

        /// <summary>
        /// will return a file element with path and 
        /// name populated, TagString will remain empty
        /// </summary>
        /// <param name="path"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static FileElement FromPath(string path)
        {
            //return new FileElement
            //{
            //    Path = path,
            //    Name = System.IO.Path.GetFileName(path)
            //};

            return new FileElement(path);
        }

        public void PopulateTagString(TagMatrix tm)
        {
            TagString = tm.GetTagString(Path);
        }

        public string Path { get; set; }
        public string Name { get; set; }
        public string TagString { get; set; }
        public DateTime ModifiedAt { get; private set; }

        public override bool Equals(object obj)
        {
            if(obj ==null || GetType() != obj.GetType())
            {
                return false;
            }

            FileElement fe = (FileElement)obj;
            return fe.Name.Equals(Name) && fe.Path.Equals(Path);
        }

        public override int GetHashCode()
        {
            //from: http://stackoverflow.com/questions/1008633/gethashcode-problem-using-xor

            int hash = 23;
            hash = hash * 37 + Name.GetHashCode();
            hash = hash * 37 + Path.GetHashCode();
            return hash;

        }

        public void MoveToTrash(Db.Sqlite.DbAdapterSwitch db)
        {
            if (File.Exists(Path))
            {
                try
                {
                    string destFile =
                        Configuration.GetTrashFileFromPath(Path);
                    File.Move(Path, destFile);  
                }
                catch (Exception ex)
                {
                    Display.Exception(ex);
                }
            }

            //even if it doesn't exist, purge from db (it 
            //exists as a file element for a reason, most 
            //likely loaded from a stale entry)
            db.DeleteFile(Path);
        }
    }
      
}