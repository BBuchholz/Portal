using NineWorldsDeep.Core;
using NineWorldsDeep.FragmentCloud;
using NineWorldsDeep.Tapestry.Nodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            FileDetailsControl.Display(fileNode);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PlayCurrent();
        }

        private void PlayCurrent()
        {
            if (fileNode != null && File.Exists(fileNode.Path))
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

        private void chkLoop_Checked(object sender, RoutedEventArgs e)
        {
            //restart so looping or non-looping takes effect
            PlayCurrent();
        }

        private void OpenExternallyButton_Click(object sender, RoutedEventArgs e)
        {
            NwdUtils.OpenFileExternally(fileNode.Path);
        }

        private string CopyToVoiceMemoStaging()
        {
            string vmStagingFolderPath = Configuration.VoiceMemoStagingFolder;

            //ensure directory exists
            Directory.CreateDirectory(vmStagingFolderPath);

            //create destination file path
            string fName = System.IO.Path.GetFileName(fileNode.Path);
            string destFilePath = System.IO.Path.Combine(vmStagingFolderPath, fName);

            //copy if !exists, else message                
            if (!File.Exists(destFilePath))
            {
                File.Copy(fileNode.Path, destFilePath);
                return "copied to: " + destFilePath;
            }
            else
            {
                return "file exists: " + destFilePath;
            }
        }

        private void CopyToExportStagingButton_Click(object sender, RoutedEventArgs e)
        {
            if (fileNode != null)
            {
                UI.Display.Message(CopyToVoiceMemoStaging());
            }
            else
            {
                UI.Display.Message("failed. file null.");
            }
        }

        private void SendToTrashButton_Click(object sender, RoutedEventArgs e)
        {

            UI.Display.Message("need to get tagging working first.");


            /////MODIFY THIS TO USE FILE NODE INSTEAD OF FILE ELEMENT
            /////NEED TO ACCOUNT FOR DB UPDATE (fe.MoveToTrash() below)
            /////WITHOUT USING FILE ELEMENT BEFORE WE CAN IMPLEMENT THIS

            //delete
            //FileElement fe = (FileElement)lvFileElements.SelectedItem;

            //string msg = "Are you sure you want to move this file to trash? " +
            //    "Be aware that these tags will be permanently lost even if " +
            //    "file is restored from trash: ";

            //if (fe != null && UI.Prompt.Confirm(msg + fe.TagString, true))
            //{
            //    StopAudioButton.RaiseEvent(
            //        new RoutedEventArgs(ButtonBase.ClickEvent));

            //    fe.MoveToTrash(dbCore);

            //    //remove path from tag matrix
            //    tagMatrix.RemovePath(fe.Path);

            //    //refresh list
            //    LoadFromSelectedTag();
            //}
        }
    }
}
