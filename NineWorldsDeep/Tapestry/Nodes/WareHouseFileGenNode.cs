using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Fragments
{
    class WareHouseFileGenNode : FragmentCloud.TapestryNode
    {
        public WareHouseFileGenNode()
            : base("WareHouse/FileGen")
        {

        }

        public override string GetShortName()
        {
            throw "FileGen";
        }

        public override void PerformSelectionAction()
        {
            var window = new Warehouse.FileGenWindow();
            window.Show();
        }
    }
}
