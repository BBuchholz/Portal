using System;
using System.Collections;
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
    /// Interaction logic for DisplayGridWindow.xaml
    /// </summary>
    public partial class DisplayGridWindow : Window
    {
        private IEnumerable secondItemsSource;

        public DisplayGridWindow()
        {
            InitializeComponent();
        }

        public DisplayGridWindow(IEnumerable secondItemsSource)
            : this()
        {
            this.secondItemsSource = secondItemsSource;
            btnShowSecondList.Visibility = Visibility.Visible;            
        }

        public MessageBoxResult Result { get; private set; }

        public IEnumerable ItemsSource
        {
            get
            {
                return dataGrid.ItemsSource;
            }

            set
            {
                dataGrid.ItemsSource = value;
            }
        }

        public string Message
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

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();                
        }

        private void ButtonShowSecondList_Click(object sender, RoutedEventArgs e)
        {
            if(secondItemsSource != null)
            {
                Display.Grid(Message, secondItemsSource);
            }
        }
    }
}
