using NineWorldsDeep.Core;
using NineWorldsDeep.Tapestry.Nodes;
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
using System.Windows.Threading;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for AudioDisplayV5.xaml
    /// </summary>
    public partial class AudioV5Display : UserControl
    {
        //TODO: replace this with MediaElement class
        //private SoundPlayer player;

        //refer to: https://docs.microsoft.com/en-us/dotnet/framework/wpf/graphics-multimedia/how-to-control-a-mediaelement-play-pause-stop-volume-and-speed 
        
        private FileSystemNode fileNode;
        private DispatcherTimer ticker;
        private bool isDragging = false;
        private bool isPlaying = false;

        public AudioV5Display()
        {
            InitializeComponent();
            ticker = new DispatcherTimer();
            ticker.Interval = new TimeSpan(0, 0, 0, 0, 10);
            ticker.Tick += Tick;
            ticker.Start();
        }

        private void Tick(object sender, EventArgs e)
        {
            if (!isDragging)
            {
                //this little bit of code keeps the slider from 
                //tripping over its own process (just trust me :)
                sliderSeek.ValueChanged -= sliderSeek_ValueChanged;
                sliderSeek.Value = meAudioMediaElement.Position.TotalMilliseconds;
                tbSeek.Text = ToStringDisplay(meAudioMediaElement.Position);
                sliderSeek.ValueChanged += sliderSeek_ValueChanged;
            }
        }

        private string ToStringDisplay(TimeSpan ts)
        {
            int hh = ts.Hours;
            int mm = ts.Minutes;
            int ss = ts.Seconds;
            int mi = ts.Milliseconds;

            string output;
            
            if (hh > 0)
            {
                output = string.Format("{0}:{1}:{2}:{3}", hh.ToString("00"), mm.ToString("00"), ss.ToString("00"), mi.ToString("000"));
            }
            else
            {
                output = string.Format("{0}:{1}:{2}", mm.ToString("00"), ss.ToString("00"), mi.ToString("000"));
            }

            return output;
        }

        public void Display(FileSystemNode nd)
        {
            meAudioMediaElement.Stop();
            this.isPlaying = false;
            fileNode = nd;
            FileDetailsControl.Display(fileNode);
            meAudioMediaElement.Source = new Uri(fileNode.Path);

            if (chkPlayOnSelect.IsChecked.Value)
            {
                PlayMedia();
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PlayCurrent();
        }

        private void PlayCurrent()
        {
            if (fileNode != null && File.Exists(fileNode.Path))
            {              
                meAudioMediaElement.Play();
                this.isPlaying = true;
                InitializePropertyValues();
            }
        }

        private void InitializePropertyValues()
        {
            meAudioMediaElement.Volume = (double)sliderVolume.Value;            
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            //if (player != null)
            //{
            //    player.Stop();
            //}

            meAudioMediaElement.Stop();
            this.isPlaying = false;
        }

        private void chkLoop_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.isPlaying && chkLoop.IsChecked.Value)
            {
                PlayMedia();
            }
        }

        private void chkPlayOnSelect_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.isPlaying && chkPlayOnSelect.IsChecked.Value)
            {
                PlayMedia();
            }
        }

        private void PlayMedia()
        {
            meAudioMediaElement.Play();
            this.isPlaying = true;
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

        private void meAudioMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            sliderSeek.Maximum = meAudioMediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;             
        }

        private void meAudioMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (chkLoop.IsChecked.Value)
            {
                meAudioMediaElement.Position = TimeSpan.Zero;
                meAudioMediaElement.Play();
                this.isPlaying = true;
            }
            else
            {
                meAudioMediaElement.Stop();
                this.isPlaying = false;
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            meAudioMediaElement.Pause();
            this.isPlaying = false;
        }

        private void sliderSeek_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isDragging)
            {
                ProcessSeekChange();
            }
        }

        private void ProcessSeekChange()
        {
            int sliderValue = (int)sliderSeek.Value;

            //TimeSpan(days, hours, minutes, seconds, milliseconds)
            meAudioMediaElement.Position =
                new TimeSpan(0, 0, 0, 0, sliderValue);
        }

        private void sliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            meAudioMediaElement.Volume = sliderVolume.Value;
        }

        private void sliderSeek_DragStarted(object sender, RoutedEventArgs e)
        {
            this.isDragging = true;
        }

        private void sliderSeek_DragCompleted(object sender, RoutedEventArgs e)
        {
            ProcessSeekChange();
            this.isDragging = false;
            meAudioMediaElement.Play();
            this.isPlaying = true;
        }

    }
}
