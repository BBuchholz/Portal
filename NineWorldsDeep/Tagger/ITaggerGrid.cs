using System.Collections.Generic;

namespace NineWorldsDeep.Tagger
{
    public interface ITaggerGrid
    {
        List<string> GetTagsForCurrentSelection();
        void SetFolderLoadStrategy(IFolderLoadStrategy fls);
        void AppendTagsToCurrentTagStringAndUpdate(List<string> extractions);
        void AppendTagToCurrentTagStringAndUpdate(string v);
        TagMatrix GetTagMatrix();
    }
}