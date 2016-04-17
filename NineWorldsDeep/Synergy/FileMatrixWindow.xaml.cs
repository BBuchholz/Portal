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

namespace NineWorldsDeep.Synergy
{
    /// <summary>
    /// Interaction logic for FileMatrixWindow.xaml
    /// </summary>
    public partial class FileMatrixWindow : Window
    {
        public FileMatrixWindow()
        {
            InitializeComponent();
            MenuController menu = new MenuController();
            menu.Configure(mainMenu);
            //FileMatrixDbAdapter db =
            //    new FileMatrixDbAdapter(
            //        ConnectionStrings.ByName("MySqlConnectionString"));
            //new FileMatrixMenuController().Configure(menu, db, dataGrid, statusBar);
        }
    }
}
