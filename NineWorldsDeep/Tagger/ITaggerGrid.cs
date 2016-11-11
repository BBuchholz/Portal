using System.Collections.Generic;

namespace NineWorldsDeep.Tagger
{
    public interface ITaggerGrid
    {
        bool HasPendingChanges { get; }
        FileElement SelectedFileElement { get; }
        IEnumerable<FileElement> FileElements { get; }

        List<string> GetTagsForCurrentSelection();
        void SetFolderLoadStrategy(IFolderLoadStrategy fls);
        void AppendTagsToCurrentTagStringAndUpdate(List<string> extractions);
        void AppendTagToCurrentTagStringAndUpdate(string v);
        TagMatrix GetTagMatrix();
        void AddSelectionChangedListener(FileElementActionSubscriber feas);
        void SaveToFile(string saveFilePath);
        void AddFolder(string folderPath);
        void LoadFromFile(string loadFilePath);
        void SaveToDb();
        void LoadFromDb(string filePathTopFolder);
        void Clear();
    }
}