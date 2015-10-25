using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NineWorldsDeep
{
    public class WorkbenchController
    {
        Workbench w;

        public void Configure(Workbench w)
        {
            this.w = w;
            w.Menu.AddMenuItem("List Operations",
                               "Union {x, y} => z",
                               UnionXYtoZ); //TODO: rename this (I honestly don't know what to call it just following a scent)
            w.Menu.AddMenuItem("List Operations",
                               "Remove Last",
                               RemoveLast);
            w.Menu.AddMenuItem("List Operations",
                               "Remove First",
                               RemoveFirst);
        }

        private void RemoveFirst(object sender, RoutedEventArgs e)
        {
            w.RemoveFirst();
        }

        private void RemoveLast(object sender, RoutedEventArgs e)
        {
            w.RemoveLast();
        }

        private void UnionXYtoZ(object sender, RoutedEventArgs e)
        {
            IEnumerable<Fragment> listX = w.GetFragments(0);
            IEnumerable<Fragment> listY = w.GetFragments(1);
            List<Fragment> listZ = new List<Fragment>();
            
            if(listX != null && listY != null)
            {
                //process here and store in listZ
                foreach(Fragment f in listX)
                {
                    Fragment listZFragment = 
                        new Fragment("listX", f.DisplayValue);

                    string bestMatchValue = "";
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
    }
}
