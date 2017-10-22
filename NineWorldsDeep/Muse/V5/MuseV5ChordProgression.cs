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

        public List<MuseV5ChordInstance> ToChordList(MuseV5Note rootNote)
        {
            List<MuseV5ChordInstance> chordList = new List<MuseV5ChordInstance>();
            List<string> scaleDegrees = ToScaleDegrees(ProgressionSignature);

            //determine if minor or major, to get associated notes for scale 
            //degrees
            MuseV5ScaleInstance scaleInstance =
                UtilsMuseV5.GetScaleInstanceFromScaleDegrees(scaleDegrees, rootNote);

            foreach(string scaleDegree in scaleDegrees)
            {
                int scaleDegreeAsInteger = 
                    UtilsMuseV5.RomanToInteger(scaleDegree);

                MuseV5Note scaleDegreeNote = 
                    scaleInstance.GetNoteForScaleDegree(scaleDegreeAsInteger);

                chordList.Add(ScaleDegreeToChord(scaleDegree, scaleDegreeNote));
            }

            return chordList;
        }


        #endregion

        #region private helper methods
        
        private MuseV5ChordInstance ScaleDegreeToChord(
            string scaleDegree, 
            MuseV5Note rootNote)
        {
            //TODO: these are copy-pasted from different places, so the order isn't clean, tidy these up

            //TODO: could replace all of these with a check for upper or lowercase, returning major or minor respectively
            switch (scaleDegree)
            {
                case "I":

                    return MuseV5Chord.MajorTriad(rootNote);

                case "ii":

                    return MuseV5Chord.MinorTriad(rootNote);

                case "iii":

                    return MuseV5Chord.MinorTriad(rootNote);


                case "IV":

                    return MuseV5Chord.MajorTriad(rootNote);

                case "V":

                    return MuseV5Chord.MajorTriad(rootNote);

                case "vi":

                    return MuseV5Chord.MinorTriad(rootNote);

                case "vii":

                    return MuseV5Chord.MinorTriad(rootNote);

                case "i":

                    return MuseV5Chord.MinorTriad(rootNote);
                    

                case "III":

                    return MuseV5Chord.MajorTriad(rootNote);

                case "iv":

                    return MuseV5Chord.MinorTriad(rootNote);

                case "v":

                    return MuseV5Chord.MinorTriad(rootNote);

                case "VI":

                    return MuseV5Chord.MajorTriad(rootNote);

                case "VII":

                    return MuseV5Chord.MajorTriad(rootNote);

                default:

                    throw new ArgumentException(
                        "invalid scale degree for major scale: "
                        + scaleDegree);
            }
        }

        #endregion

        #region public static methods

        public static List<string> ToScaleDegrees(string progressionSignature)
        {
            return progressionSignature
                .Split(new String[] { "-" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(sd => sd.Trim())
                .ToList();
        }

        #endregion

    }
}
