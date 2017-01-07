using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NineWorldsDeep.UI
{
    public class Utils
    {
        public static Window MainWindow { get; set; }

        public static void MinimizeMainWindow()
        {
            if(MainWindow != null)
            {
                MainWindow.WindowState = WindowState.Minimized;
            }
        }
        
        public static void MaximizeMainWindow()
        {
            if(MainWindow != null)
            {
                MainWindow.WindowState = WindowState.Maximized;
            }
        }
    }
}
