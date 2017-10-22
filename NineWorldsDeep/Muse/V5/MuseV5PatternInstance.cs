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

        public bool Contains(TwoOctaveNoteArray noteArray)
        {
            bool doesContainAllNotes = true;
            var theseNotes = ToNoteArray();

            foreach(string noteName in noteArray.ToStringList())
            {
                if (!theseNotes.Contains(noteName))
                {
                    doesContainAllNotes = false;
                }
            }

            return doesContainAllNotes;
        }

        public TwoOctaveNoteArray ToNoteArray()
        {
            TwoOctaveNoteArray twoOctaveNoteArray = new TwoOctaveNoteArray();

            foreach (int posVal in PatternSignature.PositionalValues)
            {
                twoOctaveNoteArray[RootNote.PositionalValue + posVal] = true;
            }

            return twoOctaveNoteArray;
        }
        
        /// <summary>
        /// returns null if position is out of range
        /// </summary>
        /// <param name="zeroBasedPositionIndex"></param>
        /// <returns></returns>
        public MuseV5Note GetNoteAtPosition(int zeroBasedPositionIndex)
        {
            MuseV5Note newNote = null;

            if(zeroBasedPositionIndex < 
                PatternSignature.PositionalValues.Count())
            {
                int posVal = 
                    RootNote.PositionalValue + 
                    PatternSignature.PositionalValues[zeroBasedPositionIndex];

                newNote = new MuseV5Note(posVal);
            }

            return newNote;
        }

        #endregion
    }
}
