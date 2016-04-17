using System.Collections.Generic;

namespace NineWorldsDeep.Tagger
{
    public class SyncTools
    {
        public static List<FileElement> CalculateElementsToBeAdded(List<FileElement> inputList, List<FileElement> dbList)
        {
            List<FileElement> outputList = new List<FileElement>();

            foreach (FileElement fe in inputList)
            {
                if (!dbList.Contains(fe))
                {
                    outputList.Add(fe);
                }
            }

            return outputList;
        }
    }
}