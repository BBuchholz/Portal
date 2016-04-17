using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Shapes;

namespace NineWorldsDeep.Synergy
{
    /// <summary>
    /// Interaction logic for ObjectGridWindow.xaml
    /// </summary>
    public partial class ObjectGridWindow : Window
    {
        private ObservableCollection<SynergyRowObject> rowObjects;
        private Parser.Parser p = new Parser.Parser();

        public ObjectGridWindow()
        {
            InitializeComponent();
            rowObjects = new ObservableCollection<SynergyRowObject>();
            dataGrid.ItemsSource = rowObjects;
            MenuController menu = new MenuController();
            menu.Configure(mainMenu);

            menu.AddMenuItem("Phone", "Load Synced FileMatrix", LoadSyncedPhoneFileMatrix);
            menu.AddMenuItem("RowObjects", "Load Metadata", LoadMetaData);
        }

        private void LoadMetaData(object sender, RoutedEventArgs e)
        {
            //load images for comparison
            MultiMap<string, string> fileNamesToLocalPaths =
                new MultiMap<string, string>(StringComparer.CurrentCultureIgnoreCase);

            List<string> localPaths = Directory.GetFiles(Configuration.ImagesFolder,
                                                      "*.*", SearchOption.AllDirectories).ToList();

            localPaths.AddRange(Directory.GetFiles(Configuration.CameraFolder,
                                                      "*.*", SearchOption.AllDirectories));

            foreach (string path in localPaths)
            {
                string fileName = System.IO.Path.GetFileName(path);
                fileNamesToLocalPaths.Add(fileName, path);
            }

            foreach (SynergyRowObject sro in rowObjects)
            {
                sro.FileName = System.IO.Path.GetFileName(sro.DevicePath);

                int count = 0;

                if (fileNamesToLocalPaths.ContainsKey(sro.FileName))
                {
                    count = fileNamesToLocalPaths[sro.FileName].Count;
                }

                sro.FileNameMatchCount = count;
            }
        }

        private void LoadSyncedPhoneFileMatrix(object sender, RoutedEventArgs e)
        {
            //load displayNameIndex.txt
            List<DisplayNameIndexEntry> displayNames = new List<DisplayNameIndexEntry>();
            string path = Configuration.GetPhoneSyncConfigFilePath("displayNameIndex");
            foreach (string line in File.ReadAllLines(path))
            {
                displayNames.Add(new DisplayNameIndexEntry()
                {
                    DisplayName = p.Extract("displayName", line),
                    DevicePath = p.Extract("path", line)
                });
            }

            //load fileHashIndex.txt
            List<FileHashIndexEntry> fileHashes = new List<FileHashIndexEntry>();
            path = Configuration.GetPhoneSyncConfigFilePath("fileHashIndex");
            foreach (string line in File.ReadAllLines(path))
            {
                fileHashes.Add(new FileHashIndexEntry()
                {
                    DevicePath = p.Extract("path", line),
                    SHA1Hash = p.Extract("sha1Hash", line)
                });
            }

            IEnumerable<SynergyRowObject> joinResult =
                from displayName in displayNames
                join fileHash in fileHashes
                on displayName.DevicePath equals fileHash.DevicePath
                select new SynergyRowObject
                {
                    SHA1Hash = fileHash.SHA1Hash,
                    DisplayName = displayName.DisplayName,
                    DevicePath = fileHash.DevicePath
                };

            LoadObjects(joinResult);
        }

        private void LoadObjects(IEnumerable<SynergyRowObject> lst)
        {
            //prevent notify changed firing for each one (this is a hack)
            //may even be less efficient initializing a new collection
            //look into this when you get the chance, at least its encapsulated :)
            rowObjects = new ObservableCollection<SynergyRowObject>(lst);
            dataGrid.ItemsSource = rowObjects;
        }

    }
}
