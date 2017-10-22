using NineWorldsDeep.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Muse.V5
{
    public class MuseV5Chord
    {
        private const string MAJOR_TRIAD_NAME = "major triad";
        private const string MINOR_TRIAD_NAME = "minor triad";
        private const string DIMINISHED_TRIAD_NAME = "diminished triad";
        private const string AUGMENTED_TRIAD_NAME = "augmented triad";
        #region creation

        public MuseV5Chord(string name)
        {
            ChordName = name;
            PopulateSignatureAndQualityForChordName(ChordName);
        }

        #endregion

        #region properties

        public string ChordName { get; private set; }

        public MuseV5PatternSignature PatternSignature { get; private set; }

        public string ChordQualitySuffix { get; private set; }

        #endregion

        #region private helper methods
        
        private void PopulateSignatureAndQualityForChordName(string chordName)
        {
            chordName = chordName.Trim().ToLower();

            switch (chordName)
            {
                case MAJOR_TRIAD_NAME:

                    ChordQualitySuffix = "";
                    PatternSignature = new MuseV5PatternSignature("0,4,7");
                    break;

                case MINOR_TRIAD_NAME:

                    ChordQualitySuffix = "m";
                    PatternSignature = new MuseV5PatternSignature("0,3,7");
                    break;

                case DIMINISHED_TRIAD_NAME:

                    ChordQualitySuffix = "*";
                    PatternSignature = new MuseV5PatternSignature("0,3,6");
                    break;

                case AUGMENTED_TRIAD_NAME:

                    ChordQualitySuffix = "+";
                    PatternSignature = new MuseV5PatternSignature("0,4,8");
                    break;

                default:
                    throw new Exception("could not parse unrecognized chord name: " + chordName);
            }
        }

        #endregion

        #region public static methods

        public static MuseV5ChordInstance MajorTriad(MuseV5Note rootNote)
        {
            return new MuseV5Chord(MAJOR_TRIAD_NAME).ToInstance(rootNote);
        }

        public static MuseV5ChordInstance MinorTriad(MuseV5Note rootNote)
        {
            return new MuseV5Chord(MINOR_TRIAD_NAME).ToInstance(rootNote);
        }

        public static MuseV5ChordInstance DiminishedTriad(MuseV5Note rootNote)
        {
            return new MuseV5Chord(DIMINISHED_TRIAD_NAME).ToInstance(rootNote);
        }

        public static MuseV5ChordInstance AugmentedTriad(MuseV5Note rootNote)
        {
            return new MuseV5Chord(AUGMENTED_TRIAD_NAME).ToInstance(rootNote);
        }

        #endregion

        #region public interface

        public override string ToString()
        {
            return ChordName;
        }

        public MuseV5ChordInstance ToInstance(MuseV5Note note)
        {
            return new MuseV5ChordInstance(note, this);
        }


        #endregion
    }
}
