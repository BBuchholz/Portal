using NineWorldsDeep.Studio;
using NineWorldsDeep.Tapestry.Nodes;
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

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for ChordDisplay.xaml
    /// </summary>
    public partial class ChordDisplay : UserControl
    {
        private ChordNode chordNode;

        public ChordDisplay()
        {
            InitializeComponent();
            visualKeyboard.Clickable = false;
        }

        public void Display(ChordNode nd)
        {
            chordNode = nd;

            visualKeyboard.Notes = chordNode.Chord.ChordNotes;            
        }
    }
}
