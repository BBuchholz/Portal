using NineWorldsDeep.Tagger;
using System.Media;

namespace NineWorldsDeep.AudioBrowser
{
    class PlayAction : FileElementActionSubscriber
    {
        public void PerformAction(FileElement fe)
        {
            if (fe != null)
            {
                SoundPlayer player = new SoundPlayer(fe.Path);
                player.Play();
            }
        }
    }
}