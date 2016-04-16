namespace NineWorldsDeep.FragmentCloud
{
    public class RootFragment : Fragment
    {
        public RootFragment()
            : base("")
        {
            AddChild(new FileSystemFragment("NWD", true));
            AddChild(new FileSystemFragment("NWD-AUX", true));
            AddChild(new FileSystemFragment("NWD-MEDIA", true));
            AddChild(new FileSystemFragment("NWD-SNDBX", true));
        }
    }
}