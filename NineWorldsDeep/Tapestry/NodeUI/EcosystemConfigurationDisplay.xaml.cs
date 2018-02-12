using NineWorldsDeep.Core;
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
    /// Interaction logic for EcosystemConfigurationDisplay.xaml
    /// </summary>
    public partial class EcosystemConfigurationDisplay : UserControl
    {
        #region fields
        #endregion

        #region creation

        public EcosystemConfigurationDisplay()
        {
            InitializeComponent();

            RefreshFolderLabels();
        }

        #endregion

        #region event handlers

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Core.UtilsUi.ProcessExpanderState((Expander)sender);
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            Core.UtilsUi.ProcessExpanderState((Expander)sender);
        }

        private void RefreshFoundFolders_Click(object sender, RoutedEventArgs e)
        {
            RefreshFolderList();
        }

        private void btnRefreshDatabaseLocationPath_Click(object sender, RoutedEventArgs e)
        {
            Configuration.RefreshDatabaseLocation();
            RefreshFolderLabels();
        }

        private void btnRefreshSyncFolderPath_Click(object sender, RoutedEventArgs e)
        {
            Configuration.RefreshSyncFolderLocation();
            RefreshFolderLabels();
        }

        private void btnSelectTrashFolderPath_Click(object sender, RoutedEventArgs e)
        {
            var newPath = UI.Prompt.ForFolder(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            if (!string.IsNullOrWhiteSpace(newPath))
            {
                Configuration.TrashFolder = newPath;
                RefreshFolderLabels();
            }
        }

        private void btnSelectIntakePdfsFolderPath_Click(object sender, RoutedEventArgs e)
        {
            var newPath = UI.Prompt.ForFolder(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            if (!string.IsNullOrWhiteSpace(newPath))
            {
                Configuration.IntakePdfsFolder = newPath;
                RefreshFolderLabels();
            }
        }

        private void btnSelectIntakeImagesFolderPath_Click(object sender, RoutedEventArgs e)
        {
            var newPath = UI.Prompt.ForFolder(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            if (!string.IsNullOrWhiteSpace(newPath))
            {
                Configuration.IntakeImagesFolder = newPath;
                RefreshFolderLabels();
            }
        }

        private void btnSelectIntakeVoicememosFolderPath_Click(object sender, RoutedEventArgs e)
        {
            var newPath = UI.Prompt.ForFolder(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            if (!string.IsNullOrWhiteSpace(newPath))
            {
                Configuration.IntakeVoiceMemosFolder = newPath;
                RefreshFolderLabels();
            }
        }

        #endregion

        #region private helper methods

        private void RefreshFolderLabels()
        {
            //database location
            lblDatabaseLocationPath.Content = Configuration.DatabaseLocationFolder;

            //sync folder
            lblSyncFolderPath.Content = Configuration.SyncFolder;

            //trash folder
            lblTrashFolderPath.Content = Configuration.TrashFolder;

            //intake images
            lblIntakeImagesFolderPath.Content = Configuration.IntakeImagesFolder;

            //intake pdfs
            lblIntakePdfsFolderPath.Content = Configuration.IntakePdfsFolder;

            //intake voicememos
            lblIntakeVoiceMemosFolderPath.Content = Configuration.IntakeVoiceMemosFolder;
        }

        private void RefreshFolderList()
        {
            //mimic tagged media tag list view (which uses tag counts)
            lvFoundFolders.ItemsSource = null;
            lvFoundFolders.ItemsSource = Configuration.GetEcosystemFolderCounts();
        }

        #endregion

    }
}
