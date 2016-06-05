using System;

namespace NineWorldsDeep.FragmentCloud
{
    internal class StudioFragment : Fragment
    {
        public StudioFragment()
            : base("Studio")
        {
        }

        public override string GetShortName()
        {
            return "Studio";
        }

        public override void PerformSelectionAction()
        {
            var window = new Studio.StudioMainWindow();
            window.Show();
        }
    }
}