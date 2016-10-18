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

namespace NineWorldsDeep.Studio
{
    /// <summary>
    /// Interaction logic for VisualKeyboard.xaml
    /// </summary>
    public partial class VisualKeyboard : UserControl
    {
        private List<VisualKey> orderedVisualKeys =
            new List<VisualKey>();

        public VisualKeyboard()
        {
            InitializeComponent();
            Clickable = true;
            RegisterKeyDots();
        }

        public bool Clickable { get; set; }

        public TwoOctaveNoteArray Notes
        {
            get
            {
                TwoOctaveNoteArray output = new TwoOctaveNoteArray();

                for (int i = 0; i < 24; i++)
                {
                    output[i] = orderedVisualKeys[i].IsHighlighted;
                }

                return output;
            }

            set
            {
                for (int i = 0; i < 24; i++)
                {
                    orderedVisualKeys[i].IsHighlighted = value[i];
                }
            }
        }

        private void RegisterKeyDots()
        {
            orderedVisualKeys.Add(new VisualKey(C0KeyDot, C0Key, true));
            orderedVisualKeys.Add(new VisualKey(CSharp0KeyDot, CSharp0Key, true));
            orderedVisualKeys.Add(new VisualKey(D0KeyDot, D0Key, true));
            orderedVisualKeys.Add(new VisualKey(DSharp0KeyDot, DSharp0Key, true));
            orderedVisualKeys.Add(new VisualKey(E0KeyDot, E0Key, true));

            orderedVisualKeys.Add(new VisualKey(F0KeyDot, F0Key, true));
            orderedVisualKeys.Add(new VisualKey(FSharp0KeyDot, FSharp0Key, true));
            orderedVisualKeys.Add(new VisualKey(G0KeyDot, G0Key, true));
            orderedVisualKeys.Add(new VisualKey(GSharp0KeyDot, GSharp0Key, true));
            orderedVisualKeys.Add(new VisualKey(A0KeyDot, A0Key, true));
            orderedVisualKeys.Add(new VisualKey(ASharp0KeyDot, ASharp0Key, true));
            orderedVisualKeys.Add(new VisualKey(B0KeyDot, B0Key, true));

            orderedVisualKeys.Add(new VisualKey(C1KeyDot, C1Key, true));
            orderedVisualKeys.Add(new VisualKey(CSharp1KeyDot, CSharp1Key, true));
            orderedVisualKeys.Add(new VisualKey(D1KeyDot, D1Key, true));
            orderedVisualKeys.Add(new VisualKey(DSharp1KeyDot, DSharp1Key, true));
            orderedVisualKeys.Add(new VisualKey(E1KeyDot, E1Key, true));

            orderedVisualKeys.Add(new VisualKey(F1KeyDot, F1Key, true));
            orderedVisualKeys.Add(new VisualKey(FSharp1KeyDot, FSharp1Key, true));
            orderedVisualKeys.Add(new VisualKey(G1KeyDot, G1Key, true));
            orderedVisualKeys.Add(new VisualKey(GSharp1KeyDot, GSharp1Key, true));
            orderedVisualKeys.Add(new VisualKey(A1KeyDot, A1Key, true));
            orderedVisualKeys.Add(new VisualKey(ASharp1KeyDot, ASharp1Key, true));
            orderedVisualKeys.Add(new VisualKey(B1KeyDot, B1Key, true));
        }

        public string CopyNotesMetaTagToClipboard()
        {
            string notesMetaTag = "Notes: " + Notes.ToString().Replace(",", "-");
            Clipboard.SetText(notesMetaTag);
            return notesMetaTag;
        }

        public void ClearHighlights()
        {
            foreach (VisualKey vk in orderedVisualKeys)
            {
                vk.ClearHighlight();
            }
        }

        public void ToggleHighlights()
        {
            foreach (VisualKey vk in orderedVisualKeys)
            {
                vk.ToggleHighlight();
            }
        }

        private void Key_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Clickable)
            {
                foreach (VisualKey vk in orderedVisualKeys)
                {
                    if (sender == vk.KeyRectangle || sender == vk.KeyDot)
                    {
                        vk.ToggleHighlight();
                    }
                }
            }
        }
    }
}
