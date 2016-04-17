using NineWorldsDeep.Tagger;
using System.Collections.Generic;
using System.Windows;

namespace NineWorldsDeep.AudioBrowser
{
    public class FileElementTagExtractionAction : FileElementActionSubscriber
    {
        private TagMatrix masterTagMatrix;
        private ITaggerGrid tgrGrid;

        public FileElementTagExtractionAction(ITaggerGrid tg)
            : this(tg.GetTagMatrix(), tg)
        {
            //do nothing here, chained constructor
        }

        public FileElementTagExtractionAction(TagMatrix tm, ITaggerGrid tg)
        {
            masterTagMatrix = tm;
            tgrGrid = tg;
        }

        public void PerformAction(FileElement fe)
        {
            FileNameTagExtractor te = new FileNameTagExtractor();
            //string timeStamp = null;

            string fName =
                System.IO.Path.GetFileNameWithoutExtension(fe.Name);

            List<string> extractions = te.ExtractFromString(fName);

            List<string> currentTags = tgrGrid.GetTagsForCurrentSelection();

            List<string> nonDuplicates = new List<string>();

            foreach (string tag in extractions)
            {
                if (!currentTags.Contains(tag))
                {
                    nonDuplicates.Add(tag);
                }
            }

            if (nonDuplicates.Count > 0)
            {
                if (MessageBox.Show("Would you like to add tags:[" + TagString.Parse(nonDuplicates) + "]?",
                                    "Add Tags?",
                                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    tgrGrid.AppendTagsToCurrentTagStringAndUpdate(extractions);
                }

            }

        }
    }
}