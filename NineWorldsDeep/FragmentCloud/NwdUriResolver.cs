using NineWorldsDeep.Tapestry.Nodes;

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

        //public Tapestry.TapestryNode Resolve(string uri)
        //{
        //    if (uri.StartsWith("NWD", 
        //        System.StringComparison.CurrentCultureIgnoreCase))
        //    {
        //        return new FileSystemNode(uri, true);
        //    }
        //    else if (uri.StartsWith("Synergy",
        //        System.StringComparison.CurrentCultureIgnoreCase))
        //    {
        //        return new SynergyFragment();
        //    }
        //    else if (uri.StartsWith("Studio",
        //        System.StringComparison.CurrentCultureIgnoreCase))
        //    {
        //        return new StudioNode();
        //    }
        //    else if (uri.StartsWith("Hierophant",
        //        System.StringComparison.CurrentCultureIgnoreCase))
        //    {
        //        return new HierophantNode();
        //    }
        //    else if (uri.StartsWith("WareHouse",
        //        System.StringComparison.CurrentCultureIgnoreCase))
        //    {
        //        return new WareHouseFragment();
        //    }
        //    else if (uri.StartsWith("Audio",
        //        System.StringComparison.CurrentCultureIgnoreCase))
        //    {
        //        return new AudioBrowserFragment();
        //    }
        //    else if (uri.StartsWith("Images",
        //        System.StringComparison.CurrentCultureIgnoreCase))
        //    {
        //        return new ImageBrowserFragment();
        //    }
        //    else if (uri.StartsWith("Mtp",
        //        System.StringComparison.CurrentCultureIgnoreCase))
        //    {
        //        return new MtpFragment();
        //    }
        //    else
        //    {
        //        return new RootNode();
        //    }
        //}
    }
}