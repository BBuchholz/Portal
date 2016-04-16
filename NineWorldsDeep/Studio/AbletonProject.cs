namespace NineWorldsDeep.Studio
{
    internal class AbletonProject : ProjectTypeInstance
    {
        private string filePath;

        public AbletonProject(string filePath)
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