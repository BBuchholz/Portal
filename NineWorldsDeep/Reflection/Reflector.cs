using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NineWorldsDeep.Reflection
{
    public class Reflector
    {
        //private string @namespace = "NineWorldsDeep";
        private List<string> namespaces = new List<string>();

        public Reflector()
        {
            namespaces.Add("NineWorldsDeep");
            namespaces.Add("NineWorldsDeep.Reflection");
        }

        public IEnumerable<Type> Reflection1()
        {
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && 
                          namespaces.Contains(t.Namespace) && 
                          t.IsPublic && 
                          !t.IsSubclassOf(typeof(Application))
                    select t;
            return q.ToList();
        }

        public IEnumerable<Type> Reflection2()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                                          .SelectMany(t => t.GetTypes())
                                          .Where(t => t.IsClass && namespaces.Contains(t.Namespace) &&
                                                      t.IsPublic && !t.IsSubclassOf(typeof(Application)))
                                          .ToList();
        }
    }
}
