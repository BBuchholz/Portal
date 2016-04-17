using System.Windows.Controls;
using NineWorldsDeep.Tagger;
using System.Media;

namespace NineWorldsDeep.AudioBrowser
{
    class ConditionalPlayAction : FileElementActionSubscriber
    {
        private MenuItem chkPlayIfChecked;

        public ConditionalPlayAction(MenuItem chkPlayIfChecked)
        {
            this.chkPlayIfChecked = chkPlayIfChecked;
        }

        public void PerformAction(FileElement fe)
        {
            if (fe != null && chkPlayIfChecked.IsChecked)
            {
                SoundPlayer player = new SoundPlayer(fe.Path);
                player.Play();
            }
        }
    }
}