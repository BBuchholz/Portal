using NineWorldsDeep.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Core
{
    public static class Extensions
    {
        public static void AddIdempotent<T>(this List<T> lst, T item)
        {
            bool found = false;

            foreach(T itm in lst)
            {
                if(itm is TapestryNode && item is TapestryNode)
                {
                    var a = itm as TapestryNode;
                    var b = item as TapestryNode;

                    if (a.Parallels(b))
                    {
                        found = true;
                    }
                }
                else
                {
                    found = lst.Contains(item);
                }

            }

            if (!found)
            {
                lst.Add(item);
            }
        }
    }
}
