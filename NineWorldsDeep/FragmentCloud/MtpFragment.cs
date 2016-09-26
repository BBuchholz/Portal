using System;
using NineWorldsDeep.Tapestry;

namespace NineWorldsDeep.FragmentCloud
{
    internal class MtpFragment : Tapestry.TapestryNode
    {
        public MtpFragment()
            : base("Mtp")
        {
        }

        public override string GetShortName()
        {
            return "MTP";
        }

        public override bool Parallels(Tapestry.TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            var window = new Mtp.MtpMainWindow();
            window.Show();
        }
    }
}