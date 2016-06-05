using NineWorldsDeep.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NineWorldsDeep.Hierophant
{
    /// <summary>
    /// Interaction logic for VisualKabbalisticTree.xaml
    /// </summary>
    public partial class VisualKabbalisticTree : UserControl
    {
        private Dictionary<Ellipse, Sephirah> sephiroth =
            new Dictionary<Ellipse, Sephirah>();
        private List<KabbalisticPath> paths =
            new List<KabbalisticPath>();

        public VisualKabbalisticTree()
        {
            InitializeComponent();
            IndexSephiroth();
            DrawPaths();
        }

        private void IndexSephiroth()
        {
            sephiroth[elKether] = new Sephirah("Kether");
            sephiroth[elChokmah] = new Sephirah("Chokmah");
            sephiroth[elBinah] = new Sephirah("Binah");
            sephiroth[elChesed] = new Sephirah("Chesed");
            sephiroth[elGeburah] = new Sephirah("Geburah");
            sephiroth[elTipareth] = new Sephirah("Tipareth");
            sephiroth[elNetzach] = new Sephirah("Netzach");
            sephiroth[elHod] = new Sephirah("Hod");
            sephiroth[elYesod] = new Sephirah("Yesod");
            sephiroth[elMalkuth] = new Sephirah("Malkuth");
        }

        private Point GetCenter(Ellipse el)
        {
            double centerX =
                Canvas.GetLeft(el) + el.Width / 2;
            double centerY =
                Canvas.GetTop(el) + el.Height / 2;

            return new Point(centerX, centerY);
        }

        private void StyleLine(Line line, Visibility visibility)
        {
            line.Visibility = visibility;
            line.StrokeThickness = 3;
            line.Stroke = System.Windows.Media.Brushes.Black;

            Panel.SetZIndex(line, 0);
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

            StyleLine(line, Visibility.Hidden);

            return line;
        }

        private void DrawPath(Ellipse from, Ellipse to)
        {
            Point fromCenter = GetCenter(from);
            Point toCenter = GetCenter(to);
            
            Line line = CreateCenterLine(fromCenter, toCenter);
            Line lineAbove = CreateOffsetLine(line, 10.0);
            Line lineBelow = CreateOffsetLine(line, -10.0);

            KabbalisticPath path =
                new KabbalisticPath()
                {
                    LineCenter = line,
                    LineAbove = lineAbove,
                    LineBelow = lineBelow
                };

            Sephirah fromSeph = sephiroth[from];
            Sephirah toSeph = sephiroth[to];

            path.Sephiroth[fromSeph.Name] = fromSeph;
            path.Sephiroth[toSeph.Name] = toSeph;

            paths.Add(path);

            canvas.Children.Add(line);
            canvas.Children.Add(lineAbove);
            canvas.Children.Add(lineBelow);
        }

        private Line CreateOffsetLine(Line originalLine, double offsetPixels)
        {
            //TODO: LICENSE NOTES
            //from: http://stackoverflow.com/questions/2825412/draw-a-parallel-line

            double x1 = originalLine.X1, 
                x2 = originalLine.X2, 
                y1 = originalLine.Y1, 
                y2 = originalLine.Y2;

            var L = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            //var offsetPixels = 10.0;

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

            StyleLine(line, Visibility.Visible);

            return line;
        }

        private void DrawPaths()
        {
            //three mothers
            //b-ch
            DrawPath(elBinah, elChokmah);
            //g-ch
            DrawPath(elGeburah, elChesed);
            //h-n
            DrawPath(elHod, elNetzach);

            //seven doubles
            //m-y
            DrawPath(elMalkuth, elYesod);
            //y-t
            DrawPath(elYesod, elTipareth);
            //t-k
            DrawPath(elTipareth, elKether);
            //g-b
            DrawPath(elGeburah, elBinah);
            //che-cho
            DrawPath(elChesed, elChokmah);
            //h-g
            DrawPath(elHod, elGeburah);
            //n-ch
            DrawPath(elNetzach, elChesed);

            //twelve diagonals
            //k-b
            DrawPath(elKether, elBinah);
            //k-ch
            DrawPath(elKether, elChokmah);
            //b-t
            DrawPath(elBinah, elTipareth);
            //ch-t
            DrawPath(elChokmah, elTipareth);

            //g-t
            DrawPath(elGeburah, elTipareth);
            //ch-t
            DrawPath(elChesed, elTipareth);
            //h-t
            DrawPath(elHod, elTipareth);
            //n-t
            DrawPath(elNetzach, elTipareth);

            //h-y
            DrawPath(elHod, elYesod);
            //n-y
            DrawPath(elNetzach, elYesod);
            //h-m
            DrawPath(elHod, elMalkuth);
            //n-m
            DrawPath(elNetzach, elMalkuth);

        }

        private void Sephirah_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HandleSephirothClick(sender);
            e.Handled = true; 
        }

        public event EventHandler SephirahClicked;

        private void HandleSephirothClick(object sender)
        {
            Ellipse clicked = (Ellipse)sender;
            
            if(clicked != null)
            {
                if (sephiroth.ContainsKey(clicked))
                {
                    if(SephirahClicked != null)
                    {
                        SephirahClicked(sephiroth[clicked], new EventArgs());
                    }
                }
            }
        }

        public event EventHandler PathClicked;

        private bool HandlePathClick()
        {
            //TODO: replace with raising event (see HandleSephirothClick())
            Display.Message("path testing not implemented yet");

            // We need to store lines for paths in pairs, and use
            // this formula to determine which side of both lines
            // the click is on, if they're different sides, it's
            // between (otherwise they'd both be left or right)
            //TODO: LICENSE NOTES
            // from: http://stackoverflow.com/questions/1560492/how-to-tell-whether-a-point-is-to-the-right-or-left-side-of-a-line
            // (its a downvoted answer, but only because it is a copy of the accepted 
            //  answer, I copied it because the explanation is more clear to me)
            //
            // Assuming the points are (Ax, Ay) (Bx, By) and(Cx, Cy), you need to compute:
            //
            // (Bx - Ax) * (Cy - Ay) - (By - Ay) * (Cx - Ax)
            //
            // This will equal zero if the point C is on the line formed by points A and B, 
            // and will have a different sign depending on the side.Which side this is 
            // depends on the orientation of your(x, y) coordinates, but you can plug test
            // values for A, B and C into this formula to determine whether negative
            // values are to the left or to the right.

            return false;
        }

        private int clickCountForTesting = 0;

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Display.Message(paths[clickCountForTesting].Description);
            clickCountForTesting++;

            if(clickCountForTesting > 21)
            {
                clickCountForTesting = 0;
            }
        }
    }
}
