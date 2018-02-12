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

        #endregion

        #region private helper methods

        private void RefreshFolderLabels()
        {
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
