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
using System.Windows.Shapes;

namespace NineWorldsDeep.Studio
{
    /// <summary>
    /// Interaction logic for VisualKeyboardTestHarness.xaml
    /// </summary>
    public partial class VisualKeyboardTestHarness : Window
    {
        public VisualKeyboardTestHarness()
        {
            InitializeComponent();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            visualKeyboard.ToggleHighlights();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            visualKeyboard.ClearHighlights();
        }

        private void TestGetButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(visualKeyboard.Notes.ToString());
        }

        private void TestSetButton_Click(object sender, RoutedEventArgs e)
        {
            //C,D,E
            TwoOctaveNoteArray cde = new TwoOctaveNoteArray();
            cde[0] = true;
            cde[2] = true;
            cde[4] = true;

            visualKeyboard.Notes = cde;
        }

        private void NoteMetaTagButton_Click(object sender, RoutedEventArgs e)
        {
            string tag = visualKeyboard.CopyNotesMetaTagToClipboard();
            MessageBox.Show("metatag [" + tag + "] copied to clipboard");
        }
    }
}
