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

        #endregion
    }
}
