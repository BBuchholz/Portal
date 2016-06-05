using System;

namespace NineWorldsDeep.FragmentCloud
{
    internal class AudioBrowserFragment : Fragment
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