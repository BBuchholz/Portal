using System;
using NineWorldsDeep.Tapestry;

namespace NineWorldsDeep.FragmentCloud
{
    internal class SynergyFragment : Tapestry.TapestryNode
    {
        public SynergyFragment()
            : base("Synergy")
        {
        }

        public override string GetShortName()
        {
            return "Synergy";
        }

        public override bool Parallels(Tapestry.TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            var window = new Synergy.SynergyV4MainWindow();
            window.Show();
            UI.Utils.MinimizeMainWindow();
        }
    }
}