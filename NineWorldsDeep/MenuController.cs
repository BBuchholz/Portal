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
        private Menu mainMenu;
        
        public void Configure(Menu mainMenu)
        {
            this.mainMenu = mainMenu;
        }
        
        public void AddMenuItem(string menuHeader, MenuItem mi)
        {
            EnsureMenu(menuHeader).Items.Add(mi);
        }

        public void AddMenuItem(string menuHeader, 
                                string header, 
                                RoutedEventHandler onClick)
        {
            MenuItem mi = new MenuItem();
            mi.Header = header;
            mi.Click += onClick;
            AddMenuItem(menuHeader, mi);
        }

        private MenuItem EnsureMenu(string header)
        {
            MenuItem found = null;

            foreach(MenuItem mi in mainMenu.Items)
            {
                if (mi.Header.Equals(header))
                {
                    found = mi;
                }
            }

            if(found == null)
            {
                found = new MenuItem();
                found.Header = header;
                mainMenu.Items.Add(found);
            }

            return found;
        }
    }
}
