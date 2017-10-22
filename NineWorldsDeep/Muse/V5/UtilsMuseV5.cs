using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Muse.V5
{
    public class UtilsMuseV5
    {
        #region fields

        private static Dictionary<char, int> RomanMap = new Dictionary<char, int>()
        {
            {'I', 1},
            {'V', 5},
            {'i', 1},
            {'v', 5}
        };

        #endregion

        #region public static methods

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

        public static MuseV5ScaleInstance GetScaleInstanceFromScaleDegrees(
            List<string> scaleDegrees, 
            MuseV5Note rootNote)
        {
            //to support situations where progressions borrow outside of their 
            //scale, we look for the overall likelihood of a progression
            //being minor or major

            int minorHits = HitsForMinorProgression(scaleDegrees);
            int majorHits = HitsForMajorProgression(scaleDegrees);
            
            if(majorHits > minorHits)
            {
                return MuseV5Scale.MajorScale(rootNote);
            }
            else
            {
                return MuseV5Scale.MinorScale(rootNote);
            }
        }

        #endregion

        #region private helper methods
        
        private static int HitsForMinorProgression(List<string> scaleDegrees)
        {
            string[] minorDegrees =
                new string[] { "i", "III", "iv", "v", "VI", "VII" };

            return minorDegrees.Where(s => scaleDegrees.Contains(s)).Count();
        }

        private static int HitsForMajorProgression(List<string> scaleDegrees)
        {
            string[] majorDegrees =
                new string[] { "I", "iii", "IV", "V", "vi", "vii" };

            return majorDegrees.Where(s => scaleDegrees.Contains(s)).Count();
        }

        #endregion
    }
}
