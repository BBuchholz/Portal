using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Studio.Utils
{
    public class Notes
    {
        public static int ParseAbsoluteValue(string noteName)
        {
            noteName = noteName.Trim().ToLower();
            
            int absoluteValue = -1;

            switch (noteName)
            {
                case "c":

                    absoluteValue = 0;
                    break;

                case "d":

                    absoluteValue = 2;
                    break;

                case "e":

                    absoluteValue = 4;
                    break;

                case "f":

                    absoluteValue = 5;
                    break;

                case "g":

                    absoluteValue = 7;
                    break;

                case "a":

                    absoluteValue = 9;
                    break;

                case "b":

                    absoluteValue = 11;
                    break;


                //sharps
                case "c#":

                    absoluteValue = 1;
                    break;

                case "d#":

                    absoluteValue = 3;
                    break;

                case "e#":

                    absoluteValue = 5; // F
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

                case "b#":

                    absoluteValue = 12; // C
                    break;


                //flats
                case "cb":

                    absoluteValue = 11; // B
                    break;

                case "db":

                    absoluteValue = 1;
                    break;

                case "eb":

                    absoluteValue = 3;
                    break;

                case "fb":

                    absoluteValue = 4; // E
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

            if(absoluteValue > -1)
            {
                return absoluteValue;
            }
            else
            {
                throw new ArgumentException("invalid note name: " + noteName);
            }
        }
    }
}
