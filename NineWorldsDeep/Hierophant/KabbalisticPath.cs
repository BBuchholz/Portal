using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NineWorldsDeep.Hierophant
{
    public class KabbalisticPath
    {
        private Dictionary<string, Sephirah> _sephiroth =
            new Dictionary<string, Sephirah>();

        public string Description
        {
            get
            {
                return "Path connecting " +
                    string.Join(", ", _sephiroth.Keys.ToList());
            }
        }

        [Obsolete("use DrawPath()")]
        public Line LineAbove { get; internal set; }
        [Obsolete("use DrawPath()")]
        public Line LineBelow { get; internal set; }

        public Line LineCenter { get; internal set; }

        public void DrawPath(Canvas canvas, double pathWidth)
        {
            if (LineCenter != null)
            {
                Point p1 = new Point(LineCenter.X1, LineCenter.Y1);
                Point p2 = new Point(LineCenter.X2, LineCenter.Y2);

                DrawRect(canvas, p1, p2, pathWidth);
            }
        }

        private void DrawRect(Canvas canvas, Point point1, Point point2, double height)
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
            
            canvas.Children.Add(r);
        }
        
        public Dictionary<string, Sephirah> Sephiroth
        {
            get { return _sephiroth; }
        }
    }
}