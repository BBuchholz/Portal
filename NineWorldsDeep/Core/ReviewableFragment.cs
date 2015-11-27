using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Core
{
    public class ReviewableFragment : Fragment
    {
        private const string flag_verify_key = "FlagForVerification";

        public ReviewableFragment(Fragment f)
            : base("FragmentType", "Reviewable")
        {
            Merge(f, ConflictMergeAction.OverwriteConflicts);
        }

        public string KeyToReview
        {
            get
            {
                return GetMeta("KeyToReview");
            }
            private set
            {
                SetMeta("KeyToReview", value);
            }
        }

        public string KeyToSetOnReview
        {
            get
            {
                return GetMeta("KeyToSetOnReview");
            }
            private set
            {
                SetMeta("KeyToSetOnReview", value);
            }
        }

        public string ValueToSetOnReview
        {
            get
            {
                return GetMeta("ValueToSetOnReview");
            }
            private set
            {
                SetMeta("ValueToSetOnReview", value);
            }
        }

        public bool IsFlagged
        {
            get
            {
                string val = GetMeta(flag_verify_key);
                return val != null && val.Equals("True");
            }
            private set
            {
                SetMeta(flag_verify_key, value.ToString());
            }
        }

        public void FlagForReview(string keyToReview,
                                  string keyToSetOnReview,
                                  string valueToSetOnReview)
        {
            IsFlagged = true;
            KeyToReview = keyToReview;
            KeyToSetOnReview = keyToSetOnReview;
            ValueToSetOnReview = valueToSetOnReview;
        }

        public void ProcessReviewed()
        {
            IsFlagged = false;
            RemoveMeta("KeyToReview");
            SetMeta(KeyToSetOnReview, ValueToSetOnReview);
            RemoveMeta("KeyToSetOnReview");
            RemoveMeta("ValueToSetOnReview");
        }
    }
}
