using System.Collections.Generic;

namespace NineWorldsDeep.Tagger
{
    public interface NwdDb
    {
        void AddFileElementsToDb(List<FileElement> feLst);
        List<FileElement> GetFileElementsFromDb();
    }
}