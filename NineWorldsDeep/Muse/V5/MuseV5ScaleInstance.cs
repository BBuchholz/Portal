using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Studio;

namespace NineWorldsDeep.Muse.V5
{
    public class MuseV5ScaleInstance : MuseV5PatternInstance
    {
        #region properties

        public string Name { get; private set; }

        #endregion

        #region creation

        public MuseV5ScaleInstance(MuseV5Note rootNote, MuseV5Scale scale) 
            : base(rootNote, scale.PatternSignature)
        {
            Name = rootNote + " " + scale.ScaleName;
        }

        #endregion
    }
}
