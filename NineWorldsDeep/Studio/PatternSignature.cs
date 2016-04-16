using System.Collections.Generic;

namespace NineWorldsDeep.Studio
{
    public class PatternSignature
    {
        private List<int> posVals = new List<int>();

        public PatternSignature(string v)
        {
            this.FromString(v);
        }

        public List<int> getSignaturePositionalValues()
        {
            return this.posVals;
        }

        private void FromString(string sig)
        {
            string[] values = sig.Split(',');

            foreach (string s in values)
            {
                int sigPosVal = int.Parse(s);

                if (!posVals.Contains(sigPosVal))
                {
                    posVals.Add(sigPosVal);
                }
            }
        }
    }
}