using NineWorldsDeep.Hive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    public class HiveMigrationRootNode : TapestryNode
    {
        public HiveRoot HiveRoot { get; set; }

        public HiveMigrationRootNode(HiveRoot hr)
            : base("Hive/Migrations/" + hr.HiveRootName)
        {
            HiveRoot = hr;
        }

        public override string GetShortName()
        {
            return "Hive Migration " + HiveRoot.HiveRootName;
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            //do nothing
        }

        public override IEnumerable<TapestryNode> Children(TapestryNodeType nodeType)
        {
            //always empty
            return new List<TapestryNode>();
        }

        public override TapestryNodeType NodeType
        {
            get
            {
                return TapestryNodeType.HiveMigration;
            }
        }

        public object Destination { get; internal set; }
    }
}
