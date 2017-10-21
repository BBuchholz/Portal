using NineWorldsDeep.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Muse.V5
{
    public class MuseV5PatternSignature
    {
        #region creation

        public MuseV5PatternSignature(string patternSignatureString)
        {
            PositionalValues = new List<int>();
            this.FromString(patternSignatureString);
        }

        #endregion

        #region properties

        public List<int> PositionalValues { get; private set; }

        #endregion

        #region public interface



        #endregion

        #region private helper methods

        private void FromString(string sig)
        {
            string[] values = sig.Split(',');

            foreach (string sigValue in values)
            {
                string trimmedSigValue = sigValue.Trim();

                int sigPosVal = int.Parse(trimmedSigValue);

                if (!PositionalValues.Contains(sigPosVal))
                {
                    PositionalValues.Add(sigPosVal);
                }
            }
        }

        #endregion
    }
}
