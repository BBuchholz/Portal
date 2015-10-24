using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NineWorldsDeep
{
    public class MenuController
    {
        private MenuItem menuItemOptions;

        public void Configure(MenuItem optionsMenu)
        {
            menuItemOptions = optionsMenu;
        }

        public void AddMenuItem(MenuItem mi)
        {
            menuItemOptions.Items.Add(mi);
        }

        public void AddMenuItem(string header, RoutedEventHandler onClick)
        {
            MenuItem mi = new MenuItem();
            mi.Header = header;
            mi.Click += onClick;
            AddMenuItem(mi);
        }
    }
}
