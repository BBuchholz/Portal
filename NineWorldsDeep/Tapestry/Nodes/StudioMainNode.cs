using NineWorldsDeep.FragmentCloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Fragments
{
    public class StudioMainNode : TapestryNode
    {
        public StudioMainNode() 
            : base("Studio/Main")
        {
        }

        public override string GetShortName()
        {
            return "Studio Main";
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            var window = new Studio.StudioMainWindow();
            window.Show();
        }
    }
}
