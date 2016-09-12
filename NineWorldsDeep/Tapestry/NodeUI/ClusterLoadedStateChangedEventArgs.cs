using NineWorldsDeep.Tapestry.Nodes;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    public class ClusterLoadedStateChangedEventArgs
    {
        public ClusterLoadedStateChangedEventArgs(ClusterNode nd)
        {
            ClusterNode = nd;
        }

        public ClusterNode ClusterNode { get; private set; }
    }
}