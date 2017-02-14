using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    public class HierophantRectPath : HierophantCanvasElement
    {
        public override Shape ShapeId { get { return Rectangle; } }
        public int PathWidth { get; set; }
        public Line LineCenter { get; internal set; }
        public Rectangle Rectangle { get; private set; }

        public HierophantRectPath(Point centerEndpoint1, Point centerEndpoint2)
        {
            PathWidth = 20;
            LineCenter = CreateCenterLine(centerEndpoint1, centerEndpoint2);
            Rectangle = GenerateRectPath();
        }
        
        private Line CreateCenterLine(Point from, Point to)
        {

            Line line = new Line()
            {
                X1 = from.X,
                Y1 = from.Y,
                X2 = to.X,
                Y2 = to.Y
            };

            return line;
        }

        private Rectangle GenerateRectPath()
        {
            Rectangle r = null;

            if (LineCenter != null)
            {
                Point p1 = new Point(LineCenter.X1, LineCenter.Y1);
                Point p2 = new Point(LineCenter.X2, LineCenter.Y2);

                r = GenerateRectangle(p1, p2, PathWidth);
            }

            return r;
        }

        private Rectangle GenerateRectangle(Point point1, Point point2, double height)
        {
            Rectangle r = new Rectangle();

            Vector v = point2 - point1;
            double width = Math.Sqrt(v.X * v.X + v.Y * v.Y);
            double cos_w = v.X / width;
            double w_rad = Math.Acos(cos_w);
            double w = w_rad * 180.0 / Math.PI;

            if (point2.Y < point1.Y)
                w = -w;

            r.Width = width;

            r.Height = height;
            r.SetValue(Canvas.LeftProperty, point1.X);
            r.SetValue(Canvas.TopProperty, point1.Y - r.Height / 2.0);

            r.Stroke = new SolidColorBrush(Colors.Black);
            r.StrokeThickness = 2;
            r.Fill = new SolidColorBrush(Colors.White);
            r.RenderTransform = new RotateTransform(w, 0, r.Height / 2.0);

            return r;
        }
    }
}
