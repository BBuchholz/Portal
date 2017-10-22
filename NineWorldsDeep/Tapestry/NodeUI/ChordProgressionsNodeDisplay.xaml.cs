using NineWorldsDeep.Muse.V5;
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
        private Db.Sqlite.MuseV5SubsetDb db = 
            new Db.Sqlite.MuseV5SubsetDb();
        
        //private List<ChordNode> chords =
        //    new List<ChordNode>();

        private List<MuseV5ChordProgression> progressions =
            new List<MuseV5ChordProgression>();


        #region creation

        public ChordProgressionsNodeDisplay()
        {
            InitializeComponent();
            cmbRootNote.ItemsSource = MuseV5Note.AllNotes();
            RefreshProgressionList();
        }

        #endregion

        #region properties
       
        private MuseV5ChordProgression SelectedProgression
        {
            get
            {
                return (MuseV5ChordProgression)cmbProgression.SelectedItem;
            }
        }

        #endregion

        #region private helper methods

        private void RefreshProgressionList()
        {
            progressions = db.GetAllChordProgressions();

            cmbProgression.ItemsSource = null;
            cmbProgression.ItemsSource = progressions;
        }

        private void ProcessSelection()
        {
            MuseV5Note rootNote = (MuseV5Note)cmbRootNote.SelectedItem;
            MuseV5ChordProgression selectedProgression = SelectedProgression;

            if (selectedProgression != null && 
                rootNote != null)
            {
                txtNotes.Text = selectedProgression.TextNotes;
                txtNotes.IsEnabled = true;
                btnUpdate.IsEnabled = true;
                 
                List<MuseV5ChordInstance> chordList = 
                    selectedProgression.ToChordList(rootNote);
                    
                lvChords.ItemsSource = null;
                lvChords.ItemsSource = chordList;
                
            }
            else
            {
                lvChords.ItemsSource = null;
                txtNotes.IsEnabled = false;
                btnUpdate.IsEnabled = false;
            }
        }
        
        private void RequestDisplayForSelectedChordNode()
        {
            //need to change this to Chord, not ChordNode(wrap it after)
            MuseV5ChordInstance chord = (MuseV5ChordInstance)lvChords.SelectedItem;

            if (chord != null)
            {
                ChordNode nd = new ChordNode(chord);

                ChordClickedEventArgs args =
                    new ChordClickedEventArgs(nd);

                OnChordClicked(args);
            }
        }

        #endregion

        #region chord clicked event

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

        #endregion

        #region event handlers

        private void lvChords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RequestDisplayForSelectedChordNode();
        }

        private void cmb_ProcessSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessSelection();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            MuseV5ChordProgression prog = SelectedProgression;

            if (prog != null)
            {
                prog.TextNotes = txtNotes.Text;

                db.Save(prog);
            }

            btnUpdate.IsEnabled = false;
        }

        private void txtNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedProgression != null)
            {
                btnUpdate.IsEnabled = true;
            }
        }

        private void btnNewProgression_Click(object sender, RoutedEventArgs e)
        {
            string msg = "Enter progression signature to create a new progression. " +
                "Progression signatures are case-sensitive, duplicates will be ignored." +
                " Use suffixes '*' and '+' for diminished and augmented chords, respectively.";

            string progSignature = UI.Prompt.Input(msg);

            if (!string.IsNullOrWhiteSpace(progSignature))
            {
                //check for new
                if (!progressions.Any(p => p.ProgressionSignature.Equals(progSignature)))
                {
                    //new entry
                    msg = "Would you like to create progression: \"" + progSignature + "\", last chance to cancel";
                    if (UI.Prompt.Confirm(msg, true))
                    {
                        try
                        {
                            MuseV5ChordProgression prog = new MuseV5ChordProgression(progSignature);

                            db.Save(prog);

                            RefreshProgressionList();
                        }
                        catch(Exception ex)
                        {
                            UI.Display.Message("could not create chord progression : " + progSignature + " [error: " + ex.Message + "]");
                        }
                    }
                }
                else
                {
                    UI.Display.Message("Progression already exists");
                }
            }
        }

        private void btnGlobal_Click(object sender, RoutedEventArgs e)
        {
            Core.Configuration.GlobalNotes = !Core.Configuration.GlobalNotes;

            RequestDisplayForSelectedChordNode();
        }


        #endregion
    }
}
