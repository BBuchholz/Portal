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
    /// Interaction logic for VisualScales.xaml
    /// </summary>
    public partial class VisualScales : Window
    {
        #region creation

        public VisualScales()
        {
            InitializeComponent();

            LoadRootNotes();
            LoadScales();

            keyboard1.ClearHighlights();
            keyboard1.Clickable = false;

            keyboard2.ClearHighlights();
        }

        #endregion

        #region private helper methods

        private void LoadScales()
        {
            cmbScale1.Items.Add(new ScalePrevious("Major"));
            cmbScale1.Items.Add(new ScalePrevious("Minor"));
        }

        private void LoadRootNotes()
        {
            cmbRoot1.Items.Add(new Note("C"));
            cmbRoot1.Items.Add(new Note("C#/Db"));
            cmbRoot1.Items.Add(new Note("D"));
            cmbRoot1.Items.Add(new Note("D#/Eb"));
            cmbRoot1.Items.Add(new Note("E"));
            cmbRoot1.Items.Add(new Note("F"));
            cmbRoot1.Items.Add(new Note("F#/Gb"));
            cmbRoot1.Items.Add(new Note("G"));
            cmbRoot1.Items.Add(new Note("G#/Ab"));
            cmbRoot1.Items.Add(new Note("A"));
            cmbRoot1.Items.Add(new Note("A#/Bb"));
            cmbRoot1.Items.Add(new Note("B"));
        }

        private void LoadScale1()
        {
            ScalePrevious scale = (ScalePrevious)cmbScale1.SelectedItem;
            Note rootNote = (Note)cmbRoot1.SelectedItem;

            if (scale != null && rootNote != null)
            {
                keyboard1.Notes = scale.PatternSignature.ToNoteArray(rootNote);
            }
        }

        #endregion

        #region event handlers
        
        private void lvCompatibleScales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbRoot1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadScale1();
        }

        private void cmbScale1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadScale1();
        }

        #endregion
    }
}
