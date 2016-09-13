using NineWorldsDeep.Tapestry.Nodes;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    public class ClusterDisplayRequestedEventArgs
    {
        public ClusterDisplayRequestedEventArgs(ClusterNode nd)
        {
            ClusterNode = nd;
        }

        public ClusterNode ClusterNode { get; private set; }
    }
}