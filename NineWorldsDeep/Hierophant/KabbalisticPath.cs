using System.Collections.Generic;
using System.Linq;
using System.Windows.Shapes;

namespace NineWorldsDeep.Hierophant
{
    public class KabbalisticPath
    {
        private Dictionary<string, Sephirah> _sephiroth =
            new Dictionary<string, Sephirah>();

        public string Description
        {
            get
            {
                return "Path connecting " + 
                    string.Join(", ", _sephiroth.Keys.ToList());
            }
        }
        public Line LineAbove { get; internal set; }
        public Line LineBelow { get; internal set; }
        public Line LineCenter { get; internal set; }

        public Dictionary<string, Sephirah> Sephiroth
        {
            get { return _sephiroth; }
        }
    }
}