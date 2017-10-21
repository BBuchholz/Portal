using NineWorldsDeep.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Muse.V5
{
    public class MuseV5ChordInstance
    {
        #region properties

        public string Name { get; private set; }



        public TwoOctaveNoteArray ChordNotes
        {
            get
            {
                //if (Core.Configuration.GlobalNotes)
                //{
                //    return TwoOctaveNoteArray.Global(_ChordNotes);
                //}
                //else
                //{
                //    return _ChordNotes;
                //}
                return new TwoOctaveNoteArray();
            }

            private set
            {
                //_ChordNotes = value;
            }
        }

        #endregion
    }
}
