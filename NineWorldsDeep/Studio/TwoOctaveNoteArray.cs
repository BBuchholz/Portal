using System.Collections.Generic;

namespace NineWorldsDeep.Studio
{
    public class TwoOctaveNoteArray
    {
        private bool[] notes = new bool[24];

        public IEnumerable<bool> Notes
        {
            get { return notes; }
        }

        public bool this[int i]
        {
            get
            {
                return notes[i];
            }

            set
            {
                notes[i] = value;
            }
        }

        public void SetAll(bool val)
        {
            for(int i = 0; i < notes.Length; i++)
            {
                notes[i] = val;
            }
        }

        public override string ToString()
        {
            return string.Join(",", ToStringList());
        }

        public List<string> ToStringList()
        {
            List<string> lst = new List<string>();

            for (int i = 0; i < 24; i++)
            {
                if (notes[i])
                {
                    lst.Add(ConvertNoteValueToString(i));
                }
            }

            return lst;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {            

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            TwoOctaveNoteArray tona = (TwoOctaveNoteArray)obj;

            bool isEqual = true;

            for(int i = 0; i < 24; i++)
            {
                if(isEqual && tona[i] != this[i])
                {
                    isEqual = false;
                }
            }

            return isEqual;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                for (int index = 0; index < notes.Length; index++)
                {
                    hash = hash * 23 + notes[index].GetHashCode();
                }
                return hash;
            }
        }

        private string ConvertNoteValueToString(int i)
        {
            switch (i)
            {
                case 0:
                    return "C0";
                case 12:
                    return "C1";

                case 1:
                    return "C#0";
                case 13:
                    return "C#1";

                case 2:
                    return "D0";
                case 14:
                    return "D1";

                case 3:
                    return "D#0";
                case 15:
                    return "D#1";

                case 4:
                    return "E0";
                case 16:
                    return "E1";

                case 5:
                    return "F0";
                case 17:
                    return "F1";

                case 6:
                    return "F#0";
                case 18:
                    return "F#1";

                case 7:
                    return "G0";
                case 19:
                    return "G1";

                case 8:
                    return "G#0";
                case 20:
                    return "G#1";

                case 9:
                    return "A0";
                case 21:
                    return "A1";

                case 10:
                    return "A#0";
                case 22:
                    return "A#1";

                case 11:
                    return "B0";
                case 23:
                    return "B1";

                default:
                    return "?";

            }
        }
    }
}