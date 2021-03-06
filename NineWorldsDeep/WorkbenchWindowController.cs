﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NineWorldsDeep
{
    public class WorkbenchWindowController
    {
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
            ConfigureClosingEvent(w);
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
            return !w.GetType().Equals(typeof(WorkbenchWindow));
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
        

    }
}
