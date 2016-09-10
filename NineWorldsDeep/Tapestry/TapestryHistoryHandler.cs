using System.Windows.Controls;

namespace NineWorldsDeep.Tapestry
{
    public interface TapestryHistoryHandler
    {
        void PerformLoad(TapestryNodeViewControl originator, TapestryNode nd);
        void NavigateRoot();
    }
}