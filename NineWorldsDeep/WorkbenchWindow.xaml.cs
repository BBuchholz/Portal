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
    public partial class WorkbenchWindow : Window
    {
        private MenuController _menu;
        private WorkbenchListViewsController _controller;

        //singleton implementation
        private static WorkbenchWindow instance;

        //singleton implementation
        public static WorkbenchWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WorkbenchWindow();
                }
                
                return instance;

            }
        }

        //singleton implementation
        private WorkbenchWindow()
        {
            InitializeComponent();
            _controller = new WorkbenchListViewsController();
            _controller.Configure(mainGrid);
            _menu = new MenuController();
            _menu.Configure(mainMenu);
            this.Closing += Workbench_Closing;
            WorkbenchWindowController.Instance.ConfigureClosingEvent(this);
            new WorkbenchMenuController().Configure(this, _controller);
        }
        
        //hides window instead of closing it for singleton implementation    
        private void Workbench_Closing(object sender, 
                                       System.ComponentModel.CancelEventArgs e)
        {        
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;            
        }

        public MenuController Menu {  get { return _menu; } }

        public void Receive(IEnumerable<Fragment> ie)
        {
            _controller.AddListView(ie);
        }

        public void ReceiveFirst(IEnumerable<Fragment> ie)
        {
            _controller.AddListViewFirst(ie);
        }
    }
}
