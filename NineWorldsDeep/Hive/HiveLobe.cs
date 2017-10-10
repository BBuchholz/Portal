using System;
using NineWorldsDeep.Mnemosyne.V5;
using System.Collections.Generic;

namespace NineWorldsDeep.Hive
{
    public abstract class HiveLobe : IEquatable<HiveLobe>
    {
        public string HiveLobeName { private set; get; }
        public HiveRoot HiveRoot { private set; get; }
        protected List<HiveSpore> SporesInternal { private set; get; }
        public IEnumerable<HiveSpore> Spores { get { return SporesInternal; } }

        public HiveLobe(string name, HiveRoot hr)
        {
            this.HiveLobeName = name;
            this.HiveRoot = hr;
            this.SporesInternal = new List<HiveSpore>();
        }
        
        /// <summary>
        /// Any HiveSpore can be passed in as a parameter, 
        /// only those that fit this particular lobe will be added
        /// 
        /// (spores drift through, lobes collect what they are designed to)
        /// </summary>
        /// <param name="spore"></param>
        public virtual void Add(HiveSpore spore)
        {
            //TODO: validate, check for duplicates, &c.

            if (!SporesInternal.Contains(spore))
            {
                SporesInternal.Add(spore);
            }
        }

        public abstract void Collect();


        #region "equality"

        public bool Equals(HiveLobe other)
        {
            if (other == null) return false;

            return HiveLobeName.ToLower().Equals(other.HiveLobeName.ToLower());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as HiveLobe);
        }

        public override int GetHashCode()
        {
            return HiveLobeName.ToLower().GetHashCode();
        }

        public void ClearSpores()
        {
            SporesInternal.Clear();
        }

        #endregion
    }
}