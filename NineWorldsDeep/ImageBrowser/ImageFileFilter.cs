using NineWorldsDeep.Tagger;
using System.Collections.Generic;
using System.Linq;

namespace NineWorldsDeep.ImageBrowser
{
    public class ImageFileFilter : FilePathFilter
    {
        public override
            IEnumerable<string> Filter(IEnumerable<string> ieFilePaths)
        {
            return ieFilePaths.Where(s => s.ToLower().EndsWith(".bmp") ||
                                          s.ToLower().EndsWith(".gif") ||
                                          s.ToLower().EndsWith(".ico") ||
                                          s.ToLower().EndsWith(".jpg") ||
                                          s.ToLower().EndsWith(".png") ||
                                          s.ToLower().EndsWith(".tiff"));
        }
    }
}