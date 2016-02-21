using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NineWorldsDeep.UI
{
    public class Display
    {
        public static MessageBoxResult Grid(string message, 
            IEnumerable itemsSource, IEnumerable secondItemsSource)
        {
            DisplayGridWindow dgw;
            if (secondItemsSource != null)
            {
                dgw = new DisplayGridWindow(secondItemsSource);
            }
            else
            {
                dgw = new DisplayGridWindow();
            }

            dgw.Message = message;
            dgw.ItemsSource = itemsSource;
            dgw.ShowDialog();

            return dgw.Result;
        }

        public static MessageBoxResult Grid(string message, IEnumerable itemsSource)            
        {
            return Grid(message, itemsSource, null);
        }
    }
}
