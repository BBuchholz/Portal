namespace NineWorldsDeep.FragmentCloud
{
    public class FragmentClickedEventArgs
    {
        public FragmentClickedEventArgs(Tapestry.TapestryNode f)
        {
            Node = f;
        }

        public Tapestry.TapestryNode Node { get; private set; }
    }
}