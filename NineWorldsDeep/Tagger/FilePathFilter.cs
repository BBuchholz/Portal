using System.Collections.Generic;

namespace NineWorldsDeep.Tagger
{
    public abstract class FilePathFilter
    {
        public abstract IEnumerable<string> Filter(IEnumerable<string> ieFilePaths);
    }
}