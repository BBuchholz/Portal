using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.FragmentCloud;

namespace NineWorldsDeep.Tapestry.Fragments
{
    public class StudioScalesFragment : FragmentCloud.Fragment
    {
        public StudioScalesFragment()
            : base("Studio/Scales")
        {
        }

        public override string GetShortName()
        {
            return "Studio Scales";
        }

        public override void PerformSelectionAction()
        {
            var window = new Studio.VisualScales();
            window.Show();
        }
    }
}
