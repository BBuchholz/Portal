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
using System.Windows.Shapes;

namespace NineWorldsDeep.Studio
{
    /// <summary>
    /// Interaction logic for LyricMatrixGridTestHarness.xaml
    /// </summary>
    public partial class LyricMatrixGridTestHarness : Window
    {

        public LyricMatrixGridTestHarness()
        {
            InitializeComponent();
            lyricMatrixGrid.RegisterDisplay(tbSelectedLyric);
            lyricMatrixGrid.RegisterEditor(txtLyricEdit);
        }

        private void MenuItemLoad_Click(object sender, RoutedEventArgs e)
        {
            lyricMatrixGrid.Load();
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            lyricMatrixGrid.Save();
        }

        private void MenuItemNewLyricBit_Click(object sender, RoutedEventArgs e)
        {
            lyricMatrixGrid.CreateNewLyricBit();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (lyricMatrixGrid.Selected != null)
            {
                lyricMatrixGrid.Selected.Update(txtLyricEdit.Text);
                lyricMatrixGrid.RefreshDisplay();
            }
        }
    }
}
