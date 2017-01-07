using System;
using NineWorldsDeep.Tapestry;

namespace NineWorldsDeep.FragmentCloud
{
    internal class AudioBrowserFragment : Tapestry.TapestryNode
    {
        public AudioBrowserFragment()
            : base("Audio")
        {
        }

        public override string GetShortName()
        {
            return "Audio";
        }

        public override bool Parallels(Tapestry.TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            var window = 
                new AudioBrowser.AudioBrowserMainWindow();
            window.Show();
            UI.Utils.MinimizeMainWindow();
        }
    }
}