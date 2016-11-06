using NineWorldsDeep.Studio;
using NineWorldsDeep.Studio.Utils;
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
    /// Interaction logic for ChordProgressionsNode.xaml
    /// </summary>
    public partial class ChordProgressionsNodeDisplay : UserControl
    {
        private List<ChordNode> chords =
            new List<ChordNode>();

        public ChordProgressionsNodeDisplay()
        {
            InitializeComponent();
            RunMock();
        }

        private void RunMock()
        {
            chords.Add(Chord.ParseToNode("Am"));
            chords.Add(Chord.ParseToNode("Dm/a"));
            chords.Add(Chord.ParseToNode("E/g#"));
            chords.Add(Chord.ParseToNode("Dm/a"));

            lvChords.ItemsSource = chords;
        }

        private void lvChords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChordNode nd = (ChordNode)lvChords.SelectedItem;

            if(nd != null)
            {
                ChordClickedEventArgs args =
                    new ChordClickedEventArgs(nd);

                OnChordClicked(args);
            }
        }

        protected virtual void OnChordClicked(ChordClickedEventArgs args)
        {
            ChordClicked?.Invoke(this, args);
        }

        public event EventHandler<ChordClickedEventArgs> ChordClicked;

        public class ChordClickedEventArgs
        {
            public ChordClickedEventArgs(ChordNode nd)
            {
                ChordNode = nd;
            }

            public ChordNode ChordNode { get; private set; }
        }
    }
}
