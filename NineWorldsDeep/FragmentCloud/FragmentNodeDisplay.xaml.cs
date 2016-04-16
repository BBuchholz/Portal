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

        public void Display(Fragment frg)
        {
            txtMultiLine.Text = frg.ToMultiLineDetail();
        }
    }
}
