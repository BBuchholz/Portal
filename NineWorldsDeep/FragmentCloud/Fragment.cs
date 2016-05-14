using System.Collections.Generic;
using System.Linq;

namespace NineWorldsDeep.FragmentCloud
{
    public class Fragment
    {
        private Dictionary<string, Fragment> uriToChildFragments =
           new Dictionary<string, Fragment>();

        public string URI { get; private set; }

        public IEnumerable<Fragment> Children
        {
            get
            {
                return GetChildren();
            }
        }

        protected virtual IEnumerable<Fragment> GetChildren()
        {
            return uriToChildFragments.Values;
        }

        public Fragment(string uri, params Fragment[] children)
        {
            this.URI = Converter.SanitizeUri(uri);
            foreach (Fragment frg in children)
            {
                AddChild(frg);
            }
        }

        public virtual string ToMultiLineDetail()
        {
            string detail = Converter.NwdUriNodeName(URI) +
                System.Environment.NewLine;
            detail += "Child Count: " + Children.Count() +
                System.Environment.NewLine;

            return detail;
        }

        public virtual string GetShortName()
        {
            int shortNameLength = 15;
            string name = Converter.NwdUriNodeName(URI);

            //HACK: to remove extension
            name = System.IO.Path.GetFileNameWithoutExtension(name);

            if (name.Length > shortNameLength)
            {
                name = name.Substring(0, shortNameLength - 6) + "..." +
                    name.Substring(name.Length - 3, 3);
            }

            return name;
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
        public bool AddChild(Fragment frg)
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
        public void SetChild(Fragment frg)
        {
            uriToChildFragments[frg.URI] = frg;
        }
    }
}