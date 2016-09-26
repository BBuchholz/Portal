using System;
using NineWorldsDeep.Tapestry;

namespace NineWorldsDeep.FragmentCloud
{
    internal class ImageBrowserFragment : Tapestry.TapestryNode
    {
        public ImageBrowserFragment()
            : base("Images")
        {
        }

        public override string GetShortName()
        {
            return "Images";
        }

        public override bool Parallels(Tapestry.TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            var window = 
                new ImageBrowser.ImageBrowserMainWindow();
            window.Show();
        }
    }
}