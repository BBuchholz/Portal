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
            cmbKey.ItemsSource = MuseV5Note.AllNoteNames();
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
            //string key = (string)cmbKey.SelectedItem;
            //MuseV5ChordProgression prog = SelectedProgression;

            //if(prog != null)
            //{
            //    txtNotes.Text = prog.Notes;
            //    txtNotes.IsEnabled = true;
            //    btnUpdate.IsEnabled = true;

            //    if (key != null)
            //    {
            //        List<ChordNode> chords = new List<ChordNode>();

            //        List<MuseV5Chord> chordList = prog.ToChordList(key);

            //        foreach (MuseV5Chord chord in chordList)
            //        {
            //            chords.Add(new ChordNode(chord));
            //        }

            //        lvChords.ItemsSource = null;
            //        lvChords.ItemsSource = chords;
            //    }
            //}
            //else
            //{
            //    txtNotes.IsEnabled = false;
            //    btnUpdate.IsEnabled = false;
            //}            
        }
        

        private void RequestDisplayForSelectedChordNode()
        {
            //ChordNode nd = (ChordNode)lvChords.SelectedItem;

            //if (nd == null)
            //{
            //    nd = new ChordNode(new MuseV5Chord("empty chord", new TwoOctaveNoteArray()));
            //}

            //ChordClickedEventArgs args =
            //    new ChordClickedEventArgs(nd);

            //OnChordClicked(args);            
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
            //RequestDisplayForSelectedChordNode();
        }

        private void cmb_ProcessSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ProcessSelection();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //MuseV5ChordProgression prog = SelectedProgression;

            //if (prog != null)
            //{
            //    prog.Notes = txtNotes.Text;

            //    db.Save(prog);
            //}

            //btnUpdate.IsEnabled = false;
        }

        private void txtNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (SelectedProgression != null)
            //{
            //    btnUpdate.IsEnabled = true;
            //}
        }

        private void btnNewProgression_Click(object sender, RoutedEventArgs e)
        {
            //string msg = "Enter progression signature to create a new progression. " +
            //    "Progression signatures are case-sensitive, duplicates will be ignored";

            //string progSignature = UI.Prompt.Input(msg);

            //if (!string.IsNullOrWhiteSpace(progSignature))
            //{
            //    //check for new
            //    if (!progressions.Any(p => p.Signature.Equals(progSignature)))
            //    {
            //        //new entry
            //        msg = "Would you like to create progression: \"" + progSignature + "\", last chance to cancel";
            //        if (UI.Prompt.Confirm(msg, true))
            //        {
            //            if (MuseV5ChordProgression.IsValidSignature(progSignature))
            //            {
            //                MuseV5ChordProgression prog = new MuseV5ChordProgression(progSignature);

            //                db.Save(prog);

            //                RefreshProgressionList();
            //            }
            //            else
            //            {
            //                UI.Display.Message("invalid chord progression signature: " + progSignature);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        UI.Display.Message("Progression already exists");
            //    }
            //}
        }

        private void btnGlobal_Click(object sender, RoutedEventArgs e)
        {
            //Core.Configuration.GlobalNotes = !Core.Configuration.GlobalNotes;

            //RequestDisplayForSelectedChordNode();
        }


        #endregion
    }
}
