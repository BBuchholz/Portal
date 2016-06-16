using NineWorldsDeep.Tapestry.Fragments;
using System;

namespace NineWorldsDeep.FragmentCloud
{
    public class StudioFragment : TapestryNode
    {
        public StudioFragment()
            : base("Studio")
        {
            AddChild(new StudioMainNode());
            AddChild(new StudioProjectsNode());
            AddChild(new StudioLyricsNode());
            AddChild(new StudioScalesNode());
            AddChild(new StudioKeyboardNode());
        }

        public override string GetShortName()
        {
            return "Studio";
        }

        public override void PerformSelectionAction()
        {
            var window = new Studio.StudioMainWindow();
            window.Show();
        }
    }
}