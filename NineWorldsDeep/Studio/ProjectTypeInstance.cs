using System;

namespace NineWorldsDeep.Studio
{
    public abstract class ProjectTypeInstance
    {
        public abstract void PreProcessing();
        public abstract void Process_Exited();

        public void Process_Exited(object sender, EventArgs e)
        {
            //calls postprocessing specific to derived types
            Process_Exited();

            //TODO: add notes
        }
    }
}