using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Studio.Utils;

namespace NineWorldsDeep.Studio.Model
{
    public abstract class Scale
    {
        protected int rootNoteAbsVal;
        protected string fullScaleName;
        protected List<int> scaleTones;

        public Scale(string rootNoteName, string scaleName)
        {
            this.rootNoteAbsVal = Note.ParseAbsoluteValue(rootNoteName);
            this.fullScaleName = rootNoteName + " " + scaleName;
            this.scaleTones = new List<int>();
        }

        public static Scale Minor(string rootNoteName)
        {
            return new MinorScale(rootNoteName);
        }

        public static Scale Major(string rootNoteName)
        {
            return new MajorScale(rootNoteName);
        }

        public abstract Chord ScaleDegreeToChord(string scaleDegree);
    }
}
