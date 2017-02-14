using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NineWorldsDeep.Hierophant
{
    public class TreePath : HierophantVertex
    {
        public TreePath(string nameId) : base(nameId)
        {
        }

        private Dictionary<string, Sephirah> _sephiroth =
            new Dictionary<string, Sephirah>();
        
        public Dictionary<string, Sephirah> Sephiroth
        {
            get { return _sephiroth; }
        }

        public override string Details()
        {
            return "Path connecting " +
                string.Join(", ", _sephiroth.Keys.ToList());
        }
    }
}
