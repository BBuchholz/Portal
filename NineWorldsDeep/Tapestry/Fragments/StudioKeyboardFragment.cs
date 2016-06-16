using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.FragmentCloud;

namespace NineWorldsDeep.Tapestry.Fragments
{
    public class StudioKeyboardFragment : FragmentCloud.Fragment
    {
        public StudioKeyboardFragment() 
            : base("Studio/Keyboard")
        {
        }

        public override string GetShortName()
        {
            return "Studio Keyboard";
        }

        public override void PerformSelectionAction()
        {
            var window = new Studio.VisualKeyboardTestHarness();
            window.Show();
        }
    }
}
