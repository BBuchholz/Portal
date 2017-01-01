using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Tapestry.Nodes;

namespace NineWorldsDeep.Studio.Utils
{
    public class Chord
    {
        public Chord(string chordName, TwoOctaveNoteArray notes)
        {
            ChordName = chordName;
            ChordNotes = notes;
        }

        public string ChordName { get; private set; }
        public TwoOctaveNoteArray ChordNotes { get; private set; }

        //currently a mock
        public static ChordNode ParseToNode(string chordName)
        {
            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            switch (chordName)
            {
                // first inversion
                case "Dm/a":
                    
                    // NB: next backfactoring we wanna make it look like this
                    // notes = Minor("D").FirstInversion(), Minor("D") should return an instance(non-static) chord object (this class)
                    // instanceChord.FirstInversion() should return a TwoOctaveNoteArray.

                    notes = MinorTriadFirstInversion(Note.ParseAbsoluteValue("D"));
                    break;
                    
                case "Am":

                    //notes[9] = true;
                    //notes[12] = true;
                    //notes[16] = true;
                    notes = MinorTriad(Note.ParseAbsoluteValue("A"));
                    break;

                // second inversion
                case "E/g#":

                    //notes[8] = true;
                    //notes[11] = true;
                    //notes[16] = true;
                    notes = MajorTriadSecondInversion(Note.ParseAbsoluteValue("E"));
                    break;

                default:

                    throw new ArgumentException(chordName + " is not a supported chord name");
            }
            
            return new ChordNode(new Chord(chordName, notes));
        }

        //public TwoOctaveNoteArray FirstInversion()
        //{
        //    bool first = true;

        //    TwoOctaveNoteArray notes = new TwoOctaveNoteArray();
        //    return notes;
        //}

        public static Chord MajorTriad(string noteName)
        {
            int rootNoteAbsVal = Note.ParseAbsoluteValue(noteName);

            string chordName = noteName.ToUpper();

            //TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            //notes[rootNoteAbsVal] = true;
            //notes[rootNoteAbsVal + 4] = true;
            //notes[rootNoteAbsVal + 7] = true;

            return new Chord(chordName, MajorTriad(rootNoteAbsVal));
        }

        public static Chord MinorTriad(string noteName)
        {
            //pattern is 0, 3, 7

            int rootNoteAbsVal = Note.ParseAbsoluteValue(noteName);

            string chordName = noteName.ToUpper() + "m";

            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[rootNoteAbsVal] = true;
            notes[rootNoteAbsVal + 3] = true;
            notes[rootNoteAbsVal + 7] = true;

            return new Chord(chordName, notes);
        }

        private static TwoOctaveNoteArray MajorTriadSecondInversion(int rootNote)
        {
            //pattern is -8, -5, 0

            //coax range
            while (rootNote - 8 < 0)
            {
                rootNote += 12;
            }

            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[rootNote - 8] = true;
            notes[rootNote - 5] = true;
            notes[rootNote] = true;

            return notes;
        }

        private static int CoaxRootNote(int rootNote)
        {
            //always shift as low as octaves will allow
            while(rootNote > 11)
            {
                rootNote -= 12;
            }

            //minmum is 0 for low C
            while (rootNote < 0)
            {
                rootNote += 12;
            }

            return rootNote;
        }

        private static TwoOctaveNoteArray MinorTriad(int rootNote)
        {
            //pattern is 0, 3, 7

            //coax range
            rootNote = CoaxRootNote(rootNote);

            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[rootNote] = true;
            notes[rootNote + 3] = true;
            notes[rootNote + 7] = true;

            return notes;
        }

        private static TwoOctaveNoteArray MajorTriad(int rootNoteAbsVal)
        {
            //pattern is 0, 4, 7

            //coax range
            rootNoteAbsVal = CoaxRootNote(rootNoteAbsVal);
            
            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[rootNoteAbsVal] = true;
            notes[rootNoteAbsVal + 4] = true;
            notes[rootNoteAbsVal + 7] = true;

            return notes;
        }

        private static TwoOctaveNoteArray MinorTriadFirstInversion(int rootNote)
        {
            //pattern is -5, 0, 3

            //coax range
            while(rootNote - 5 < 0)
            {
                rootNote += 12;
            }

            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[rootNote - 5] = true;
            notes[rootNote] = true;
            notes[rootNote + 3] = true;

            return notes;
        }
        
    }
}
