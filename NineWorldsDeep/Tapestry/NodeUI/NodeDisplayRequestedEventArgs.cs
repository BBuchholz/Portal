namespace NineWorldsDeep.Tapestry.NodeUI
{
    public class NodeDisplayRequestedEventArgs
    {
        public NodeDisplayRequestedEventArgs(TapestryNode nd)
        {
            TapestryNode = nd;
        }

        public TapestryNode TapestryNode { get; private set; }
    }
}