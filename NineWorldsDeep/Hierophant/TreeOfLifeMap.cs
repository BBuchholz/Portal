using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hierophant
{
    public class TreeOfLifeMap : ISemanticallyRenderable
    {
        private Dictionary<SemanticKey, bool> keysToSelectionStatus =
            new Dictionary<SemanticKey, bool>();

        public void Render(ISemanticallyAddressable target)
        {
            foreach(SemanticKey semKey in target.SemanticKeys)
            {
                if (keysToSelectionStatus.ContainsKey(semKey) &&
                    keysToSelectionStatus[semKey] == true)
                {
                    target.Highlight(semKey);
                }
                else
                {
                    target.ClearHighlight(semKey);
                }
            }
        }

        public void Select(SemanticKey semKey)
        {
            if (!keysToSelectionStatus.ContainsKey(semKey))
            {
                keysToSelectionStatus.Add(semKey, true);
            }
            else
            {
                keysToSelectionStatus[semKey] = true;
            }
        }
    }
}
