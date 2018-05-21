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

namespace NineWorldsDeep.Tapestry
{
    /// <summary>
    /// Interaction logic for TapestrySplitViewWindow.xaml
    /// </summary>
    public partial class TapestrySplitViewWindow : Window
    {
        private Dictionary<string, MenuItem> headerNameToBreadcrumbMenuItems;
        private Dictionary<MenuItem, CommandItemCoupling> menuItemToCommandCoupling; 

        public TapestrySplitViewWindow()
        {
            InitializeComponent();
            UI.Utils.MainWindow = this;
            TapestryRegistry.MainWindow = this;
            headerNameToBreadcrumbMenuItems = new Dictionary<string, MenuItem>();
            menuItemToCommandCoupling = new Dictionary<MenuItem, CommandItemCoupling>();
        }

        private void MenuItemNavigateRoot_Click(object sender, RoutedEventArgs e)
        {
            ctrlNodeSplitView.NavigateRoot();
            ClearBreadcrumbs();
        }

        private void ClearBreadcrumbs()
        {            
            foreach(MenuItem mi in headerNameToBreadcrumbMenuItems.Values)
            {
                if (mainMenu.Items.Contains(mi))
                {
                    mainMenu.Items.Remove(mi);
                }
            }

            headerNameToBreadcrumbMenuItems.Clear();
            menuItemToCommandCoupling.Clear();          
        }

        private void MenuItem_GlobalLoadLocalCheckChanged(object sender, RoutedEventArgs e)
        {
            TapestryRegistry.GlobalLoadLocal = chkGlobalLoadLocal.IsChecked;
            TapestryRegistry.MainWindow = this;
        }



        //private void AddBreadCrumbNode(TapestryNode nd)
        //{
        //    if (!headerNameToBreadcrumbMenuItems.ContainsKey(nd.ShortName))
        //    {
        //        MenuItem menuItem = new MenuItem();
        //        menuItem.Header = nd.ShortName;
        //        mainMenu.Items.Add(menuItem);
        //        headerNameToBreadcrumbMenuItems[nd.ShortName] = menuItem;
        //    }
        //}

        //public void AddBreadCrumb(TapestrySplitViewLoadCommand lc)
        //{
        //    if (lc.LeftNode != null)
        //    {
        //        TapestryRegistry.MainWindow.AddBreadCrumbNode(lc.LeftNode);
        //    }

        //    if (lc.RightNode != null)
        //    {
        //        TapestryRegistry.MainWindow.AddBreadCrumbNode(lc.RightNode);
        //    }
        //}

        public void AddBreadCrumb(TapestrySplitViewLoadCommand lc)
        {
            CommandItemCoupling coupling = new CommandItemCoupling(lc);

            //if (!headerNameToBreadcrumbMenuItems.ContainsKey(coupling.Header))
            //{
            //    MenuItem menuItem = new MenuItem();
            //    menuItem.Header = coupling.Header;
            //    mainMenu.Items.Add(menuItem);
            //    menuItem.Click += LoadCommand_Click;
            //    headerNameToBreadcrumbMenuItems[coupling.Header] = menuItem;
            //    menuItemToCommandCoupling[menuItem] = coupling;
            //}

            //MODIFYING TO REPLACE EXISTING WITH LAST NAVIGATED LOAD COMMAND 
            
            MenuItem menuItem = new MenuItem();
            menuItem.Header = coupling.Header;
            menuItem.Click += LoadCommand_Click;

            //remove existing item with same header/key
            foreach (MenuItem mi in headerNameToBreadcrumbMenuItems.Values)
            {
                if ((mi.Header as string).Equals((menuItem.Header as string), StringComparison.CurrentCultureIgnoreCase))
                {
                    mainMenu.Items.Remove(mi);
                }
            }

            mainMenu.Items.Add(menuItem);

            headerNameToBreadcrumbMenuItems[coupling.Header] = menuItem;
            menuItemToCommandCoupling[menuItem] = coupling;
            
        }

        private void LoadCommand_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            if(mi != null)
            {
                //UI.Display.Message(mi.Header as string);
                if(TapestryRegistry.SplitViewNodeControl != null &&
                    menuItemToCommandCoupling.ContainsKey(mi))
                {
                    TapestryRegistry.SplitViewNodeControl.ResolveLoadCommand(
                        menuItemToCommandCoupling[mi].Command);
                }
            }
        }

        private class CommandItemCoupling
        {
            public TapestrySplitViewLoadCommand Command { get; private set; }
            public string Header { get; private set; }

            public CommandItemCoupling(TapestrySplitViewLoadCommand lc)
            {
                Header = lc.LeftNode.ShortName + "|" + lc.RightNode;
                Command = lc;
            }
        }

        private void MenuItem_ShowV6MainWindow(object sender, RoutedEventArgs e)
        {
            // when this becomes the top level app window
            // this should go in the App.xaml.cs file, OnStartup() method
            // see demo project 
            // demo project article: https://msdn.microsoft.com/en-us/magazine/dd419663.aspx
            // demo project code: https://github.com/djangojazz/JoshSmith_MVVMDemo

            V6Core.View.V6MainWindow window = new V6Core.View.V6MainWindow();

            // Create the ViewModel to which 
            // the main window binds.
            var viewModel = new V6Core.ViewModel.MainWindowViewModel();

            // When the ViewModel asks to be closed, 
            // close the window.
            EventHandler handler = null;
            handler = delegate
            {
                viewModel.RequestClose -= handler;
                window.Close();
            };
            viewModel.RequestClose += handler;

            // Allow all controls in the window to 
            // bind to the ViewModel by setting the 
            // DataContext, which propagates down 
            // the element tree.
            window.DataContext = viewModel;

            window.Show();
        }
    }

    
}
