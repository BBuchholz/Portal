using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NineWorldsDeep.Studio.ArpaBet
{
    public class ArpaBetLoader
    {
        private static Regex regex = new Regex("^[A-Z]+");
        
        public static IEnumerable<ArpaBetPair> RetrieveArpaBet()
        {
            List<ArpaBetPair> lst = new List<ArpaBetPair>();
            
            using (StreamReader sr = 
                new StreamReader(Configuration.GetArphaBetFilePath()))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    if (IsArpaBetWordDefinition(line))
                    {
                        lst.Add(ToArpaBetPair(line));
                    }
                }
            }
            
            return lst;
        }



        private static ArpaBetPair ToArpaBetPair(string line)
        {
            ArpaBetPair abp = new ArpaBetPair();
            string[] arr = line.Split(new char[] { ' ' }, 2, 
                StringSplitOptions.RemoveEmptyEntries);

            string parenRemoved =
                arr[0].Split(new char[] { '(' }, 2)[0];

            abp.Word = parenRemoved;

            string arpaBetValueWithStress = arr[1];

            string arpaBetValueWithoutStress = 
                Regex.Replace(arpaBetValueWithStress, @"[\d]", string.Empty);

            abp.StrippedArpaBetValue = arpaBetValueWithoutStress;

            return abp;
        }

        public static bool IsArpaBetWordDefinition(string line)
        {
            Match m = regex.Match(line);

            return m.Success;
        }
    }
}
