namespace NineWorldsDeep.Studio
{
    internal class VisualStudioProject : ProjectTypeInstance
    {
        private string filePath;

        public VisualStudioProject(string filePath)
        {
            this.filePath = filePath;
        }

        public override void PreProcessing()
        {

        }

        public override void Process_Exited()
        {

        }
    }
}