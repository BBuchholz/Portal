using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NineWorldsDeep
{
    public class WorkbenchMenuController
    {
        WorkbenchListViewsController wc;
        WorkbenchWindow w;
        string fuzzyIntersectionProcessedTag = "fuzzyIntersectionProcessed";

        public void Configure(WorkbenchWindow w, 
                              WorkbenchListViewsController wc)
        {
            this.w = w;
            this.wc = wc;
            //w.Menu.AddMenuItem("List Operations",
            //                   "Fuzzy Intersection {x, y} => z",
            //                   FuzzyIntersectionXYtoZ);
            w.Menu.AddMenuItem("List Operations",
                               "Fuzzy Intersection {x, y} => x++",
                               FuzzyIntersectionXYtoXpp);
            w.Menu.AddMenuItem("List Operations",
                               "Set Selected {x, y} => x++",
                               SetSelectedXYtoXpp);
            w.Menu.AddMenuItem("List Operations",
                               "Merge {x, y, z1, z2} => {x, y, z}",
                               MergeXYZZtoXYZ);
            w.Menu.AddMenuItem("List Operations",
                               "Send First",
                               SendFirst);
            w.Menu.AddMenuItem("List Operations",
                               "Send Last",
                               SendLast);
            w.Menu.AddMenuItem("List Operations",
                               "Remove Last",
                               RemoveLast);
            w.Menu.AddMenuItem("List Operations",
                               "Remove First",
                               RemoveFirst);
            w.Menu.AddMenuItem("List Operations",
                               "Mark Fuzzy Intersection Unprocessed {x}",
                               MarkFuzzyIntersectionUnprocessed);
        }

        private void MergeXYZZtoXYZ(object sender, RoutedEventArgs e)
        {
            IEnumerable<Fragment> listZ1 = wc.GetFragments(2);
            IEnumerable<Fragment> listZ2 = wc.GetFragments(3);

            if(listZ1 == null || listZ2 == null)
            {
                MessageBox.Show("Merge is for combining result sets," +
                                " indexes 2 and 3 must be populated.");
            }
            else
            {
                List<Fragment> listZFinal = new List<Fragment>();

                foreach(Fragment f in listZ1)
                {
                    listZFinal.Add(f);
                }

                foreach(Fragment f in listZ2)
                {
                    listZFinal.Add(f);
                }

                //remove old
                wc.RemoveLast();
                wc.RemoveLast();

                w.Receive(listZFinal);
            }
        }

        private void SetSelectedXYtoXpp(object sender, RoutedEventArgs e)
        {
            string displayKeyY = wc.GetDisplayKey(1);
            string selectedValueY = wc.GetDisplayValue(1);
            Fragment f = wc.GetSelectedFragment(0);

            if(displayKeyY == null || 
                selectedValueY == null ||
                f == null)
            {
                MessageBox.Show("DisplayKey Y, Fragment Y, and Fragment X must all be selected.");
            }
            else
            {
                f.SetMeta(displayKeyY, selectedValueY);
                w.Receive(f.ToList());
            }
        }

        private void MarkFuzzyIntersectionUnprocessed(object sender, RoutedEventArgs e)
        {
            IEnumerable<Fragment> ie = wc.GetFragments(0);
            if(ie != null)
            {
                foreach (Fragment f in ie)
                {
                    f.SetMeta(fuzzyIntersectionProcessedTag, "False");
                }
            }
        }

        private void SendFirst(object sender, RoutedEventArgs e)
        {
            wc.SendFirst();
        }

        private void SendLast(object sender, RoutedEventArgs e)
        {
            wc.SendLast();
        }

        private void RemoveFirst(object sender, RoutedEventArgs e)
        {
            wc.RemoveFirst();
        }

        private void RemoveLast(object sender, RoutedEventArgs e)
        {
            wc.RemoveLast();
        }

        private void FuzzyIntersectionXYtoXpp(object sender, RoutedEventArgs e)
        {
            IEnumerable<Fragment> listX = wc.GetFragments(0);
            IEnumerable<Fragment> listY = wc.GetFragments(1);

            do
            {
                int segmentSize = 1;
                bool parseSucceeded = false;

                do
                {
                    parseSucceeded =
                        Int32.TryParse(Prompt.Input("Enter integer segment size"), 
                                       out segmentSize);

                } while (!parseSucceeded);

                IEnumerable<Fragment> listXSegment =
                listX.GetUnprocessedSegment(segmentSize, fuzzyIntersectionProcessedTag);

                if (listX != null && listY != null)
                {
                    //process here and store in listZ
                    foreach (Fragment f in listXSegment)
                    {

                        string bestMatchValue = "[no match found]";
                        string bestMatchKey = "[no match found]";
                        double bestPercentMatch =
                            f.DisplayValue.PercentMatchTo(bestMatchValue);

                        foreach (Fragment f2 in listY)
                        {
                            string currentMatchValue = f2.DisplayValue;
                            string currentMatchKey = f2.DisplayKey;
                            double currentPercentMatch =
                                f.DisplayValue.PercentMatchTo(currentMatchValue);

                            if (currentPercentMatch > bestPercentMatch)
                            {
                                bestMatchKey = currentMatchKey;
                                bestMatchValue = currentMatchValue;
                                bestPercentMatch = currentPercentMatch;
                            }

                        }
                        f.SetMeta("bestMatchKey", bestMatchKey);
                        f.SetMeta("bestMatchValue", bestMatchValue);
                        f.SetMeta("bestPercentMatch",
                                  bestPercentMatch.ToString());

                        string concatenated = f.DisplayValue +
                            " => " + bestMatchValue + " ("
                            + bestPercentMatch.ToString() + ")";

                        f.SetMeta("concatenated", concatenated);
                        f.SetProcessed(fuzzyIntersectionProcessedTag);

                        if(Double.Parse(f.GetMeta("bestPercentMatch")) < 1)
                        {
                            f.FlagForReview("concatenated");
                        }
                    }
                }

            } while (Prompt.Confirm("Process another segment?"));
            
            w.Receive(listX.GetProcessed(fuzzyIntersectionProcessedTag).DeepCopy());
            wc.RefreshMetaKeys(0); 
        }
    }
}
