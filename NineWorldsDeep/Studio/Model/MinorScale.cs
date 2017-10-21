using System;
using NineWorldsDeep.Studio.Utils;

namespace NineWorldsDeep.Studio.Model
{
    //public class MinorScale : Scale
    //{
    //    public MinorScale(string rootNoteName)
    //        : base(rootNoteName, "Minor Scale")
    //    {
    //        int[] minorScalePositionalValues = new int[] { 0, 2, 3, 5, 7, 8, 10, 12 };

    //        foreach (int posVal in minorScalePositionalValues)
    //        {
    //            this.scaleTones.Add(this.rootNoteAbsVal + posVal);
    //        }
    //    }

    //    // MINOR SCALE CHORDS
    //    // 1=minor, 2=dim, 3=major, 4=minor, 5=minor, 6=major, 7=major

    //    public Chord i
    //    {
    //        get
    //        {
    //            return Chord.MinorTriad(scaleTones[0]);
    //        }
    //    }

    //    public Chord ii
    //    {
    //        get
    //        {
    //            return Chord.DiminishedTriad(scaleTones[1]);
    //        }
    //    }

    //    public Chord III
    //    {
    //        get
    //        {
    //            return Chord.MajorTriad(scaleTones[2]);
    //        }
    //    }

    //    public Chord iv
    //    {
    //        get
    //        {
    //            return Chord.MinorTriad(scaleTones[3]);
    //        }
    //    }

    //    public Chord v
    //    {
    //        get
    //        {
    //            return Chord.MinorTriad(scaleTones[4]);
    //        }
    //    }

    //    public Chord VI
    //    {
    //        get
    //        {
    //            return Chord.MajorTriad(scaleTones[5]);
    //        }
    //    }

    //    public Chord VII
    //    {
    //        get
    //        {
    //            return Chord.MajorTriad(scaleTones[6]);
    //        }
    //    }

    //    public override Chord ScaleDegreeToChord(string scaleDegree)
    //    {
    //        switch (scaleDegree)
    //        {
    //            case "i":

    //                return i;

    //            case "ii":

    //                return ii;

    //            case "III":

    //                return III;

    //            case "iv":

    //                return iv;

    //            case "v":

    //                return v;

    //            case "VI":

    //                return VI;

    //            case "VII":

    //                return VII;

    //            default:

    //                throw new ArgumentException(
    //                    "invalid scale degree for minor scale: "
    //                    + scaleDegree);
    //        }
    //    }
    //}
}