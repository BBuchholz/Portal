using System.Collections.Generic;

namespace NineWorldsDeep.Hierophant
{
    public interface ISemanticallyAddressable
    {
        IEnumerable<SemanticKey> AllSemanticKeys { get; }
        IEnumerable<SemanticKey> HighlightedSemanticKeys { get; }

        void Highlight(SemanticKey semanticKey);
        void ClearHighlight(SemanticKey semanticKey);
        void Display(ISemanticallyRenderable semanticRenderMap);
    }
}