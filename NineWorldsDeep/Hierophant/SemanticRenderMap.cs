using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hierophant
{
    public class SemanticRenderMap : ISemanticallyRenderable
    {
        #region fields

        private Dictionary<SemanticKey, bool> _KeysToSelectionStatus =
            new Dictionary<SemanticKey, bool>();

        #endregion

        #region public interface

        public void Render(ISemanticallyAddressable target)
        {
            foreach(SemanticKey semKey in target.AllSemanticKeys)
            {
                if (_KeysToSelectionStatus.ContainsKey(semKey) &&
                    _KeysToSelectionStatus[semKey] == true)
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
            if (!_KeysToSelectionStatus.ContainsKey(semKey))
            {
                _KeysToSelectionStatus.Add(semKey, true);
            }
            else
            {
                _KeysToSelectionStatus[semKey] = true;
            }
        }

        public IEnumerable<SemanticKey> AllKeys
        {
            get
            {
                return _KeysToSelectionStatus.Keys;
            }
        }

        public IEnumerable<SemanticKey> HighlightedKeys
        {
            get
            {
                return _KeysToSelectionStatus
                            .Where(x => x.Value == true)
                            .Select(x => x.Key);
            }
        }

        #endregion
    }
}
