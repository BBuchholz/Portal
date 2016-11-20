using NineWorldsDeep.Db.Sqlite;
using NineWorldsDeep.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for MediaMasterDisplay.xaml
    /// </summary>
    public partial class MediaMasterDisplay : UserControl
    {
        private MediaSubsetDb db;

        public MediaMasterDisplay()
        {
            InitializeComponent();
            db = new MediaSubsetDb();
            LoadMediaDevices();
        }

        public MediaDeviceModelItem SelectedMediaDevice
        {
            get
            {
                return (MediaDeviceModelItem)mMediaDevicesComboBox.SelectedItem;
            }
        }

        private void LoadMediaDevices()
        {
            mMediaDevicesComboBox.ItemsSource = db.GetAllMediaDevices();
        }

        private void mMediaDevicesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MediaDeviceModelItem mdmi = SelectedMediaDevice;                

            if(mdmi != null)
            {
                if(mdmi.MediaDeviceId == db.LocalDeviceId)
                {
                    RefreshMediaRoots();
                }
                else
                {
                    UI.Display.Message("External Devices not yet supported");
                }
            }
        }

        private void mAddMediaRootButton_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = UI.Prompt.ForFolder("C:\\");

            if(folderPath != null)
            {
                db.InsertMediaRoot(db.LocalDeviceId, folderPath);

                RefreshMediaRoots();
            }
        }

        private void RefreshMediaRoots()
        {
            MediaDeviceModelItem mdmi = SelectedMediaDevice;

            if (mdmi != null)
            {
                List<MediaRootModelItem> lst =
                            db.GetMediaRootsForDeviceId(mdmi.MediaDeviceId);

                mMediaRootsComboBox.ItemsSource = lst;
            }
        }
    }
}
