using NineWorldsDeep.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Mtp
{
    public static class Extensions
    {
        public static void ResetAll(this IEnumerable<NwdUri> ie)
        {
            foreach (NwdUri uri in ie)
            {
                uri.ResetStack();
            }
        }

        public static bool IsEmptyOrNull(this Stack<NwdUriProcessEntry> stack)
        {
            return stack == null || stack.Count < 1;
        }

        public static IEnumerable<NwdUriProcessEntry>
            Pop(this Stack<NwdUriProcessEntry> stack, int quantity)
        {
            List<NwdUriProcessEntry> lst =
                new List<NwdUriProcessEntry>();

            for (int i = 0; i < quantity; i++)
            {
                if (stack.Count > 0)
                {
                    lst.Add(stack.Pop());
                }
            }

            return lst;
        }

        public static IEnumerable<NwdUri> ToNwdUris(this IEnumerable<NwdUriProcessEntry> pes)
        {
            List<NwdUri> lst = new List<NwdUri>();

            foreach (NwdUriProcessEntry pe in pes)
            {
                lst.Add(pe.NwdUri);
            }

            return lst;
        }
    }
}
