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

namespace NineWorldsDeep
{
    /// <summary>
    /// Interaction logic for Workbench.xaml
    /// </summary>
    public partial class Workbench : Window
    {
        //singleton implementation
        private static Workbench instance;

        //singleton implementation
        public static Workbench Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Workbench();
                }
                
                return instance;

            }
        }

        //singleton implementation
        private Workbench()
        {
            InitializeComponent();
            this.Closing += Workbench_Closing;
            new WorkbenchController().ConfigureClosingEvent(this);
        }
        
        //hides window instead of closing it for singleton implementation    
        private void Workbench_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {        
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;            
        }

        public void Receive(IEnumerable<Fragment> ie)
        {
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            MessageBox.Show("received " + ie.Count() + " fragments");

            //just for testing of dynamic grid manipulation
            int cols = mainGrid.ColumnDefinitions.Count();
            MessageBox.Show("cols: " + cols);
        }

    }
}
