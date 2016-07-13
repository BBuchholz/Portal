using System;

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

        public override void PerformSelectionAction()
        {
            var window = 
                new AudioBrowser.AudioBrowserMainWindow();
            window.Show();
        }
    }
}