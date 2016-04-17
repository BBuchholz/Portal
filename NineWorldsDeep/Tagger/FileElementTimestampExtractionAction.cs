using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NineWorldsDeep.Tagger
{
    class FileElementTimestampExtractionAction : FileElementActionSubscriber
    {
        private TagMatrix masterTagMatrix;
        private ITaggerGrid tgrGrid;

        public FileElementTimestampExtractionAction(TagMatrix tm, ITaggerGrid tg)
        {
            masterTagMatrix = tm;
            tgrGrid = tg;
        }

        public void PerformAction(FileElement fe)
        {
            if (!masterTagMatrix.FileElementHasMetaTag(fe, "timestamp"))
            {
                TimeStampExtractor tse = new TimeStampExtractor();
                string timeStamp = null;

                List<string> extractions = tse.ExtractFromString(fe.Name);

                if (extractions.Count > 0)
                {
                    if (extractions.Count == 1)
                    {
                        timeStamp = extractions.First();
                    }
                    else
                    {
                        MessageBox.Show("ambiguous timestamp matches, unable to auto extract");
                    }
                }
                else
                {
                    //TODO:extract from file attributes if none exists - github issue #7
                    string fromFile = tse.ExtractFromFileAttributes(fe.Path);

                    if (fromFile != null)
                    {
                        //we try for file name first, but if not found, use file attributes
                        timeStamp = fromFile;
                    }
                }

                if (timeStamp != null)
                {
                    if (MessageBox.Show("Would you like to add metaTag timestamp:[" + timeStamp + "]?",
                                       "Add TimeStamp?",
                                       MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        tgrGrid.AppendTagToCurrentTagStringAndUpdate("timestamp: " + timeStamp);
                    }

                }
            }
        }
    }
}
