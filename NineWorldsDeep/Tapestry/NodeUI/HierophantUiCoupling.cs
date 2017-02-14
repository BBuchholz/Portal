using NineWorldsDeep.Hierophant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

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
    }
}
