using NineWorldsDeep.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections;

namespace NineWorldsDeep.Muse.V5
{
    public class MuseV5Note
    {
        #region creation

        public MuseV5Note(string name)
        {
            NoteName = name;
            PositionalValue = ParseNameToAbsVal(NoteName);
        }

        public MuseV5Note(int positionalValue)
        {
            PositionalValue = positionalValue;
            NoteName = ConvertNoteValueToString(PositionalValue);
        }

        #endregion

        #region properties

        public int PositionalValue { get; private set; }
        public string NoteName { get; private set; }

        #endregion

        #region public interface

        public override string ToString()
        {
            return NoteName;
        }

        #endregion

        #region static methods

        public static int CoaxNote(int noteIndex)
        {
            //always shift as low as octaves will allow
            while (noteIndex > 11)
            {
                noteIndex -= 12;
            }

            //minmum is 0 for low C
            while (noteIndex < 0)
            {
                noteIndex += 12;
            }

            return noteIndex;
        }

        public static List<MuseV5Note> AllNotes()
        {
            List<MuseV5Note> lst = new List<MuseV5Note>();

            foreach(string noteName in AllNoteNames())
            {
                lst.Add(new MuseV5Note(noteName));
            }

            return lst;
        }

        public static string ConvertNoteValueToString(
            int i,
            bool includeOctaveNumeral = false)
        {
            i = CoaxNote(i);

            string noteName = "";

            switch (i)
            {
                case 0:
                    noteName = "C0";
                    break;

                case 12:
                    noteName = "C1";
                    break;


                case 1:
                    noteName = "C#0";
                    break;

                case 13:
                    noteName = "C#1";
                    break;


                case 2:
                    noteName = "D0";
                    break;

                case 14:
                    noteName = "D1";
                    break;


                case 3:
                    noteName = "D#0";
                    break;

                case 15:
                    noteName = "D#1";
                    break;


                case 4:
                    noteName = "E0";
                    break;

                case 16:
                    noteName = "E1";
                    break;


                case 5:
                    noteName = "F0";
                    break;

                case 17:
                    noteName = "F1";
                    break;


                case 6:
                    noteName = "F#0";
                    break;

                case 18:
                    noteName = "F#1";
                    break;


                case 7:
                    noteName = "G0";
                    break;

                case 19:
                    noteName = "G1";
                    break;


                case 8:
                    noteName = "G#0";
                    break;

                case 20:
                    noteName = "G#1";
                    break;


                case 9:
                    noteName = "A0";
                    break;

                case 21:
                    noteName = "A1";
                    break;


                case 10:
                    noteName = "A#0";
                    break;

                case 22:
                    noteName = "A#1";
                    break;


                case 11:
                    noteName = "B0";
                    break;

                case 23:
                    noteName = "B1";
                    break;


                default:
                    throw new ArgumentException("note index " + i + " out of bounds");

            }

            if (!includeOctaveNumeral)
            {
                noteName = TrimOctaveNumeral(noteName);
            }

            return noteName;
        }

        public static int AbsoluteVal(int noteValue)
        {
            while (noteValue > 11)
            {
                noteValue -= 12;
            }

            while (noteValue < 0)
            {
                noteValue += 12;
            }

            return noteValue;
        }

        public static List<string> AllNoteNames()
        {
            TwoOctaveNoteArray octaveArray =
                new TwoOctaveNoteArray();

            //populate first twelve
            for (int i = 0; i < 12; i++)
            {
                octaveArray[i] = true;
            }

            return octaveArray.ToStringList();
        }

        public static bool AreEquivalent(string thisNoteName, string noteName)
        {
            return ParseAbsoluteValue(thisNoteName) == ParseAbsoluteValue(noteName);

        }

        private static string TrimOctaveNumeral(string noteName)
        {
            return Regex.Replace(noteName, @"[\d]", string.Empty);
        }

        public static int ParseAbsoluteValue(string noteName)
        {
            //need to support "C", "C0", "C2", "B#" &c.

            noteName = noteName.Trim().ToLower();

            //remove octave digits if they exist
            noteName = TrimOctaveNumeral(noteName);

            int absoluteValue = -1;

            switch (noteName)
            {
                case "c":
                case "b#":

                    absoluteValue = 0;
                    break;

                case "d":

                    absoluteValue = 2;
                    break;

                case "e":
                case "fb":

                    absoluteValue = 4;
                    break;

                case "f":
                case "e#":

                    absoluteValue = 5;
                    break;

                case "g":

                    absoluteValue = 7;
                    break;

                case "a":

                    absoluteValue = 9;
                    break;

                case "b":
                case "cb":

                    absoluteValue = 11;
                    break;


                //sharps
                case "c#":

                    absoluteValue = 1;
                    break;

                case "d#":

                    absoluteValue = 3;
                    break;

                case "f#":

                    absoluteValue = 6;
                    break;

                case "g#":

                    absoluteValue = 8;
                    break;

                case "a#":

                    absoluteValue = 10;
                    break;

                //flats
                case "db":

                    absoluteValue = 1;
                    break;

                case "eb":

                    absoluteValue = 3;
                    break;

                case "gb":

                    absoluteValue = 6;
                    break;

                case "ab":

                    absoluteValue = 8;
                    break;

                case "bb":

                    absoluteValue = 10;
                    break;
            }

            if (absoluteValue > -1)
            {
                return absoluteValue;
            }
            else
            {
                throw new ArgumentException("invalid note name: " + noteName);
            }
        }

        public static int ParseNameToAbsVal(string name)
        {
            switch (name)
            {
                case "C":
                    return 0;
                case "C#/Db":
                case "C#":
                case "Db":
                    return 1;
                case "D":
                    return 2;
                case "D#/Eb":
                case "D#":
                case "Eb":
                    return 3;
                case "E":
                    return 4;
                case "F":
                    return 5;
                case "F#/Gb":
                case "F#":
                case "Gb":
                    return 6;
                case "G":
                    return 7;
                case "G#/Ab":
                case "G#":
                case "Ab":
                    return 8;
                case "A":
                    return 9;
                case "A#/Bb":
                case "A#":
                case "Bb":
                    return 10;
                case "B":
                    return 11;
                default:
                    throw new InvalidNoteParseException();
            }
        }



        #endregion
    }
}
