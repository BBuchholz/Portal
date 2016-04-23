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
        private Dictionary<Ellipse, string> sephiroth =
            new Dictionary<Ellipse, string>();

        public VisualKabbalisticTree()
        {
            InitializeComponent();
            IndexSephiroth();
            DrawPaths();
        }

        private void IndexSephiroth()
        {
            sephiroth[elKether] = "Kether";
            sephiroth[elChokmah] = "Chokmah";
            sephiroth[elBinah] = "Binah";
            sephiroth[elChesed] = "Chesed";
            sephiroth[elGeburah] = "Geburah";
            sephiroth[elTipareth] = "Tipareth";
            sephiroth[elNetzach] = "Netzach";
            sephiroth[elHod] = "Hod";
            sephiroth[elYesod] = "Yesod";
            sephiroth[elMalkuth] = "Malkuth";
        }

        private Point GetCenter(Ellipse el)
        {
            double centerX =
                Canvas.GetLeft(el) + el.Width / 2;
            double centerY =
                Canvas.GetTop(el) + el.Height / 2;

            return new Point(centerX, centerY);
        }

        private void DrawPath(Ellipse from, Ellipse to)
        {
            Point fromCenter = GetCenter(from);
            Point toCenter = GetCenter(to);

            Line line = new Line()
            {
                X1 = fromCenter.X,
                Y1 = fromCenter.Y,
                X2 = toCenter.X,
                Y2 = toCenter.Y
            };

            line.Visibility = System.Windows.Visibility.Visible;
            line.StrokeThickness = 4;
            line.Stroke = System.Windows.Media.Brushes.Black;

            Panel.SetZIndex(line, 0);

            canvas.Children.Add(line);
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
            Display.Message(HandleSephirothClick(sender));
        }

        private string HandleSephirothClick(object sender)
        {
            Ellipse clicked = (Ellipse)sender;
            
            if(clicked != null)
            {
                if (sephiroth.ContainsKey(clicked))
                {
                    return sephiroth[clicked];
                }
            }

            return "not found";
        }

        private bool HitTestForPaths()
        {
            Display.Message("path testing not implemented yet");

            // We need to store lines for paths in pairs, and use
            // this formula to determine which side of both lines
            // the click is on, if they're different sides, it's
            // between (otherwise they'd both be left or right)

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
    }
}
