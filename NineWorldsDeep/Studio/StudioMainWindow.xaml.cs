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
using System.Xml.Linq;

namespace NineWorldsDeep.Studio
{
    /// <summary>
    /// Interaction logic for StudioMainWindow.xaml
    /// </summary>
    public partial class StudioMainWindow : Window
    {
        ObservableCollection<AudioVignette> audioVignettes =
            new ObservableCollection<AudioVignette>();

        private string audioVignettesFolder = "C:\\NWD-AUX\\audioVignettes";

        public string AudioVignettesMasterFile { get; private set; }

        public StudioMainWindow()
        {
            InitializeComponent();
            AudioVignettesMasterFile =
                System.IO.Path.Combine(audioVignettesFolder,
                                       "masterFile.xml");
            lvAudioVignettes.ItemsSource = audioVignettes;
        }

        private void MenuItemLoad_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(AudioVignettesMasterFile))
            {
                audioVignettes.Clear();

                var doc = XDocument.Load(AudioVignettesMasterFile);

                //TODO: finish load functionality
            }
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            XElement audioVignettesEl = new XElement("audioVignettes");

            foreach (AudioVignette av in audioVignettes)
            {
                XElement audioVignetteEl =
                    new XElement("audioVignette",
                        new XElement("name", av.Name),
                        new XElement("path", av.DirectoryPath),
                        new XElement("elements"));

                foreach (FileElement fe in av.Elements)
                {
                    audioVignetteEl.Element("elements").Add(
                        new XElement("element",
                            new XElement("path", fe.Path),
                            new XElement("notes", fe.Notes)));
                }

                audioVignettesEl.Add(audioVignetteEl);
            }

            XDocument doc = new XDocument();
            doc.Add(audioVignettesEl);
            doc.Save(AudioVignettesMasterFile);
        }

        private void MenuItemAddFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = audioVignettesFolder;
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                AddAudioVignetteFromFolder(dialog.SelectedPath);
            }
        }

        private void AddAudioVignetteFromFolder(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);

            if (di.Exists)
            {
                AudioVignette av = new AudioVignette()
                {
                    Name = di.Name,
                    DirectoryPath = path
                };

                foreach (string filePath in Directory.GetFiles(path))
                {
                    av.Elements.Add(new FileElement()
                    {
                        Path = filePath
                    });
                }

                audioVignettes.Add(av);
            }
        }

        private void MenuItemCreate_Click(object sender, RoutedEventArgs e)
        {
            string name = UI.Prompt.Input("Name for new Audio Vignette?", TimeStamp.Now());
            if (!string.IsNullOrWhiteSpace(name))
            {
                CreateAudioVignette(name);
            }
        }

        private void CreateAudioVignette(string name)
        {
            string folderPath = System.IO.Path.Combine(audioVignettesFolder, name);

            if (Directory.Exists(folderPath))
            {
                MessageBox.Show("folder " + folderPath + " already exists!");
            }
            else
            {
                Directory.CreateDirectory(folderPath);
                AddAudioVignetteFromFolder(folderPath);
            }
        }

        private void MenuItemTimeStamp_Click(object sender, RoutedEventArgs e)
        {
            String current = DateTime.Now.ToString("yyyyMMddHHmmss");
            Clipboard.SetText(current);
            MessageBox.Show("[" + current + "] copied to clipboard");
        }

        private void VisualKeyboardTestHarness_Click(object sender, RoutedEventArgs e)
        {
            VisualKeyboardTestHarness vkth = new VisualKeyboardTestHarness();
            vkth.Show();
        }

        private void MenuItemLyricMatrix_Click(object sender, RoutedEventArgs e)
        {
            LyricMatrixGridTestHarness lmgth =
                new LyricMatrixGridTestHarness();
            lmgth.Show();
        }

        private void MenuItemProjectWindow_Click(object sender, RoutedEventArgs e)
        {
            ProjectsWindow pw = new ProjectsWindow();
            pw.Show();
        }

        private void MenuItemVisualScales_Click(object sender, RoutedEventArgs e)
        {
            new VisualScales().Show();
        }
    }
}
