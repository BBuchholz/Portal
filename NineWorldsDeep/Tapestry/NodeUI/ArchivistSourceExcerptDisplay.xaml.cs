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

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for ArchivistSourceExcerptDisplay.xaml
    /// </summary>
    public partial class ArchivistSourceExcerptDisplay : UserControl
    {
        private ArchivistSourceExcerptNode sourceExcerptNode;

        public ArchivistSourceExcerptDisplay()
        {
            InitializeComponent();
        }

        internal void Display(ArchivistSourceExcerptNode aseNode)
        {
            this.sourceExcerptNode = aseNode;
            UI.Display.Message(aseNode.SourceExcerpt.ExcerptValue);
        }
    }
}
