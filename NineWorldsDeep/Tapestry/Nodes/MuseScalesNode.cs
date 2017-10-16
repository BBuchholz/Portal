using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.FragmentCloud;

namespace NineWorldsDeep.Tapestry.Fragments
{
    public class MuseScalesNode : TapestryNode
    {
        public MuseScalesNode()
            : base("Muse/Scales")
        {
        }

        public override string GetShortName()
        {
            return "Scales";
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            var window = new Studio.VisualScales();
            window.Show();
        }
    }
}
