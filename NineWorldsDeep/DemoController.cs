using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NineWorldsDeep
{
    public class DemoController : IProgressBarCompatible
    {
        private FragmentMetaWindow fmw;
        private BackgroundWorker bw; 

        public int ProgressBarMinimumValue
        {
            get
            {
                return 0;
            }
        }

        public int ProgressBarMaximumValue
        {
            get
            {
                return 100;
            }
        }

        public void Configure(FragmentMetaWindow fmw)
        {
            this.fmw = fmw;
            this.fmw.Menu.AddMenuItem("Demo", 
                                      "Load Demo Frags", 
                                      MenuItemDemo_Click);
            this.fmw.Menu.AddMenuItem("Demo",
                                      "Demo Progress Bar",
                                      DemoProgressBar);
        }

        private void DemoProgressBar(object sender, RoutedEventArgs e)
        {
            SimpleProgressBarWindow spbw = new SimpleProgressBarWindow();
            spbw.Configure(this, ProgressBarDemoDoWork);
            spbw.Show();
            spbw.Start();
        }

        private void ProgressBarDemoDoWork(object sender, DoWorkEventArgs e)
        {
            int min = ProgressBarMinimumValue;
            int max = ProgressBarMaximumValue;

            for (int i = min; i <= max; i++)
            {
                Thread.Sleep(100);
                bw.ReportProgress(i);
            }
        }

        private void MenuItemDemo_Click(object sender, RoutedEventArgs e)
        {
            DemoFragments();
            DemoDynamicMenus();
        }

        private void DemoFragments()
        {
            List<Fragment> lst = new List<Fragment>();

            for (int i = 1; i < 10; i++)
            {
                Fragment f = new Fragment("frg" + i);
                f.SetMeta("DemoKey", "demo value for frg" + i);

                if(i % 2 == 0)
                {                    
                    f.SetMeta("ConditionalMetaKey", "example " + i);
                }

                lst.Add(f);
            }

            fmw.Receive(lst);
        }

        private void DemoDynamicMenus()
        {
            fmw.Menu.AddMenuItem("Demo", "Dynamic Menu Item", Test_Click);
            fmw.Menu.AddMenuItem("Demo", "Prompt For Text File", Prompt_Click);
        }
        
        private void Test_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Testing");
        }

        private void Prompt_Click(object sender, RoutedEventArgs e)
        {
            string selected = Prompt.ForTextFile("C:\\Users");
            if(selected == null)
            {
                selected = "nothing selected";
            }
            MessageBox.Show(selected);
        }

        public void RegisterBackgroundWorker(BackgroundWorker bw)
        {
            this.bw = bw;
        }

        public string GetCompletedMessage()
        {
            return "progress bar demo complete";
        }
    }

    
}
