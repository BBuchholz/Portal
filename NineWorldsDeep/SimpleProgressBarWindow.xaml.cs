using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NineWorldsDeep
{
    /// <summary>
    /// Interaction logic for SimpleProgressBarWindow.xaml
    /// </summary>
    public partial class SimpleProgressBarWindow : Window
    {//TODO: LICENSE NOTES
        //adapted from: http://stackoverflow.com/questions/19334583/how-to-correctly-implement-a-backgroundworker-with-progressbar-updates

        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private IProgressBarCompatible ipc;

        public SimpleProgressBarWindow()
        {
            InitializeComponent();
        }

        public void Configure(IProgressBarCompatible ipc, DoWorkEventHandler doWork)
        {
            this.ipc = ipc;

            ipc.RegisterBackgroundWorker(backgroundWorker);

            progressBar.Minimum = ipc.ProgressBarMinimumValue;
            progressBar.Maximum = ipc.ProgressBarMaximumValue;
            
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += doWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        public void Start()
        {
            backgroundWorker.RunWorkerAsync();
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // This is called on the UI thread when ReportProgress method is called
            progressBar.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // This is called on the UI thread when the DoWork method completes
            // so it's a good place to hide busy indicators, or put clean up code
            MessageBox.Show(ipc.GetCompletedMessage());
            this.Close();
        }
    }
}
