using NineWorldsDeep.Muse.V5;
using NineWorldsDeep.Studio;
using NineWorldsDeep.Studio.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    public class ChordNode : TapestryNode
    {
        public ChordNode(MuseV5ChordInstance chord) :
            base("Chord/" + chord.ChordNotes)
        {
            Chord = chord;
        }
        
        public MuseV5ChordInstance Chord { get; private set; }

        public override string GetShortName()
        {
            //if (string.IsNullOrWhiteSpace(Chord.ChordName))
            //{
            //    return "Chord/" + Chord.ChordNotes;
            //}

            //return Chord.ChordName;

            return null;
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            //do nothing
        }

        public override TapestryNodeType NodeType
        {
            get
            {
                return TapestryNodeType.Chord;
            }
        }
    }
}
