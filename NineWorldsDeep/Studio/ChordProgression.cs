using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Studio.Utils;
using NineWorldsDeep.Studio.Model;

namespace NineWorldsDeep.Studio
{
    public class ChordProgression
    {
        public ChordProgression(string signature)
        {
            Signature = signature;
        }

        public int ChordProgressionId { get; set; }
        public string Signature { get; private set; }
        public string Notes { get; set; }

        private static Dictionary<char, int> RomanMap = new Dictionary<char, int>()
        {
            {'I', 1},
            {'V', 5},
            {'i', 1},
            {'v', 5}
        };

        public static int RomanToInteger(string romanNumeral)
        {
            //adapted from http://stackoverflow.com/questions/14900228/roman-numbers-to-integers

            int number = 0;
            for (int i = 0; i < romanNumeral.Length; i++)
            {
                if (i + 1 < romanNumeral.Length && 
                    RomanMap[romanNumeral[i]] < RomanMap[romanNumeral[i + 1]])
                {
                    number -= RomanMap[romanNumeral[i]];
                }
                else
                {
                    number += RomanMap[romanNumeral[i]];
                }
            }

            return number;
        }

        public override string ToString()
        {
            return Signature;
        }

        //public List<Chord> ToChordList(string keyNoteName)
        //{
        //    return ChordProgression.ToChordList(keyNoteName, Signature);
        //}

        public static List<ChordProgression> AllProgressions()
        {
            Db.Sqlite.StudioV5SubsetDb db = new Db.Sqlite.StudioV5SubsetDb();

            List<ChordProgression> lst = db.GetAllChordProgressions();
                        
            return lst;
        }

        public static List<Chord> ToChordList(string keyNoteName, string progressionSignature)
        {
            List<Chord> chordList = new List<Chord>();
            List<string> scaleDegrees = ToScaleDegrees(progressionSignature);

            //if (IsMajorProgression(scaleDegrees))
            //{
            //    Scale majorScale = Scale.Major(keyNoteName);

            //    foreach (string scaleDegree in scaleDegrees)
            //    {
            //        chordList.Add(majorScale.ScaleDegreeToChord(scaleDegree));
            //    }
            //}
            //else if (IsMinorProgression(scaleDegrees))
            //{
            //    Scale minorScale = Scale.Minor(keyNoteName);

            //    foreach (string scaleDegree in scaleDegrees)
            //    {
            //        chordList.Add(minorScale.ScaleDegreeToChord(scaleDegree));
            //    }
            //}

            return chordList;
        }

        private static bool IsMinorProgression(List<string> scaleDegrees)
        {
            string[] minorDegrees =
                new string[] { "i", "III", "iv", "v", "VI", "VII" };

            return minorDegrees.Any(s => scaleDegrees.Contains(s));
        }

        private static bool IsMajorProgression(List<string> scaleDegrees)
        {
            string[] majorDegrees = 
                new string[] { "I", "iii", "IV", "V", "vi", "vii" };

            return majorDegrees.Any(s => scaleDegrees.Contains(s));
        }

        public static List<string> ToScaleDegrees(string progressionSignature)
        {
            return progressionSignature
                .Split(new String[] { "-" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(sd => sd.Trim())
                .ToList();
        }

        public static bool IsValidSignature(string progressionSignature)
        {
            List<string> scaleDegrees = ToScaleDegrees(progressionSignature);

            //make sure we aren't mixing scale degrees in our signature
            return IsMajorProgression(scaleDegrees) != IsMinorProgression(scaleDegrees);
        }

        //SEE Scale.cs (need to implement for scale degrees)

        // MINOR SCALE CHORDS
        // 1=minor, 2=dim, 3=major, 4=minor, 5=minor, 6=major, 7=major

        // MAJOR SCALE CHORDS
        // 1=Major, 2=minor, 3=minor, 4=Major, 5=Major, 6=minor, 7=dim


    }
}
