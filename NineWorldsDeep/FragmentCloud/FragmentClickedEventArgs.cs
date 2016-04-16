namespace NineWorldsDeep.FragmentCloud
{
    public class FragmentClickedEventArgs
    {
        public FragmentClickedEventArgs(Fragment f)
        {
            Fragment = f;
        }

        public Fragment Fragment { get; private set; }
    }
}