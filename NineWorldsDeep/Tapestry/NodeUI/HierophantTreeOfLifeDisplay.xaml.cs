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
using NineWorldsDeep.Hierophant;
using NineWorldsDeep.Tapestry.Nodes;
using NineWorldsDeep.Core;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for HierophantTreeOfLifeDisplay.xaml
    /// </summary>
    public partial class HierophantTreeOfLifeDisplay : UserControl
    {
        public HierophantTreeOfLifeDisplay()
        {
            InitializeComponent();
            hierophantTreeOfLifeInstance.VertexClicked += TreeOfLife_VertexClicked;
            lurianicTreeOfLifeInstance.VertexClicked += TreeOfLife_VertexClicked;
        }

        public event EventHandler<HierophantVertexClickedEventArgs> VertexClicked;

        private void TreeOfLife_VertexClicked(object sender, HierophantVertexClickedEventArgs args)
        {
            args.VertexNode.LoadLocal = chkLoadLocal.IsChecked.Value;
            OnVertexClicked(args);
        }

        protected virtual void OnVertexClicked(HierophantVertexClickedEventArgs args)
        {
            VertexClicked?.Invoke(this, args);
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            UtilsUi.ProcessExpanderState((Expander)sender);
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            UtilsUi.ProcessExpanderState((Expander)sender);
        }
        
        public void Display(HierophantVertexNode vNode)
        {
            string details = "";

            if (vNode.Coupling != null)
            {
                HierophantVertex vertex = vNode.Coupling.Vertex;
                details += vertex.Details();
            }

            tbTestText.Text = details;
        }

        private void btnExample_Click(object sender, RoutedEventArgs e)
        {
            SemanticRenderMap map = new SemanticRenderMap();

            map.Select(new SemanticKey("Binah-Chokmah")); //should display on both
            map.Select(new SemanticKey("Chesed")); //should display on both
            map.Select(new SemanticKey("Hod-Malkuth")); //should only display on GD tree
            map.Select(new SemanticKey("Chokmah-Geburah")); //should only display on Lurianic tree

            hierophantTreeOfLifeInstance.Display(map);
            lurianicTreeOfLifeInstance.Display(map);
        }
    }
}
