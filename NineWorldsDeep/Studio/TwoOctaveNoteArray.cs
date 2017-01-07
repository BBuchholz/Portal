using System;
using System.Collections.Generic;
using System.Linq;

namespace NineWorldsDeep.Studio
{
    public class TwoOctaveNoteArray
    {
        private List<int> noteIndexes = new List<int>();
        
        public bool this[int i]
        {
            get
            {
                if (noteIndexes.Contains(i))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            set
            {
                if (value)
                {
                    if (!noteIndexes.Contains(i))
                    {
                        noteIndexes.Add(i);
                    }
                }
                else
                {
                    noteIndexes.Remove(i);
                }
            }
        }

        public bool IsLowest(string noteName)
        {
            return Note.AreEquivalent(GetLowestNoteName(), noteName);
        }

        /// <summary>
        /// generates a global note array (notes from both octaves lit up by Absolute Value)
        /// </summary>
        /// <param name="chordNotes"></param>
        /// <returns></returns>
        public static TwoOctaveNoteArray Global(TwoOctaveNoteArray chordNotes)
        {
            TwoOctaveNoteArray newArray = new TwoOctaveNoteArray();

            foreach(int index in chordNotes.noteIndexes)
            {
                for(int i = 0; i < 24; i++)
                {
                    if(Note.AbsVal(index) == Note.AbsVal(i))
                    {
                        newArray[i] = true;
                    }
                }
            }

            return newArray;
        }

        public void Invert()
        {
            int maxIndex = noteIndexes.Max();

            int newIndex = maxIndex - 12;

            this[maxIndex] = false;
            this[newIndex] = true;

            CoaxAll();            
        }

        public string GetLowestNoteName()
        {
            int idx = noteIndexes.Min();
            return Note.ConvertNoteValueToString(idx);
        }

        public TwoOctaveNoteArray GetCopy()
        {
            TwoOctaveNoteArray newArray = new TwoOctaveNoteArray();

            foreach(int idx in noteIndexes)
            {
                newArray[idx] = true;
            }

            return newArray;
        }

        public bool Contains(string noteName)
        {
            foreach(string thisNoteName in ToStringList())
            {
                if(Note.AreEquivalent(thisNoteName, noteName))
                {
                    return true;
                }
            }

            return false;
        }

        public void CoaxAll()
        {
            int lowestIndex = noteIndexes.Min();
            List<int> newIndexes = new List<int>();
            List<int> oldIndexes = new List<int>();

            if(lowestIndex > 11)
            {
                int targetLowestIndex = lowestIndex % 12;

                int shift = lowestIndex - targetLowestIndex;

                foreach(int idx in noteIndexes)
                {
                    int newIdx = idx - shift;

                    //this[idx] = false;
                    //this[newIdx] = true;
                    oldIndexes.Add(idx);
                    newIndexes.Add(newIdx);
                }
            }

            if(lowestIndex < 0)
            {
                int targetLowestIndex = (lowestIndex % 12) + 12;

                int shift = targetLowestIndex - lowestIndex;

                foreach(int idx in noteIndexes)
                {
                    int newIdx = idx + shift;

                    //this[idx] = false;
                    //this[newIdx] = true;
                    oldIndexes.Add(idx);
                    newIndexes.Add(newIdx);
                }
            }

            foreach(int oldIndex in oldIndexes)
            {
                this[oldIndex] = false;
            }

            foreach(int newIndex in newIndexes)
            {
                this[newIndex] = true;
            }
        }
        
        public override string ToString()
        {
            return string.Join(",", ToStringList());
        }

        public List<string> ToStringList()
        {
            List<string> lst = new List<string>();

            noteIndexes.Sort();

            foreach (int idx in noteIndexes)
            {
                lst.Add(Note.ConvertNoteValueToString(idx));                
            }

            return lst;
        }
        
        public override bool Equals(object obj)
        {            

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            TwoOctaveNoteArray tona = (TwoOctaveNoteArray)obj;

            if(tona.noteIndexes.Count != noteIndexes.Count)
            {
                return false;
            }

            bool isEqual = true;

            foreach(int idx in tona.noteIndexes)
            {
                if (!noteIndexes.Contains(idx))
                {
                    isEqual = false;
                }
            }

            return isEqual;
        }

        public override int GetHashCode()
        {
            noteIndexes.Sort();

            unchecked
            {
                int hash = 17;
                foreach(int idx in noteIndexes)
                {
                    hash = hash * 23 + idx.GetHashCode();
                }
                return hash;
            }
        }

    }
}