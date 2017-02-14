using NineWorldsDeep.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace NineWorldsDeep.Core
{
    public static class Extensions
    {
        public static void AddIdempotent<T>(this List<T> lst, T item)
        {
            bool found = false;

            foreach(T itm in lst)
            {
                if(itm is TapestryNode && item is TapestryNode)
                {
                    var a = itm as TapestryNode;
                    var b = item as TapestryNode;

                    if (a.Parallels(b))
                    {
                        found = true;
                    }
                }
                else
                {
                    found = lst.Contains(item);
                }

            }

            if (!found)
            {
                lst.Add(item);
            }
        }

        public static Point GetCenter(this Ellipse el)
        {
            double centerX =
                Canvas.GetLeft(el) + el.Width / 2;
            double centerY =
                Canvas.GetTop(el) + el.Height / 2;

            return new Point(centerX, centerY);
        }

        public static Line CreateOffsetLine(this Line originalLine, double offsetPixels)
        {
            //from: http://stackoverflow.com/questions/2825412/draw-a-parallel-line

            double x1 = originalLine.X1,
                x2 = originalLine.X2,
                y1 = originalLine.Y1,
                y2 = originalLine.Y2;

            var L = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            // This is the second line
            var x1p = x1 + offsetPixels * (y2 - y1) / L;
            var x2p = x2 + offsetPixels * (y2 - y1) / L;
            var y1p = y1 + offsetPixels * (x1 - x2) / L;
            var y2p = y2 + offsetPixels * (x1 - x2) / L;

            Line line = new Line()
            {
                X1 = x1p,
                X2 = x2p,
                Y1 = y1p,
                Y2 = y2p
            };

            return line;
        }
    }
}
