using NineWorldsDeep.Tapestry.Nodes;
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

namespace NineWorldsDeep.FragmentCloud
{
    /// <summary>
    /// Interaction logic for FragmentNodeDisplay.xaml
    /// </summary>
    public partial class FragmentNodeDisplay : UserControl
    {
        public FragmentNodeDisplay()
        {
            InitializeComponent();
        }

        public void Display(Tapestry.TapestryNode frg)
        {
            if(frg is FileSystemNode)
            {
                FileDetailsControl.Visibility = Visibility.Visible;
                MultiLineTextBox.Visibility = Visibility.Collapsed;
                //FileDetailsControl.Display((FileSystemNode)frg);
            }
            else
            {
                MultiLineTextBox.Visibility = Visibility.Visible;
                FileDetailsControl.Visibility = Visibility.Collapsed;
                MultiLineTextBox.Text = frg.ToMultiLineDetail();
            }

            FileDetailsControl.Display((FileSystemNode)frg);

            frg.PerformSelectionAction();
        }
    }
}
