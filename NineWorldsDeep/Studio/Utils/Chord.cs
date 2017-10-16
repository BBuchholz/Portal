using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Tapestry.Nodes;

namespace NineWorldsDeep.Studio.Utils
{
    public class Chord
    {
        private TwoOctaveNoteArray _ChordNotes;

        public Chord(string chordName, TwoOctaveNoteArray notes)
        {
            ChordName = chordName;
            _ChordNotes = notes;
        }

        public string ChordName { get; private set; }
        public TwoOctaveNoteArray ChordNotes
        {
            get
            {
                if (Core.Configuration.GlobalNotes)
                {
                    return TwoOctaveNoteArray.Global(_ChordNotes);
                }
                else
                {
                    return _ChordNotes;
                }
            }

            private set
            {
                _ChordNotes = value;
            }
        }

        public Chord Inversion(string noteName)
        {
            if (!ChordNotes.Contains(noteName))
            {
                throw new ArgumentException(noteName + " not found in " + ChordName);
            }

            TwoOctaveNoteArray invertedNotes = ChordNotes.GetCopy();

            int inversionCount = 0;

            bool inversionNoteIsLowest = invertedNotes.IsLowest(noteName);

            while (!inversionNoteIsLowest)
            {
                invertedNotes.Invert();
                inversionCount++;
                inversionNoteIsLowest = invertedNotes.IsLowest(noteName);
            }

            string invertedChordName = ChordName;

            if (inversionCount > 0)
            {
                invertedChordName += "/" + invertedNotes.GetLowestNoteName().ToLower();
            }

            return new Chord(invertedChordName, invertedNotes);
        }

        //currently a mock
        public static ChordNode ParseToNode(string chordName)
        {
            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            Chord chord = null;

            switch (chordName)
            {
                // first inversion
                case "Dm/a":

                    //notes = MinorTriadFirstInversion(Note.ParseAbsoluteValue("D"));

                    //chord = new Chord(chordName, notes);

                    chord = MinorTriad("D").Inversion("a");

                    break;
                    
                case "Am":
                    
                    //notes = MinorTriad(Note.ParseAbsoluteValue("A"));
                    chord = MinorTriad("A");
                    break;

                // second inversion
                case "E/g#":

                    //notes = MajorTriadSecondInversion(Note.ParseAbsoluteValue("E"));
                    //chord = new Chord(chordName, notes);

                    chord = MajorTriad("E").Inversion("g#");

                    break;

                default:

                    throw new ArgumentException(chordName + " is not a supported chord name");
            }

            return new ChordNode(chord);
        }

        public static Chord DiminishedTriad(int rootNoteAbsVal)
        {
            return DiminishedTriad(Note.ConvertNoteValueToString(rootNoteAbsVal));
        }

        public static Chord DiminishedTriad(string noteName)
        {
            int rootNoteAbsVal = Note.ParseAbsoluteValue(noteName);

            string chordName = noteName.ToUpper() + "dim";
            
            return new Chord(chordName, DiminishedTriadNoteArray(rootNoteAbsVal));
        }

        //public TwoOctaveNoteArray FirstInversion()
        //{
        //    bool first = true;

        //    TwoOctaveNoteArray notes = new TwoOctaveNoteArray();
        //    return notes;
        //}

        public static Chord MajorTriad(int rootNoteAbsVal)
        {
            return MajorTriad(Note.ConvertNoteValueToString(rootNoteAbsVal));
        }

        public static Chord MajorTriad(string noteName)
        {
            int rootNoteAbsVal = Note.ParseAbsoluteValue(noteName);

            string chordName = noteName.ToUpper();

            //TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            //notes[rootNoteAbsVal] = true;
            //notes[rootNoteAbsVal + 4] = true;
            //notes[rootNoteAbsVal + 7] = true;

            return new Chord(chordName, MajorTriadNoteArray(rootNoteAbsVal));
        }

        public static Chord MinorTriad(int rootNoteAbsVal)
        {
            return MinorTriad(Note.ConvertNoteValueToString(rootNoteAbsVal));
        }

        public static Chord MinorTriad(string noteName)
        {
            //pattern is 0, 3, 7

            int rootNoteAbsVal = Note.ParseAbsoluteValue(noteName);

            string chordName = noteName.ToUpper() + "m";

            //TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            //notes[rootNoteAbsVal] = true;
            //notes[rootNoteAbsVal + 3] = true;
            //notes[rootNoteAbsVal + 7] = true;

            return new Chord(chordName, MinorTriadNoteArray(rootNoteAbsVal));
        }

        private static TwoOctaveNoteArray MajorTriadSecondInversion(int rootNote)
        {
            //pattern is -8, -5, 0

            //coax range
            while (rootNote - 8 < 0)
            {
                rootNote += 12;
            }

            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[rootNote - 8] = true;
            notes[rootNote - 5] = true;
            notes[rootNote] = true;

            return notes;
        }

        private static int CoaxRootNote(int rootNote)
        {
            ////always shift as low as octaves will allow
            //while(rootNote > 11)
            //{
            //    rootNote -= 12;
            //}

            ////minmum is 0 for low C
            //while (rootNote < 0)
            //{
            //    rootNote += 12;
            //}

            //return rootNote;

            return Note.CoaxNote(rootNote);
        }

        private static TwoOctaveNoteArray MinorTriadNoteArray(int rootNote)
        {
            //pattern is 0, 3, 7

            //coax range
            rootNote = CoaxRootNote(rootNote);

            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[rootNote] = true;
            notes[rootNote + 3] = true;
            notes[rootNote + 7] = true;

            return notes;
        }

        private static TwoOctaveNoteArray DiminishedTriadNoteArray(int rootNoteAbsVal)
        {
            //pattern is 0, 3, 6

            //coax range
            rootNoteAbsVal = CoaxRootNote(rootNoteAbsVal);

            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[rootNoteAbsVal] = true;
            notes[rootNoteAbsVal + 3] = true;
            notes[rootNoteAbsVal + 6] = true;

            return notes;
        }

        private static TwoOctaveNoteArray MajorTriadNoteArray(int rootNoteAbsVal)
        {
            //pattern is 0, 4, 7

            //coax range
            rootNoteAbsVal = CoaxRootNote(rootNoteAbsVal);
            
            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[rootNoteAbsVal] = true;
            notes[rootNoteAbsVal + 4] = true;
            notes[rootNoteAbsVal + 7] = true;

            return notes;
        }

        private static TwoOctaveNoteArray MinorTriadFirstInversion(int rootNote)
        {
            //pattern is -5, 0, 3

            //coax range
            while(rootNote - 5 < 0)
            {
                rootNote += 12;
            }

            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[rootNote - 5] = true;
            notes[rootNote] = true;
            notes[rootNote + 3] = true;

            return notes;
        }
        
    }
}
