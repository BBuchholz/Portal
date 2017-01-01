using NineWorldsDeep.Studio.Utils;
using System;

namespace NineWorldsDeep.Studio.Model
{
    internal class MajorScale : Scale
    {
        public MajorScale(string rootNoteName)
            : base(rootNoteName, "Major Scale")
        {
            int[] majorScalePositionalValues = new int[] { 0, 2, 4, 5, 7, 9, 11, 12 };
            
            foreach(int posVal in majorScalePositionalValues)
            {
                this.scaleTones.Add(this.rootNoteAbsVal + posVal);
            }
        }

        public Chord I
        {
            get
            {
                return Chord.MajorTriad(scaleTones[0]);
            }
        }

        public Chord ii
        {
            get
            {
                return Chord.MinorTriad(scaleTones[1]);
            }
        }
        public Chord iii
        {
            get
            {
                return Chord.MinorTriad(scaleTones[2]);

            }
        }
        public Chord IV
        {
            get
            {
                return Chord.MajorTriad(scaleTones[3]);
            }
        }
        public Chord V
        {
            get
            {
                return Chord.MajorTriad(scaleTones[4]);
            }
        }
        public Chord vi
        {
            get
            {
                return Chord.MinorTriad(scaleTones[5]);
            }
        }
        public Chord vii
        {
            get
            {
                return Chord.DiminishedTriad(scaleTones[6]);
            }
        }

        public override Chord ScaleDegreeToChord(string scaleDegree)
        {
            switch (scaleDegree)
            {
                case "I":

                    return I;


                case "ii":

                    return ii;

                case "iii":

                    return iii;


                case "IV":

                    return IV;

                case "V":

                    return V;

                case "vi":

                    return vi;

                case "vii":

                    return vii;

                default:

                    throw new ArgumentException(
                        "invalid scale degree for major scale: " 
                        + scaleDegree);
            }        
        }

        // MAJOR SCALE CHORDS
        // 1=Major, 2=minor, 3=minor, 4=Major, 5=Major, 6=minor, 7=dim
    }
}