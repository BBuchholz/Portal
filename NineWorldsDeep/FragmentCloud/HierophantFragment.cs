using System;
using NineWorldsDeep.Tapestry;

namespace NineWorldsDeep.FragmentCloud
{
    internal class HierophantFragment : Tapestry.TapestryNode
    {
        public HierophantFragment()
            : base("Hierophant")
        {
        }

        public override string GetShortName()
        {
            return "Hierophant";
        }

        public override bool Parallels(Tapestry.TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            var window = new Hierophant.HierophantMainWindow();
            window.Show();
        }
    }
}