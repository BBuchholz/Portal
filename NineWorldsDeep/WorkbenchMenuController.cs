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

        public void Configure(WorkbenchWindow w, 
                              WorkbenchListViewsController wc)
        {
            this.w = w;
            this.wc = wc;
            w.Menu.AddMenuItem("List Operations",
                               "Fuzzy Intersection {x, y} => z",
                               FuzzyIntersectionXYtoZ);
            w.Menu.AddMenuItem("List Operations",
                               "Fuzzy Intersection {x, y} => x++",
                               FuzzyIntersectionXYtoXpp);
            w.Menu.AddMenuItem("List Operations",
                               "Send First",
                               SendFirst);
            w.Menu.AddMenuItem("List Operations",
                               "Remove Last",
                               RemoveLast);
            w.Menu.AddMenuItem("List Operations",
                               "Remove First",
                               RemoveFirst);
        }

        private void SendFirst(object sender, RoutedEventArgs e)
        {
            wc.SendFirst();
        }

        private void RemoveFirst(object sender, RoutedEventArgs e)
        {
            wc.RemoveFirst();
        }

        private void RemoveLast(object sender, RoutedEventArgs e)
        {
            wc.RemoveLast();
        }
        
        private void FuzzyIntersectionXYtoZ(object sender, RoutedEventArgs e)
        {
            IEnumerable<Fragment> listX = wc.GetFragments(0);
            IEnumerable<Fragment> listY = wc.GetFragments(1);
            List<Fragment> listZ = new List<Fragment>();
            
            if(listX != null && listY != null)
            {
                //process here and store in listZ
                foreach(Fragment f in listX)
                {
                    Fragment listZFragment = 
                        new Fragment("listX", f.DisplayValue);

                    string bestMatchValue = "[no match found]";
                    double bestPercentMatch = 
                        f.DisplayValue.PercentMatchTo(bestMatchValue);

                    foreach(Fragment f2 in listY)
                    {
                        string currentMatchValue = f2.DisplayValue;
                        double currentPercentMatch =
                            f.DisplayValue.PercentMatchTo(currentMatchValue);
                        
                        if(currentPercentMatch > bestPercentMatch)
                        {
                            bestMatchValue = currentMatchValue;
                            bestPercentMatch = currentPercentMatch;
                        }

                    }

                    listZFragment.SetMeta("bestMatchValue", bestMatchValue);
                    listZFragment.SetMeta("bestPercentMatch", 
                                           bestPercentMatch.ToString());

                    string concatenated = f.DisplayValue + 
                        " => " + bestMatchValue + " ("
                        + bestPercentMatch.ToString() + ")";

                    listZFragment.SetMeta("concatenated", concatenated);
                    listZFragment.DisplayKey = "concatenated";

                    listZ.Add(listZFragment);
                }
            }

            w.Receive(listZ); //adds result to workbench
        }

        private void FuzzyIntersectionXYtoXpp(object sender, RoutedEventArgs e)
        {
            string processedTag = "fuzzyIntersectionProcessed";

            IEnumerable<Fragment> listX = wc.GetFragments(0);
            IEnumerable<Fragment> listY = wc.GetFragments(1);
            //TODO: segment size should be stored in a modifiable configuration
            IEnumerable<Fragment> listXSegment =
                listX.GetUnprocessedSegment(10, processedTag);

            if (listX != null && listY != null)
            {
                //process here and store in listZ
                foreach (Fragment f in listXSegment)
                {
                    
                    string bestMatchValue = "[no match found]";
                    double bestPercentMatch =
                        f.DisplayValue.PercentMatchTo(bestMatchValue);

                    foreach (Fragment f2 in listY)
                    {
                        string currentMatchValue = f2.DisplayValue;
                        double currentPercentMatch =
                            f.DisplayValue.PercentMatchTo(currentMatchValue);

                        if (currentPercentMatch > bestPercentMatch)
                        {
                            bestMatchValue = currentMatchValue;
                            bestPercentMatch = currentPercentMatch;
                        }

                    }

                    f.SetMeta("bestMatchValue", bestMatchValue);
                    f.SetMeta("bestPercentMatch",
                              bestPercentMatch.ToString());

                    string concatenated = f.DisplayValue +
                        " => " + bestMatchValue + " ("
                        + bestPercentMatch.ToString() + ")";

                    f.SetMeta("concatenated", concatenated);
                    f.SetProcessed(processedTag);
                }
            }

            w.Receive(listX.GetProcessed(processedTag).DeepCopy());
            wc.RefreshMetaKeys(0); 
        }
    }
}
