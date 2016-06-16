using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.FragmentCloud;

namespace NineWorldsDeep.Tapestry.Fragments
{
    public class StudioLyricsNode : FragmentCloud.TapestryNode
    {
        public StudioLyricsNode()
            : base("Studio/Lyrics")
        {
        }

        public override string GetShortName()
        {
            return "Studio Lyrics";
        }

        public override void PerformSelectionAction()
        {
            var window = new Studio.LyricMatrixGridTestHarness();
            window.Show();
        }
    }
}
