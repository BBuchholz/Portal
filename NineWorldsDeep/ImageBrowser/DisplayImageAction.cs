using System.Windows.Controls;
using NineWorldsDeep.Tagger;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System;

namespace NineWorldsDeep.ImageBrowser
{
    public class DisplayImageAction : FileElementActionSubscriber
    {
        private Image imageControl;
        private FileElement fileElement;

        public DisplayImageAction(Image imageControl)
        {
            this.imageControl = imageControl;
            imageControl.MouseDown += ImageControl_MouseDown;
        }

        public void PerformAction(FileElement fe)
        {
            if (fe != null)
            {
                fileElement = fe;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fe.Path);
                bitmap.EndInit();
                imageControl.Source = bitmap;
            }
        }

        private void ImageControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left &&
                e.ClickCount == 2 &&
                fileElement != null)
            {
                Process proc = new Process();
                proc.StartInfo.FileName = fileElement.Path;
                proc.Start();
            }
        }
    }
}