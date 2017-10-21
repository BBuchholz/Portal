using NineWorldsDeep.Hierophant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    public class HierophantUiCoupling
    {
        public HierophantUiCoupling(HierophantCanvasElement canvasElement, HierophantVertex vertex)
        {
            CanvasElement = canvasElement;
            Vertex = vertex;
        }

        public HierophantCanvasElement CanvasElement { get; set; }
        public HierophantVertex Vertex { get; set; }
        public bool Highlighted { get; private set; }

        public void Highlight(SolidColorBrush highlightColor)
        {
            var shape = CanvasElement.ShapeId;
            shape.Fill = Brushes.Red;
            Highlighted = true;
        }

        public void ClearHighlight(SolidColorBrush clearColor)
        {
            var shape = CanvasElement.ShapeId;
            shape.Fill = Brushes.White;
            Highlighted = false;
        }
    }
}
