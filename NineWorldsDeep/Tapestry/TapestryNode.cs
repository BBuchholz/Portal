using NineWorldsDeep.FragmentCloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry
{

    public abstract class TapestryNode
    {
        private Dictionary<string, TapestryNode> uriToChildFragments =
           new Dictionary<string, TapestryNode>();

        public string URI { get; private set; }

        public IEnumerable<TapestryNode> Children
        {
            get
            {
                return GetChildren();
            }
        }

        public string ShortName
        {
            get
            {
                return GetShortName();
            }
        }

        public int ChildCount
        {
            get
            {
                return Children.Count();
            }
        }

        public string LongName
        {
            get
            {
                return GetLongName();
            }
        }

        public virtual TapestryNodeType NodeType
        {
            get
            {
                if (Children.Count() > 0)
                {
                    return TapestryNodeType.Collection;
                }
                else
                {
                    return TapestryNodeType.SingleNodeDefault;
                }
            }
        }

        protected virtual IEnumerable<TapestryNode> GetChildren()
        {
            return uriToChildFragments.Values;
        }

        public TapestryNode(string uri, params TapestryNode[] children)
        {
            this.URI = Converter.SanitizeUri(uri);
            foreach (TapestryNode frg in children)
            {
                AddChild(frg);
            }
        }

        public abstract void PerformSelectionAction();

        public virtual string ToMultiLineDetail()
        {
            string detail = Converter.NwdUriNodeName(URI) +
                System.Environment.NewLine;
            detail += "Child Count: " + Children.Count() +
                System.Environment.NewLine;

            return detail;
        }

        public abstract string GetShortName();

        public virtual string GetLongName()
        {
            //Default
            return GetShortName();
        }

        public void ClearChildren()
        {
            uriToChildFragments.Clear();
        }

        /// <summary>
        /// Will attempt to add the fragment as a child of this fragment
        /// keyed to the uri of the given fragment. If a fragment
        /// is already keyed to the given uri, the fragment will
        /// not be added and the method will return false (use SetChild())
        /// </summary>
        /// <param name="frg">the fragment to add as child to this fragment</param>
        /// <returns>true if Add was successful, false if not</returns>
        public bool AddChild(TapestryNode frg)
        {
            if (!uriToChildFragments.ContainsKey(frg.URI))
            {
                uriToChildFragments.Add(frg.URI, frg);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Will add specified fragment as a child of this fragment
        /// overwriting any existing fragment keyed to the 
        /// same URI as the given fragment
        /// </summary>
        /// <param name="frg">the fragment to add as child, overwriting URI key if necessary</param>
        public void SetChild(TapestryNode frg)
        {
            uriToChildFragments[frg.URI] = frg;
        }

        public override string ToString()
        {
            return GetShortName();
        }
    }
}
