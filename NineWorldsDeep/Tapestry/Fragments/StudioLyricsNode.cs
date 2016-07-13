using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.FragmentCloud;

namespace NineWorldsDeep.Tapestry.Fragments
{
    public class StudioLyricsNode : TapestryNode
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

        //just putting this here for "findable" reference:
        //CMU Pronouncing Dictionary
        //http://www.speech.cs.cmu.edu/cgi-bin/cmudict
        //http://cmusphinx.svn.sourceforge.net/svnroot/cmusphinx/trunk/cmudict/cmudict.0.7a
        //http://svn.code.sf.net/p/cmusphinx/code/trunk/cmudict/cmudict.0.7a
    }
}
