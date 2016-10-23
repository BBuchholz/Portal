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
        //currently a mock
        public static ChordNode Parse(string chordName)
        {

            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            switch (chordName)
            {
                // first inversion
                case "Dm/a":
                    
                    // NB: next backfactoring we wanna make it look like this
                    // notes = Minor("D").FirstInversion(), Minor("D") should return a non-static chord object (this class)
                    // instanceChord.FirstInversion() should return a TwoOctaveNoteArray.

                    notes = MinorFirstInversion(Notes.ParseAbsoluteValue("D"));
                    break;
                    
                case "Am":

                    //notes[9] = true;
                    //notes[12] = true;
                    //notes[16] = true;
                    notes = Minor(Notes.ParseAbsoluteValue("A"));
                    break;

                // second inversion
                case "E/g#":

                    //notes[8] = true;
                    //notes[11] = true;
                    //notes[16] = true;
                    notes = MajorSecondInversion(Notes.ParseAbsoluteValue("E"));
                    break;

                default:

                    //light em all up
                    notes.SetAll(true);
                    break;
            }

            return new ChordNode(chordName, notes);
        }

        private static TwoOctaveNoteArray MajorSecondInversion(int rootNote)
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

        private static TwoOctaveNoteArray Minor(int rootNote)
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

        private static TwoOctaveNoteArray MinorFirstInversion(int rootNote)
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
