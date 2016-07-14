using NineWorldsDeep.FragmentCloud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for AudioNodeDisplay.xaml
    /// </summary>
    public partial class AudioNodeDisplay : UserControl
    {
        private SoundPlayer player;

        private FileSystemNode fileNode;

        public AudioNodeDisplay()
        {
            InitializeComponent();
        }

        public void Display(FileSystemNode nd)
        {
            fileNode = nd;
            txtMultiLine.Text = nd.ToMultiLineDetail();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if(fileNode != null && File.Exists(fileNode.Path))
            {
                player = new SoundPlayer(fileNode.Path);

                if (chkLoop.IsChecked.Value)
                {
                    player.PlayLooping();
                }
                else
                {
                    player.Play();
                }
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if(player != null)
            {
                player.Stop();
            }
        }
    }
}
