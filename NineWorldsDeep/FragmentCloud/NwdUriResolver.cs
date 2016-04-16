namespace NineWorldsDeep.FragmentCloud
{
    public class NwdUriResolver
    {
        private static NwdUriResolver instance;

        private NwdUriResolver()
        {
            //singleton private constructor
        }

        public static NwdUriResolver GetInstance()
        {
            if (instance == null)
            {
                instance = new NwdUriResolver();
            }

            return instance;
        }

        public Fragment Resolve(string uri)
        {
            if (uri.StartsWith("NWD"))
            {
                return new FileSystemFragment(uri, true);
            }
            else
            {
                return new RootFragment();
            }
        }
    }
}