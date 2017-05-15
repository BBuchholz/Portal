using NineWorldsDeep.Mnemosyne.V5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    public class MediaTagNode : TapestryNode
    {
        public MediaTag MediaTag { get; private set; }

        public MediaTagNode(MediaTag tag)
            : base("MediaTag/" + tag.MediaTagValue)
        {
            MediaTag = tag;
        }

        protected MediaTagNode()
            : base("MediaTag/NULL")
        {
            MediaTag = null;
        }

        public override string GetShortName()
        {
            return MediaTag.MediaTagValue;
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
                return TapestryNodeType.MediaTag;
            }
        }
    }
}
