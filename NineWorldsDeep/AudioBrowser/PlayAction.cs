using NineWorldsDeep.Tagger;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System;

namespace NineWorldsDeep.AudioBrowser
{
    class PlayAction : FileElementActionSubscriber
    {
        private SoundPlayer player;

        public PlayAction(Button stopAudioButton)
        {
            stopAudioButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(StopAudioButton_Click));
        }

        private void StopAudioButton_Click(object sender, RoutedEventArgs e)
        {
            if(player != null)
            {
                player.Stop();
            }
        }

        public void PerformAction(FileElement fe)
        {
            if (fe != null)
            {
                player = new SoundPlayer(fe.Path);
                player.Play();
            }
        }
    }
}