using NineWorldsDeep.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace NineWorldsDeep.Studio
{
    /// <summary>
    /// Interaction logic for ProjectsWindow.xaml
    /// </summary>
    public partial class ProjectsWindow : Window
    {
        public ProjectsWindow()
        {
            InitializeComponent();
            LoadProjectTypes();
        }

        private void LoadProjectTypes()
        {
            cmbFolders.Items.Add(new AbletonProjects());
            cmbFolders.Items.Add(new AudacityProjects());
            cmbFolders.Items.Add(new CubaseProjects());
            cmbFolders.Items.Add(new FLStudioProjects());
            cmbFolders.Items.Add(new VisualStudioProjects());
        }

        private void cmbFolders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProjectType pt = (ProjectType)cmbFolders.SelectedItem;
            if (pt != null)
            {
                lvProjectFiles.ItemsSource = pt.GetProjectFiles();
            }
        }

        private void lvProjectFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string filePath = (string)lvProjectFiles.SelectedItem;
            ProjectType pt = (ProjectType)cmbFolders.SelectedItem;

            if (pt != null && File.Exists(filePath))
            {
                ProjectTypeInstance pti = pt.InstantiateForPath(filePath);
                pti.PreProcessing();
                Process proc = new Process();
                proc.StartInfo.FileName = filePath;
                proc.EnableRaisingEvents = true;
                proc.Exited += pti.Process_Exited;
                proc.Start();
            }
        }

        private void MenuItemExportProjectList_Click(object sender, RoutedEventArgs e)
        {
            ProjectType pt = (ProjectType)cmbFolders.SelectedItem;
            if (pt != null)
            {
                String projectType = pt.DisplayName;
                List<ProjectListEntry> projectNameList = pt.GetProjectNames();

                Display.Grid(projectType + " projects:", projectNameList);
            }
            else
            {
                Display.Message("project type empty");
            }
        }
    }
}
