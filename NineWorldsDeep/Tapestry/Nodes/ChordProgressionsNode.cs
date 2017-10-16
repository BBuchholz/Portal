using System;
using System.Collections.Generic;

namespace NineWorldsDeep.Tapestry.Nodes
{
    internal class ChordProgressionsNode : TapestryNode
    {
        public ChordProgressionsNode()
            : base("Muse/ChordProgressions")
        {
        }

        public override string GetShortName()
        {
            return "Progressions";
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

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override TapestryNodeType NodeType
        {
            get
            {
                return TapestryNodeType.ChordProgressions;
            }
        }
    }
}