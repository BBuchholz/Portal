using NineWorldsDeep.Core;
using NineWorldsDeep.Tapestry.Nodes;
using NineWorldsDeep.Tapestry.NodeUI;
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
    /// Interaction logic for HierophantTreeOfLife.xaml
    /// </summary>
    public partial class HierophantTreeOfLife : UserControl, ISemanticallyAddressable
    {
        private Dictionary<Shape, HierophantUiCoupling> shapesToCouplings =
            new Dictionary<Shape, HierophantUiCoupling>();
        private Dictionary<SemanticKey, HierophantUiCoupling> keysToCouplings =
            new Dictionary<SemanticKey, HierophantUiCoupling>();

        public IEnumerable<SemanticKey> SemanticKeys
        {
            get
            {
                return keysToCouplings.Keys;
            }
        }

        public HierophantTreeOfLife()
        {
            InitializeComponent();
            IndexSephiroth();
            CreateAndDrawPaths();
        }

        public void Display(ISemanticallyRenderable semMap)
        {
            semMap.Render(this);
        }
        
        private Sephirah AddSephirahCoupling(Ellipse el, string sephirahName)
        {
            return AddSephirahCoupling(new HierophantCircleElement(el), sephirahName);
        }       

        private Sephirah AddSephirahCoupling(HierophantCanvasElement canvasElement, string sephirahName)
        {
            Sephirah seph = new Sephirah(sephirahName);
            HierophantUiCoupling coupling = 
                new HierophantUiCoupling(canvasElement, seph);
            shapesToCouplings[canvasElement.ShapeId] = coupling;
            keysToCouplings[new SemanticKey(sephirahName)] = coupling;

            return seph;
        }

        private TreePath AddPathCoupling(HierophantCanvasElement canvasElement, string pathNameId)
        {
            TreePath tp = new TreePath(pathNameId);

            var coupling = new HierophantUiCoupling(canvasElement, tp);
            shapesToCouplings[canvasElement.ShapeId] = coupling;
            keysToCouplings[new SemanticKey(pathNameId)] = coupling;

            return tp;
        }

        private void IndexSephiroth()
        {
            AddSephirahCoupling(elKether, "Kether");
            AddSephirahCoupling(elChokmah, "Chokmah");
            AddSephirahCoupling(elBinah, "Binah");
            AddSephirahCoupling(elChesed, "Chesed");
            AddSephirahCoupling(elGeburah, "Geburah");
            AddSephirahCoupling(elTipareth, "Tipareth");
            AddSephirahCoupling(elNetzach, "Netzach");
            AddSephirahCoupling(elHod, "Hod");
            AddSephirahCoupling(elYesod, "Yesod");
            AddSephirahCoupling(elMalkuth, "Malkuth");
        }
        
        private void CreateAndDrawPath(Ellipse from, Ellipse to, string pathNameId)
        {
            Point fromCenter = from.GetCenter();
            Point toCenter = to.GetCenter();

            HierophantRectPath rectPath = 
                new HierophantRectPath(fromCenter, toCenter);

            TreePath path =
                AddPathCoupling(rectPath, pathNameId);

            Sephirah fromSeph = TryRetrieveSephirah(from);
            Sephirah toSeph = TryRetrieveSephirah(to);            

            if(fromSeph != null && toSeph != null)
            {
                path.Sephiroth[fromSeph.NameId] = fromSeph;
                path.Sephiroth[toSeph.NameId] = toSeph;

                canvas.Children.Add(rectPath.ShapeId);
                rectPath.ShapeId.MouseLeftButtonDown += RectPath_MouseLeftButtonDown;
            }
        }
        
        private Sephirah TryRetrieveSephirah(Shape shape)
        {
            Sephirah retrieved = null;

            if (shapesToCouplings.ContainsKey(shape))
            {
                if(shapesToCouplings[shape].Vertex is Sephirah)
                {
                    retrieved = (Sephirah)shapesToCouplings[shape].Vertex;
                }
            }

            return retrieved;
        }

        private HierophantUiCoupling TryRetrieveCoupling(Shape shape)
        {
            HierophantUiCoupling retrieved = null;

            if (shapesToCouplings.ContainsKey(shape))
            {
                retrieved = shapesToCouplings[shape];                
            }

            return retrieved;
        }
        
        private void CreateAndDrawPaths()
        {
            //three mothers
            CreateAndDrawPath(elBinah, elChokmah, "Binah-Chokmah");
            CreateAndDrawPath(elGeburah, elChesed, "Geburah-Chesed");
            CreateAndDrawPath(elHod, elNetzach, "Hod-Netzach");

            //seven doubles
            CreateAndDrawPath(elMalkuth, elYesod, "Malkuth-Yesod");
            CreateAndDrawPath(elYesod, elTipareth, "Yesod-Tipareth");
            CreateAndDrawPath(elTipareth, elKether, "Tipareth-Kether");
            CreateAndDrawPath(elGeburah, elBinah, "Geburah-Binah");
            CreateAndDrawPath(elChesed, elChokmah, "Chesed-Chokmah");
            CreateAndDrawPath(elHod, elGeburah, "Hod-Geburah");
            CreateAndDrawPath(elNetzach, elChesed, "Netzach-Chesed");

            //twelve diagonals
            CreateAndDrawPath(elKether, elBinah, "Kether-Binah");
            CreateAndDrawPath(elKether, elChokmah, "Kether-Chokmah");
            CreateAndDrawPath(elBinah, elTipareth, "Binah-Tipareth");
            CreateAndDrawPath(elChokmah, elTipareth, "Chokmah-Tipareth");

            CreateAndDrawPath(elGeburah, elTipareth, "Geburah-Tipareth");
            CreateAndDrawPath(elChesed, elTipareth, "Chesed-Tipareth");
            CreateAndDrawPath(elHod, elTipareth, "Hod-Tipareth");
            CreateAndDrawPath(elNetzach, elTipareth, "Netzach-Tipareth");

            CreateAndDrawPath(elHod, elYesod, "Hod-Yesod");
            CreateAndDrawPath(elNetzach, elYesod, "Netzach-Yesod");
            CreateAndDrawPath(elHod, elMalkuth, "Hod-Malkuth");
            CreateAndDrawPath(elNetzach, elMalkuth, "Netzach-Malkuth");
        }

        private void Sephirah_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HandleVertexClicked(sender);
            e.Handled = true;
        }

        private void RectPath_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HandleVertexClicked(sender);
            e.Handled = true;
        }

        private void HandleVertexClicked(object sender)
        {
            if(sender is Shape)
            {
                Shape shapeId = (Shape)sender;
                HierophantUiCoupling coupling =
                    TryRetrieveCoupling(shapeId);

                if(coupling != null)
                {
                    HierophantVertexNode nd =
                        new HierophantVertexNode(coupling);

                    HierophantVertexClickedEventArgs args =
                        new HierophantVertexClickedEventArgs(nd);

                    OnVertexClicked(args);
                }
            }                
        }

        public event EventHandler<HierophantVertexClickedEventArgs> VertexClicked;

        protected virtual void OnVertexClicked(HierophantVertexClickedEventArgs args)
        {
            VertexClicked?.Invoke(this, args);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ProcessNullVertexSelection();
            e.Handled = true;
        }

        private void ProcessNullVertexSelection()
        {
            HierophantVertexClickedEventArgs args =
                new HierophantVertexClickedEventArgs(new NullHierophantVertexNode());

            OnVertexClicked(args);
        }

        public void Highlight(SemanticKey semanticKey)
        {
            var shape = keysToCouplings[semanticKey].CanvasElement.ShapeId;

            shape.Fill = Brushes.Red;
        }

        public void ClearHighlight(SemanticKey semanticKey)
        {
            var shape = keysToCouplings[semanticKey].CanvasElement.ShapeId;

            shape.Fill = Brushes.White;
        }
    }

    public class HierophantVertexClickedEventArgs
    {
        public HierophantVertexClickedEventArgs(HierophantVertexNode nd)
        {
            VertexNode = nd;
        }

        public HierophantVertexNode VertexNode { get; private set; }

    }
}
