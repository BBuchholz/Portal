namespace NineWorldsDeep.Studio
{
    internal class CubaseProject : ProjectTypeInstance
    {
        private string filePath;

        public CubaseProject(string filePath)
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