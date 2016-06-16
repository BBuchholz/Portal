using System;

namespace NineWorldsDeep.FragmentCloud
{
    internal class WareHouseFragment : TapestryNode
    {
        public WareHouseFragment()
            : base("WareHouse")
        {

        }

        public override string GetShortName()
        {
            return "WareHouse";
        }

        public override void PerformSelectionAction()
        {
            var window = new Warehouse.WarehouseMainWindow();
            window.Show();
        }
    }
}