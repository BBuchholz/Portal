using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.FragmentCloud;

namespace NineWorldsDeep.Tapestry.Fragments
{
    public class StudioKeyboardNode : TapestryNode
    {
        public StudioKeyboardNode() 
            : base("Studio/Keyboard")
        {
        }

        public override string GetShortName()
        {
            return "Studio Keyboard";
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            var window = new Studio.VisualKeyboardTestHarness();
            window.Show();
        }
    }
}
