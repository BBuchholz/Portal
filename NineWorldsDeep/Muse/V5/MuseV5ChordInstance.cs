using NineWorldsDeep.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Muse.V5
{
    public class MuseV5ChordInstance : MuseV5PatternInstance
    {
        #region properties

        public string Name { get; private set; }

        #endregion

        #region creation

        public MuseV5ChordInstance(MuseV5Note rootNote, MuseV5Chord chord) 
            : base(rootNote, chord.PatternSignature)
        {
            //Name = rootNote + " " + chord.ChordName;
            Name = rootNote + chord.ChordQualitySuffix;
        }

        #endregion

        #region public interface

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
