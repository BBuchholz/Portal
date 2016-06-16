using System;

namespace NineWorldsDeep.FragmentCloud
{
    internal class HierophantFragment : TapestryNode
    {
        public HierophantFragment()
            : base("Hierophant")
        {
        }

        public override string GetShortName()
        {
            return "Hierophant";
        }

        public override void PerformSelectionAction()
        {
            var window = new Hierophant.HierophantMainWindow();
            window.Show();
        }
    }
}