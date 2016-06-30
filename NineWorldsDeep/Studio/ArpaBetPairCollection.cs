using NineWorldsDeep.Warehouse;

namespace NineWorldsDeep.Studio
{
    //inheriting this way allows xml data binding
    //see: https://msdn.microsoft.com/en-us/library/ff407126(v=vs.100).aspx
    public class ArpaBetPairCollection : ObservableCollectionWithItemNotify<ArpaBetPair>
    {
    }
}