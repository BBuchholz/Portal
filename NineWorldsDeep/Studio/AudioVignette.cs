using System.Collections.ObjectModel;

namespace NineWorldsDeep.Studio
{
    class AudioVignette
    {
        private ObservableCollection<FileElement> _Elements =
            new ObservableCollection<FileElement>();

        public string Name { get; set; }
        public string DirectoryPath { get; set; }
        public ObservableCollection<FileElement> Elements { get { return _Elements; } }
        public int ElementCount { get { return Elements.Count; } }
    }
}