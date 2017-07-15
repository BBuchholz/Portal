using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    class GoogleApiNode : TapestryNode
    {
        public GoogleApiNode()
            : base("GoogleAPIs")
        {
            AddChild(new BooksApiNode());
            AddChild(new SpeechApiNode());
            AddChild(new PhotosApiNode());
            //AddChild(&c.);
        }

        public override string GetShortName()
        {
            return "Google APIs";
        }

        public override bool Parallels(Tapestry.TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            //do nothing
        }
    }
}
