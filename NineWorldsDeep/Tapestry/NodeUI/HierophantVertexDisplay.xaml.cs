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
using NineWorldsDeep.Tapestry.Nodes;
using NineWorldsDeep.Hierophant;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for HierophantVertexDisplay.xaml
    /// </summary>
    public partial class HierophantVertexDisplay : UserControl
    {
        public HierophantVertexDisplay()
        {
            InitializeComponent();
        }

        public void Display(HierophantVertexNode vNode)
        {
            string details = "";

            if (vNode.Coupling != null)
            {
                HierophantVertex vertex = vNode.Coupling.Vertex;
                details += vertex.Details();
            }
            
            tbDetails.Text = details;
        }
    }
}
