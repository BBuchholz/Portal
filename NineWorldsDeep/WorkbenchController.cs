using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NineWorldsDeep
{
    public class WorkbenchController
    {
        private FragmentMetaWindow window;

        public void Configure(FragmentMetaWindow w)
        {
            window = w;
            window.Menu.AddMenuItem("Workbench",
                                    "Send To Workbench",
                                    SendToWorkbench);
            ConfigureClosingEvent(window);
        }

        public void ConfigureClosingEvent(Window w)
        {
            w.Closing += TargetWindowClosing;
        }

        private void TargetWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CheckOpenWindowsForClose(sender))
            {
                Application.Current.Shutdown();
            }
            
        }

        //just for testing purposes
        private bool CheckOpenWindowsForClose(object sender)
        {
            //testing to determine if explicit application shutdown should be called
            //for when workbench is hidden but all other windows close.
            string msg = "";
            bool testing = false; //set this to true for testing

            if (testing) { 
                msg += "sender: " + sender.GetType().Name + Environment.NewLine;
                msg += "windows open: " + Environment.NewLine;
            }

            bool senderFound = false;
            bool workbenchFound = false;
            string senderName = sender.GetType().Name;
            string workbenchName = typeof(Workbench).Name;
            int windowCount = Application.Current.Windows.Count;

            foreach (Window w in Application.Current.Windows)
            {
                if(testing)
                    msg += w.GetType().Name + Environment.NewLine;

                if(w.GetType().Name.Equals(senderName))
                    senderFound = true;

                if (w.GetType().Name.Equals(workbenchName))
                    workbenchFound = true;
            }

            if(testing)
                MessageBox.Show(msg);

            //just sender and workbench open, close app
            if (senderFound && workbenchFound && 
                windowCount == 2 && !senderName.Equals(workbenchName))
                return true;

            //just sender open, close app
            if (senderFound && windowCount == 1)
                return true;
            
            //more windows are open than just sender and workbench, keep open
            return false;
            
        }

        private void SendToWorkbench(object sender, RoutedEventArgs e)
        {
            Workbench w = Workbench.Instance;
            w.Receive(window.GetFragments());
            w.Show();
        }
    }
}
