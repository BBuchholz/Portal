using NineWorldsDeep.Archivist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    public class ArchivistSourceExcerptNode : TapestryNode
    {
        public ArchivistSourceExcerpt SourceExcerpt { get; private set; }

        public ArchivistSourceExcerptNode(ArchivistSourceExcerpt src)
            : base("ArchivistSourceExcerpt/" + src.ToString())
        {
            SourceExcerpt = src;
        }

        protected ArchivistSourceExcerptNode()
            : base("ArchivistSourceExcerpt/NULL")
        {
            SourceExcerpt = null;
        }

        public override string GetShortName()
        {
            return SourceExcerpt.ToString();
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            //do nothing
        }

        public override TapestryNodeType NodeType
        {
            get
            {
                return TapestryNodeType.ArchivistSourceExcerpt;
            }
        }
    }
}
