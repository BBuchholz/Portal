﻿using System;

namespace NineWorldsDeep.FragmentCloud
{
    internal class ImageBrowserFragment : Fragment
    {
        public ImageBrowserFragment()
            : base("Images")
        {
        }

        public override string GetShortName()
        {
            return "Images";
        }

        public override void PerformSelectionAction()
        {
            var window = 
                new ImageBrowser.ImageBrowserMainWindow();
            window.Show();
        }
    }
}