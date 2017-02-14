using System;
using System.Windows.Shapes;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    public class HierophantCircleElement : HierophantCanvasElement
    {
        public HierophantCircleElement(Ellipse el)
        {
            Ellipse = el;
        }

        public override Shape ShapeId { get { return Ellipse; } }
        public Ellipse Ellipse { get; set; }
    }
}