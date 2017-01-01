using System.Collections.Generic;

namespace NineWorldsDeep.Studio
{
    public class PatternSignature
    {
        private List<int> posVals = new List<int>();

        public PatternSignature(string patternSignatureString)
        {
            this.FromString(patternSignatureString);
        }

        public List<int> getSignaturePositionalValues()
        {
            return this.posVals;
        }

        private void FromString(string sig)
        {
            string[] values = sig.Split(',');

            foreach (string sigValue in values)
            {
                string trimmedSigValue = sigValue.Trim();

                int sigPosVal = int.Parse(trimmedSigValue);

                if (!posVals.Contains(sigPosVal))
                {
                    posVals.Add(sigPosVal);
                }
            }
        }
    }
}