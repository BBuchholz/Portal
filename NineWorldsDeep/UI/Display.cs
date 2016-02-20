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
        public static MessageBoxResult Grid(string message, IEnumerable itemsSource)
        {
            DisplayGridWindow dgw = new DisplayGridWindow();
            dgw.tbMessage.Text = message;
            dgw.ItemsSource = itemsSource;
            dgw.ShowDialog();

            return dgw.Result;
        }
    }
}
