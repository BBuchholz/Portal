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

namespace NineWorldsDeep.Studio
{
    /// <summary>
    /// Interaction logic for LyricMatrixGrid.xaml
    /// </summary>
    public partial class LyricMatrixGrid : UserControl
    {
        private LyricMatrix lyricMatrix;
        
        public LyricMatrixGrid()
        {
            InitializeComponent();
            lyricMatrix = new LyricMatrix();
        }

        public void Load()
        {
            MessageBox.Show("load goes here");
        }

        public void Save()
        {
            MessageBox.Show("save goes here");
        }






    }
}
