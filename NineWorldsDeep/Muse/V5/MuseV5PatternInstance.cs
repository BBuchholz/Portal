using NineWorldsDeep.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Muse.V5
{
    public class MuseV5PatternInstance
    {
        #region properties

        public MuseV5PatternSignature PatternSignature { get; private set; }
        public MuseV5Note RootNote { get; private set; }

        #endregion

        #region creation

        public MuseV5PatternInstance(MuseV5Note rootNote, 
            MuseV5PatternSignature patternSignature)
        {
            RootNote = rootNote;
            PatternSignature = patternSignature;
        }

        #endregion

        #region public interface

        public TwoOctaveNoteArray ToNoteArray()
        {
            TwoOctaveNoteArray twoOctaveNoteArray = new TwoOctaveNoteArray();

            foreach (int posVal in PatternSignature.PositionalValues)
            {
                twoOctaveNoteArray[RootNote.PositionalValue + posVal] = true;
            }

            return twoOctaveNoteArray;
        }

        #endregion
    }
}
