using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NineWorldsDeep
{
    public class WorkbenchWindowController
    {
        private FragmentMetaWindow window;
        private List<Window> registeredWindows =
            new List<Window>();

        private static WorkbenchWindowController instance;

        private WorkbenchWindowController()
        {
            //singleton private constructor
        }

        public static WorkbenchWindowController Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new WorkbenchWindowController();
                }

                return instance;
            }
        }

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
            if (WindowIsNotWorkbench(w))
            {
                registeredWindows.Add(w);
            }
        }

        private void TargetWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CloseAppAfterDeRegistering(sender))
            {
                Application.Current.Shutdown();
            }            
        }

        private bool WindowIsNotWorkbench(Window w)
        {
            return !w.GetType().Equals(typeof(Workbench));
        }

        private bool CloseAppAfterDeRegistering(object sender)
        {
            Window w = (Window)sender;
                        
            if (WindowIsNotWorkbench(w))
            {
                registeredWindows.Remove(w);
            }
            
            if(registeredWindows.Count == 0)
            {
                return true;
            }
            
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
