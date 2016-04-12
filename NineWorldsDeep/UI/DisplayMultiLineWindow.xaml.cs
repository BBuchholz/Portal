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

namespace NineWorldsDeep.UI
{
    /// <summary>
    /// Interaction logic for DisplayMultiLineWindow.xaml
    /// </summary>
    public partial class DisplayMultiLineWindow : Window
    {
        public DisplayMultiLineWindow()
        {
            InitializeComponent();
        }

        public string Text
        {
            get
            {
                return txtOutput.Text;
            }

            internal set
            {
                txtOutput.Text = value;
            }
        }

        public string SummaryText
        {
            get
            {
                return tbMessage.Text;
            }

            internal set
            {
                tbMessage.Text = value;
            }
        }

        public MessageBoxResult Result { get; private set; }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }
    }
}
