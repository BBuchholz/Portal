namespace NineWorldsDeep.FragmentCloud
{
    public class FragmentClickedEventArgs
    {
        public FragmentClickedEventArgs(TapestryNode f)
        {
            Fragment = f;
        }

        public TapestryNode Fragment { get; private set; }
    }
}