using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep
{
    public interface IProgressBarCompatible
    {
        int ProgressBarMinimumValue { get; }
        int ProgressBarMaximumValue { get; }
        void RegisterBackgroundWorker(BackgroundWorker bw);
        string GetCompletedMessage();
    }
}
