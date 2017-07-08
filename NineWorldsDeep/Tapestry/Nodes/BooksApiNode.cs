using System;
using System.Collections.Generic;

namespace NineWorldsDeep.Tapestry.Nodes
{
    internal class BooksApiNode : TapestryNode
    {
        public BooksApiNode()
            : base("BooksApi")
        {
        }

        public override string GetShortName()
        {
            return "Books API";
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
                return TapestryNodeType.BooksApiMain;
            }
        }
    }
}