using NineWorldsDeep.Tagger;
using System.Collections.Generic;
using System.IO;

namespace NineWorldsDeep.ImageBrowser
{
    public class LoadImagesStrategy : IFolderLoadStrategy
    {
        ImageFileFilter iff = new ImageFileFilter();

        public IEnumerable<string> GetFilesForFolder(string path)
        {
            //return Directory.GetFiles(path, "*.*")
            //    .Where(s => s.ToLower().EndsWith(".bmp") ||
            //                s.ToLower().EndsWith(".gif") ||
            //                s.ToLower().EndsWith(".ico") ||
            //                s.ToLower().EndsWith(".jpg") ||
            //                s.ToLower().EndsWith(".png") ||
            //                s.ToLower().EndsWith(".tiff"));            

            return iff.Filter(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories));
        }

    }
}