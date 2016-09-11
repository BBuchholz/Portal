using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.FragmentCloud;

namespace NineWorldsDeep.Tapestry.Fragments
{
    public class ArpaBetNode : TapestryNode
    {
        public ArpaBetNode()
            : base("ArpaBet")
        {
        }

        public override string GetShortName()
        {
            return "ArpaBet";
        }

        public override void PerformSelectionAction()
        {
            var window = new Studio.ArpaBetWindow();
            window.Show();
        }

        //just putting this here for "findable" reference:
        //CMU Pronouncing Dictionary
        //http://www.speech.cs.cmu.edu/cgi-bin/cmudict
        //http://cmusphinx.svn.sourceforge.net/svnroot/cmusphinx/trunk/cmudict/cmudict.0.7a
        //http://svn.code.sf.net/p/cmusphinx/code/trunk/cmudict/cmudict.0.7a
    }
}
