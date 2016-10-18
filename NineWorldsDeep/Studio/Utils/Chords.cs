using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Tapestry.Nodes;

namespace NineWorldsDeep.Studio.Utils
{
    public class Chords
    {
        //currently a mock
        public static ChordNode Parse(string chordName)
        {

            // Dm/a (first inversion)
            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            switch (chordName)
            {
                // first inversion
                case "Dm/a":
                    
                    notes = MinorFirstInversion(Notes.ParseAbsoluteValue("D"));
                    break;
                    
                case "Am":

                    notes[9] = true;
                    notes[12] = true;
                    notes[16] = true;
                    break;

                // second inversion
                case "E/g#":

                    notes[8] = true;
                    notes[11] = true;
                    notes[16] = true;
                    break;

                default:

                    //light em all up
                    notes.SetAll(true);
                    break;
            }

            return new ChordNode(chordName, notes);
        }

        private static TwoOctaveNoteArray MinorFirstInversion(int rootNote)
        {
            //pattern is -5, 0, 3

            //test range
            if(rootNote - 5 < 0)
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
