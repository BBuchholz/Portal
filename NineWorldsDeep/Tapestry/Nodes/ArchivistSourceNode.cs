using NineWorldsDeep.Archivist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    public class ArchivistSourceNode : TapestryNode
    {
        public ArchivistSource Source { get; private set; }

        public ArchivistSourceNode(ArchivistSource src)
            : base("ArchivistSource/" + src.ToString())
        {
            Source = src;
        }

        protected ArchivistSourceNode()
            : base("ArchivistSource/NULL")
        {
            Source = null;
        }

        public override string GetShortName()
        {
            return Source.ToString();
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
                return TapestryNodeType.ArchivistSource;
            }
        }
    }
}
