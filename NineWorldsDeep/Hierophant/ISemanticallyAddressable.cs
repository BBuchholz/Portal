using System.Collections.Generic;

namespace NineWorldsDeep.Hierophant
{
    public interface ISemanticallyAddressable
    {
        IEnumerable<SemanticKey> SemanticKeys { get; }

        void Highlight(SemanticKey semanticKey);
        void ClearHighlight(SemanticKey semanticKey);
    }
}