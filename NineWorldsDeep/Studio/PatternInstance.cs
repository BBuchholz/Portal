using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Studio
{
    /// <summary>
    /// a pattern instance is a pattern signature tied to a specific root note
    /// </summary>
    public class PatternInstance
    {                
        #region creation

        public PatternInstance(Note rootNote, PatternSignature signature)
        {
            RootNote = rootNote;
            PatternSignature = signature;
        }

        #endregion

        #region properties

        private PatternSignature PatternSignature { get; set; }

        public Note RootNote { get; private set; }

        #endregion

        #region public interface

        public TwoOctaveNoteArray ToNoteArray()
        {
            return PatternSignature.ToNoteArray(RootNote);
        }

        #endregion

    }
}
