﻿using System;
using NineWorldsDeep.Tapestry;

namespace NineWorldsDeep.FragmentCloud
{
    internal class HierophantNode : Tapestry.TapestryNode
    {
        public HierophantNode()
            : base("Hierophant/previous")
        {
        }

        public override string GetShortName()
        {
            return "Hierophant (previous)";
        }

        public override bool Parallels(Tapestry.TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            var window = new Hierophant.HierophantMainWindow();
            window.Show();
            UI.Utils.MinimizeMainWindow();
        }
    }
}