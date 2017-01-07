using System;
using NineWorldsDeep.Tapestry;

namespace NineWorldsDeep.FragmentCloud
{
    internal class WareHouseFragment : Tapestry.TapestryNode
    {
        public WareHouseFragment()
            : base("WareHouse")
        {

        }

        public override string GetShortName()
        {
            return "WareHouse";
        }

        public override bool Parallels(Tapestry.TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            var window = new Warehouse.WarehouseMainWindow();
            window.Show();
            UI.Utils.MinimizeMainWindow();
        }
    }
}