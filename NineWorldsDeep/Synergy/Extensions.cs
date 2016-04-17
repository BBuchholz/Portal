using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Synergy
{
    public static class Extensions
    {
        public static List<StringListItem> ToListItems(this List<string> lst)
        {
            List<StringListItem> newLst = new List<StringListItem>();

            foreach (string s in lst)
            {
                newLst.Add(new StringListItem(s));
            }

            return newLst;
        }

        /// <summary>
        /// returns true if this status enumerable contains
        /// any Archived or Completed statuses
        /// </summary>
        /// <param name="ie"></param>
        /// <returns></returns>
        public static bool ContainsNonActiveStatuses(this IEnumerable<Status> ie)
        {
            bool found = false;

            foreach (Status s in ie)
            {
                if (s.StatusValue.Equals("Archived", StringComparison.CurrentCultureIgnoreCase) ||
                    s.StatusValue.Equals("Completed", StringComparison.CurrentCultureIgnoreCase))
                {
                    found = true;
                }
            }

            return found;
        }

        /// <summary>
        /// returns true if this ToDoItem enumerable contains
        /// only ToDoItems in an Archived or Completed status
        /// </summary>
        /// <param name="ie"></param>
        /// <returns></returns>
        public static bool ContainsOnlyInactiveStatuses(this IEnumerable<ToDoItem> ie)
        {
            bool containsOnlyInactiveStatuses = true;

            foreach (ToDoItem tdi in ie)
            {
                if (!tdi.Statuses.ContainsNonActiveStatuses())
                {
                    containsOnlyInactiveStatuses = false;
                }
            }

            return containsOnlyInactiveStatuses;
        }

        /// <summary>
        /// will match ANY status with matching StatusValue,
        /// ignoring StampedAt
        /// </summary>
        /// <param name="ie"></param>
        /// <param name="statusValue"></param>
        /// <returns></returns>
        public static bool
            ContainsStatus(this IEnumerable<Status> ie,
                                string statusValue)
        {
            bool found = false;

            foreach (Status s in ie)
            {
                if (found == false &&
                    s.StatusValue.Equals(statusValue,
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    found = true;
                }
            }

            return found;
        }

        /// <summary>
        /// will match ONLY a status with matching StatusValue,
        /// AND matching StampedAt fields.
        /// </summary>
        /// <param name="ie"></param>
        /// <param name="statusValue"></param>
        /// <returns></returns>
        public static bool
            ContainsStatus(this IEnumerable<Status> ie, Status status)
        {
            bool found = false;

            foreach (Status s in ie)
            {
                if (found == false &&
                    s.StatusValue.Equals(status.StatusValue,
                        StringComparison.CurrentCultureIgnoreCase) &&
                    s.StampedAt.Equals(status.StampedAt))
                {
                    found = true;
                }
            }

            return found;
        }

        /// <summary>
        /// will add to collection, ignoring if already exists
        /// (making all adds idempotent)
        /// </summary>
        /// <param name="col"></param>
        /// <param name="status"></param>
        public static void
            AddWithMerge(this ICollection<Status> col, Status status)
        {
            if (!col.ContainsStatus(status))
            {
                col.Add(status);
            }
        }

        public static void
            AddWithMerge(this ICollection<Status> col,
                              IEnumerable<Status> statuses)
        {
            foreach (Status s in statuses)
            {
                col.AddWithMerge(s);
            }
        }

        public static bool ContainsFragment(this IEnumerable<Fragment> ie, Fragment f)
        {
            bool found = false;

            foreach (Fragment frg in ie)
            {
                if (found == false &&
                    f.FragmentValue.Equals(frg.FragmentValue,
                        StringComparison.CurrentCultureIgnoreCase))
                {
                    found = true;
                }
            }

            return found;
        }

        public static void
            AddWithMerge(this ICollection<Fragment> col, Fragment f)
        {
            if (!col.ContainsFragment(f))
            {
                col.Add(f);
            }
        }

        public static void
            AddWithMerge(this ICollection<Fragment> col,
                              IEnumerable<Fragment> fragments)
        {
            foreach (Fragment f in fragments)
            {
                col.AddWithMerge(f);
            }
        }
    }
}
