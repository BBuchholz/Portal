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
        private TextBlock tbDisplay;
        private TextBox txtEditor;

        private string lyricMatrixFolder = "C:\\NWD\\lyricBits";

        public LyricMatrixGrid()
        {
            InitializeComponent();
            lyricMatrix = new LyricMatrix();
        }

        public void RegisterDisplay(TextBlock tb)
        {
            tbDisplay = tb;
        }

        public void RegisterEditor(TextBox txt)
        {
            txtEditor = txt;
        }

        private string LyricMatrixFile
        {
            get
            {
                return lyricMatrixFolder + "\\lyricMatrix.xml";
            }
        }

        public void Load()
        {
            MessageBox.Show("load goes here");
        }

        public void Save()
        {
            MessageBox.Show("save goes here");
        }

        public void CreateNewLyricBit()
        {
            LyricBit lb = lyricMatrix.CreateNewLyricBit();
            RefreshDisplay();
            lvLyricBits.SelectedItem = lb;
        }

        public LyricBit Selected
        {
            get
            {
                return (LyricBit)lvLyricBits.SelectedItem;
            }

            set
            {
                lvLyricBits.SelectedItem = value;
            }
        }

        public void RefreshDisplay()
        {
            lvLyricBits.ItemsSource = null;
            lvLyricBits.ItemsSource = lyricMatrix.All();
        }

        private void lvLyricBits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Selected != null)
            {
                SetDisplayText(Selected.CurrentVersion);
                SetEditorText(Selected.CurrentVersion);
            }
            else
            {
                SetDisplayText("");
                SetEditorText("");
            }
        }

        private void SetDisplayText(string txt)
        {
            if (tbDisplay != null)
            {
                tbDisplay.Text = txt;
            }
        }

        private void SetEditorText(string txt)
        {
            if (txtEditor != null)
            {
                txtEditor.Text = txt;
            }
        }

    }
}
