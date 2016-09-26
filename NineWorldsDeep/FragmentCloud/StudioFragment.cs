using NineWorldsDeep.Tapestry.Fragments;
using System;
using NineWorldsDeep.Tapestry;

namespace NineWorldsDeep.FragmentCloud
{
    public class StudioNode : Tapestry.TapestryNode
    {
        public StudioNode()
            : base("Studio")
        {
            AddChild(new StudioMainNode());
            AddChild(new StudioProjectsNode());
            AddChild(new StudioLyricsNode());
            AddChild(new StudioScalesNode());
            AddChild(new StudioKeyboardNode());
            AddChild(new ArpaBetNode());
        }

        public override string GetShortName()
        {
            return "Studio";
        }

        public override bool Parallels(Tapestry.TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            var window = new Studio.StudioMainWindow();
            window.Show();
        }
    }
}