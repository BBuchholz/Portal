using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Core
{
    public class ProcessableFragment : Fragment
    {
        private string processedKey;
        
        public ProcessableFragment(Fragment f, string processedKey)
            : base("FragmentType", "Processable")
        {
            this.processedKey = processedKey;
            Merge(f, ConflictMergeAction.OverwriteConflicts);
        }

        public bool IsProcessed()
        {
            string val = GetMeta(processedKey);
            return val != null && val.Equals("True");
        }

        public void SetProcessed()
        {
            SetMeta(processedKey, "True");
        }
    }
}
