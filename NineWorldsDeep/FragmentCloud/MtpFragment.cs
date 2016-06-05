using System;

namespace NineWorldsDeep.FragmentCloud
{
    internal class MtpFragment : Fragment
    {
        public MtpFragment()
            : base("Mtp")
        {
        }

        public override string GetShortName()
        {
            return "MTP";
        }

        public override void PerformSelectionAction()
        {
            var window = new Mtp.MtpMainWindow();
            window.Show();
        }
    }
}