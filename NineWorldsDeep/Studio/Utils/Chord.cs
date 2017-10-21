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
        private TwoOctaveNoteArray _ChordNotes;

        public Chord(string chordName, TwoOctaveNoteArray notes)
        {
            ChordName = chordName;
            _ChordNotes = notes;
        }

        public string ChordName { get; private set; }

        public TwoOctaveNoteArray ChordNotes
        {
            get
            {
                if (Core.Configuration.GlobalNotes)
                {
                    return TwoOctaveNoteArray.Global(_ChordNotes);
                }
                else
                {
                    return _ChordNotes;
                }
            }

            private set
            {
                _ChordNotes = value;
            }
        }

    }
}
