namespace NineWorldsDeep.Studio
{
    internal class FLStudioProject : ProjectTypeInstance
    {
        private string filePath;

        public FLStudioProject(string filePath)
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