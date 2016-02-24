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

        public static void Exception(Exception ex)
        {
            //TODO: make this more dope eventually
            MessageBox.Show("Error: " + ex.Message);
        }

        public static void Message(string msg)
        {
            //TODO: we could make Display take different configurations
            // so that these methods would be handled differently 
            // in different cases (like this could be configured to 
            // display to a status bar
            // or a message box, but all code could call this method)

            MessageBox.Show(msg);
        }
    }
}
