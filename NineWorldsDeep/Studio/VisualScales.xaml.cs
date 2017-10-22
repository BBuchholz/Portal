using NineWorldsDeep.Muse.V5;
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

            keyboard2.KeyboardClicked += keyboard2_KeyboardClicked;
        }

        #endregion

        #region private helper methods

        private void LoadScales()
        {
            cmbScale1.Items.Add(new MuseV5Scale("Major"));
            cmbScale1.Items.Add(new MuseV5Scale("Minor"));
        }

        private void LoadRootNotes()
        {
            cmbRoot1.Items.Add(new MuseV5Note("C"));
            cmbRoot1.Items.Add(new MuseV5Note("C#/Db"));
            cmbRoot1.Items.Add(new MuseV5Note("D"));
            cmbRoot1.Items.Add(new MuseV5Note("D#/Eb"));
            cmbRoot1.Items.Add(new MuseV5Note("E"));
            cmbRoot1.Items.Add(new MuseV5Note("F"));
            cmbRoot1.Items.Add(new MuseV5Note("F#/Gb"));
            cmbRoot1.Items.Add(new MuseV5Note("G"));
            cmbRoot1.Items.Add(new MuseV5Note("G#/Ab"));
            cmbRoot1.Items.Add(new MuseV5Note("A"));
            cmbRoot1.Items.Add(new MuseV5Note("A#/Bb"));
            cmbRoot1.Items.Add(new MuseV5Note("B"));
        }

        #endregion

        #region private helper methods
        
        private void LoadScaleInstanceToKeyboardOne(MuseV5ScaleInstance scaleInstance)
        {
            if (scaleInstance != null)
            {
                keyboard1.Notes = scaleInstance.ToNoteArray();
            }
        }

        private void LoadScaleInstanceFromRootAndScaleSelection()
        {
            MuseV5Note note = (MuseV5Note)cmbRoot1.SelectedItem;
            MuseV5Scale scale = (MuseV5Scale)cmbScale1.SelectedItem;

            if(note != null && scale != null)
            {
                keyboard2.ClearHighlights();
                lvCompatibleScales.ItemsSource = null;
                LoadScaleInstanceToKeyboardOne(scale.ToInstance(note));
            }
        }

        private void PopulateCompatibleScales(TwoOctaveNoteArray selectedNotes)
        {            
            lvCompatibleScales.ItemsSource = null;

            List<MuseV5ScaleInstance> compatibleScales =
                UtilsMuseV5.GetCompatibleScaleInstances(selectedNotes);

            lvCompatibleScales.ItemsSource = compatibleScales;
        }

        #endregion

        #region event handlers

        private void lvCompatibleScales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MuseV5ScaleInstance scaleInstance = (MuseV5ScaleInstance)lvCompatibleScales.SelectedItem;

            if (scaleInstance != null) {

                cmbRoot1.SelectedItem = null;
                cmbScale1.SelectedItem = null;

                LoadScaleInstanceToKeyboardOne(scaleInstance);
            }
        }

        private void cmbRoot1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadScaleInstanceFromRootAndScaleSelection();
        }

        private void cmbScale1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadScaleInstanceFromRootAndScaleSelection();
        }

        private void keyboard2_KeyboardClicked(object sender, VisualKeyboard.KeyboardClickedEventArgs e)
        {
            PopulateCompatibleScales(e.SelectedNotes);
        }


        #endregion
    }
}
