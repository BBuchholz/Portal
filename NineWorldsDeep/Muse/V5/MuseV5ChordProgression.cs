using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Muse.V5
{
    public class MuseV5ChordProgression
    {
        #region creation
        
        public MuseV5ChordProgression(string progressionSignature)
        {
            ProgressionSignature = progressionSignature;
        }
        
        #endregion

        #region properties
        
        public int ChordProgressionId { get; set; }
        public string ProgressionSignature { get; private set; }
        public string TextNotes { get; set; }

        #endregion

        #region public interface

        public override string ToString()
        {
            return ProgressionSignature;
        }

        #endregion

    }
}
