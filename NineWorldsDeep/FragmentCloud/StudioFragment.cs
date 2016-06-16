using NineWorldsDeep.Tapestry.Fragments;
using System;

namespace NineWorldsDeep.FragmentCloud
{
    public class StudioFragment : Fragment
    {
        public StudioFragment()
            : base("Studio")
        {
            AddChild(new StudioMainFragment());
            AddChild(new StudioProjectsFragment());
            AddChild(new StudioLyricsFragment());
            AddChild(new StudioScalesFragment());
            AddChild(new StudioKeyboardFragment());
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