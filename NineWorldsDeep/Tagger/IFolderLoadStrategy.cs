using System.Collections.Generic;

namespace NineWorldsDeep.Tagger
{
    public interface IFolderLoadStrategy
    {
        IEnumerable<string> GetFilesForFolder(string path);
    }
}