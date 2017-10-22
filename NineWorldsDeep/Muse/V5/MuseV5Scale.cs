using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Muse.V5
{
    public class MuseV5Scale
    {
        private const string MAJOR_SCALE_NAME = "major";
        private const string MINOR_SCALE_NAME = "minor";
        #region creation

        public MuseV5Scale(string name)
        {
            ScaleName = name;
            PatternSignature = ParseNameToSignature(ScaleName);
        }

        #endregion

        #region properties

        public string ScaleName { get; private set; }

        public MuseV5PatternSignature PatternSignature { get; private set; }

        #endregion

        #region public static methods

        public static MuseV5PatternSignature ParseNameToSignature(string scaleName)
        {
            scaleName = scaleName.Trim().ToLower();

            switch (scaleName)
            {
                case MAJOR_SCALE_NAME:
                    return new MuseV5PatternSignature("0,2,4,5,7,9,11,12");
                case MINOR_SCALE_NAME:
                    return new MuseV5PatternSignature("0,2,3,5,7,8,10,12");
                default:
                    throw new Exception("could not parse unrecognized scale name: " + scaleName);
            }
        }

        public static MuseV5ScaleInstance MajorScale(MuseV5Note rootNote)
        {
            return new MuseV5Scale(MAJOR_SCALE_NAME).ToInstance(rootNote);
        }

        public static MuseV5ScaleInstance MinorScale(MuseV5Note rootNote)
        {
            return new MuseV5Scale(MINOR_SCALE_NAME).ToInstance(rootNote);
        }

        #endregion

        #region public interface

        public override string ToString()
        {
            return ScaleName;
        }

        public MuseV5ScaleInstance ToInstance(MuseV5Note note)
        {
            return new MuseV5ScaleInstance(note, this);
        }

        #endregion
    }
}
