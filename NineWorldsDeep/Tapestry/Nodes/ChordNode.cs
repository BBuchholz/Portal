using NineWorldsDeep.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    public class ChordNode : TapestryNode
    {
        public ChordNode(string chordName, TwoOctaveNoteArray notes) : 
            base("Chord/" + notes)
        {
            ChordName = chordName;
            Notes = notes;
        }

        public TwoOctaveNoteArray Notes { get; private set; }
        public string ChordName { get; private set; }

        public override string GetShortName()
        {
            if (string.IsNullOrWhiteSpace(ChordName))
            {
                return "Chord/" + Notes;
            }

            return ChordName;
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
