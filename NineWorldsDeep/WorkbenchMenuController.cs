using NineWorldsDeep.Core;
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
                               "Mark Fuzzy Intersection Unprocessed {x}",
                               MarkFuzzyIntersectionUnprocessed);
            w.Menu.AddMenuItem("Workbench",
                               "Send First",
                               SendFirst);
            w.Menu.AddMenuItem("Workbench",
                               "Send Last",
                               SendLast);
            w.Menu.AddMenuItem("Workbench",
                               "Remove Last",
                               RemoveLast);
            w.Menu.AddMenuItem("Workbench",
                               "Remove First",
                               RemoveFirst);
            w.Menu.AddMenuItem("Fragment",
                               "Display Selected {x}",
                               DisplayFragmentX);
            w.Menu.AddMenuItem("Fragment",
                               "Display Selected {y}",
                               DisplayFragmentY);
            w.Menu.AddMenuItem("Fragment",
                               "Display Selected {z}",
                               DisplayFragmentZ);
        }

        private void DisplayFragment(Fragment f)
        {
            if(f != null)
            {
                MessageBox.Show(f.ToMultiLineString());
            }
            else
            {
                MessageBox.Show("fragment null");
            }
        }
        private void DisplayFragmentZ(object sender, RoutedEventArgs e)
        {
            DisplayFragment(wc.GetSelectedFragment(2));
        }

        private void DisplayFragmentY(object sender, RoutedEventArgs e)
        {
            DisplayFragment(wc.GetSelectedFragment(1));
        }

        private void DisplayFragmentX(object sender, RoutedEventArgs e)
        {
            DisplayFragment(wc.GetSelectedFragment(0));
        }

        private void MergeXYZZtoXYZ(object sender, RoutedEventArgs e)
        {
            MergeXYZZtoXYZ();
        }

        private void MergeXYZZtoXYZ()
        {
            IEnumerable<Fragment> listZ1 = wc.GetFragments(2);
            IEnumerable<Fragment> listZ2 = wc.GetFragments(3);

            if (listZ1 == null || listZ2 == null)
            {
                MessageBox.Show("Merge is for combining result sets," +
                                " indexes 2 and 3 must be populated.");
            }
            else
            {
                List<Fragment> listZFinal = new List<Fragment>();

                foreach (Fragment f in listZ1)
                {
                    listZFinal.Add(f);
                }

                foreach (Fragment f in listZ2)
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

                if(wc.ColumnCount > 3)
                {
                    if(Prompt.Confirm("Auto-merge results?"))
                    {
                        MergeXYZZtoXYZ();
                    }
                }
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

        /// <summary>
        /// Allows user to populate a specified field of listX with a value from
        /// the same field in listY based on a matching algorithm using
        /// the current display values of both lists. The matching is done using 
        /// a percent match, and if a less than 100% match is given,
        /// the suggested value will be stored under the keys bestMatchKey and bestMatchValue
        /// with the percent value stored in bestMatchPercent, the fragment will
        /// be flagged for review, so a visual user confirmation can be performed. If a 100% match
        /// is found, the value will automatically be set without flagging the fragment for review.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FuzzyIntersectionXYtoXpp(object sender, RoutedEventArgs e)
        {
            IEnumerable<ProcessableFragment> listX = 
                wc.GetFragments(0).ToProcessables(fuzzyIntersectionProcessedTag);

            IEnumerable<ProcessableFragment> listY = 
                wc.GetFragments(1).ToProcessables(fuzzyIntersectionProcessedTag);

            string metaKeyToSet = Prompt.Input("Key to set on confirm?");

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

                IEnumerable<ProcessableFragment> listXSegment =
                    listX.GetUnprocessedSegment(segmentSize);

                if (listX != null && listY != null)
                {                    
                    foreach (ProcessableFragment f in listXSegment)
                    {

                        string bestMatchValue = "[no match found]";
                        string bestMatchKey = "[no match found]";
                        string bestValueToSet = "[no match found]";
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
                                bestValueToSet = f2.GetMeta(metaKeyToSet);
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
                        //f.SetProcessed(fuzzyIntersectionProcessedTag);
                        f.SetProcessed();

                        if(Double.Parse(f.GetMeta("bestPercentMatch")) < 1)
                        {
                            //this is a hackish solution just for now
                            ReviewableFragment temp =
                                new ReviewableFragment(f);
                            temp.FlagForReview("concatenated", metaKeyToSet, bestValueToSet);
                            f.Merge(temp, ConflictMergeAction.SkipConflicts);
                        }
                        else if(Double.Parse(f.GetMeta("bestPercentMatch")) == 1) 
                        {
                            //no need to flag for review, perfect match, just set
                            f.SetMeta(metaKeyToSet, bestValueToSet);
                        }
                    }
                }

            } while (Prompt.Confirm("Process another segment?"));
            
            w.Receive(listX.GetProcessed().DeepCopy());
            wc.RefreshMetaKeys(0); 
        }
    }
}
