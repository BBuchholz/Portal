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

                    notes = MinorTwoOctaveFirstInversion(Note.ParseAbsoluteValue("D"));
                    break;
                    
                case "Am":

                    //notes[9] = true;
                    //notes[12] = true;
                    //notes[16] = true;
                    notes = MinorTwoOctave(Note.ParseAbsoluteValue("A"));
                    break;

                // second inversion
                case "E/g#":

                    //notes[8] = true;
                    //notes[11] = true;
                    //notes[16] = true;
                    notes = MajorTwoOctaveSecondInversion(Note.ParseAbsoluteValue("E"));
                    break;

                default:

                    //light em all up
                    notes.SetAll(true);
                    break;
            }

            return new ChordNode(chordName, notes);
        }

        public TwoOctaveNoteArray FirstInversion()
        {
            bool first = true;

            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();
            return notes;
        }

        public static Chord Major(string noteName)
        {
            int rootNoteAbsVal = Note.ParseAbsoluteValue(noteName);

            string chordName = noteName.ToUpper();

            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[rootNoteAbsVal] = true;
            notes[rootNoteAbsVal + 4] = true;
            notes[rootNoteAbsVal + 7] = true;

            return new Chord(chordName, notes);
        }

        public static Chord Minor(string noteName)
        {
            int rootNoteAbsVal = Note.ParseAbsoluteValue(noteName);

            string chordName = noteName.ToUpper() + "m";

            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[rootNoteAbsVal] = true;
            notes[rootNoteAbsVal + 3] = true;
            notes[rootNoteAbsVal + 7] = true;

            return new Chord(chordName, notes);
        }

        private static TwoOctaveNoteArray MajorTwoOctaveSecondInversion(int rootNote)
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

        private static TwoOctaveNoteArray MinorTwoOctave(int rootNote)
        {
            //pattern is 0, 3, 7

            //coax range
            while(rootNote < 0)
            {
                rootNote += 12;
            }

            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[rootNote] = true;
            notes[rootNote + 3] = true;
            notes[rootNote + 7] = true;

            return notes;
        }

        private static TwoOctaveNoteArray MinorTwoOctaveFirstInversion(int rootNote)
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
