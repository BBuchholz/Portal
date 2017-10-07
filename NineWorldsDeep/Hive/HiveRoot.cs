using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hive
{
    public class HiveRoot : IEquatable<HiveRoot>
    {
        protected List<HiveLobe> LobesInternal { private set; get; }
        public string HiveRootName { get; set; }
        public int HiveRootId { get; set; }

        public IEnumerable<HiveLobe> Lobes { get { return LobesInternal; } }

        public DateTime? HiveRootActivatedAt { get; private set; }
        public DateTime? HiveRootDeactivatedAt { get; private set; }

        public HiveRoot()
        {
            LobesInternal = new List<HiveLobe>();
        }

        public void Add(HiveLobe lobe)
        {
            //TODO: configure/validate/&c.

            if (!LobesInternal.Contains(lobe))
            {
                LobesInternal.Add(lobe);
            }            
        }

        public override string ToString()
        {
            return HiveRootName;
        }

        public void ClearLobes()
        {
            LobesInternal.Clear();
        }

        /// <summary>
        /// 
        /// will resolve conflicts, newest date will always take precedence
        /// passing null values allowed as well to just set one or two
        /// null values always resolve to the non-null value(unless both null)
        /// 
        /// NOTE: THIS CONVERTS TO UTC, WHICH IS DEPENDENT ON DateTime.Kind
        /// This property defaults to local, so if you are passing a UTC time, make
        /// sure that the Kind is set to UTC or the conversion will
        /// be off. 
        /// 
        /// </summary>
        /// <param name="newActivatedAt"></param>
        /// <param name="newDeactivatedAt"></param>
        public void SetTimeStamps(DateTime? newActivatedAt,
                                  DateTime? newDeactivatedAt)
        {
            if (newActivatedAt != null)
            {
                if (newActivatedAt.Value.Kind != DateTimeKind.Utc)
                {
                    newActivatedAt = newActivatedAt.Value.ToUniversalTime();
                }

                if (HiveRootActivatedAt == null ||
                   DateTime.Compare(HiveRootActivatedAt.Value, newActivatedAt.Value) < 0)
                {
                    //HiveRootActivatedAt is older or null
                    HiveRootActivatedAt = newActivatedAt;
                }
            }

            if (newDeactivatedAt != null)
            {
                if (newDeactivatedAt.Value.Kind != DateTimeKind.Utc)
                {
                    newDeactivatedAt = newDeactivatedAt.Value.ToUniversalTime();
                }

                if (HiveRootDeactivatedAt == null ||
                   DateTime.Compare(HiveRootDeactivatedAt.Value, newDeactivatedAt.Value) < 0)
                {
                    //HiveRootDeactivatedAt is older or null
                    HiveRootDeactivatedAt = newDeactivatedAt;
                }
            }

        }

        public bool IsActive()
        {
            if(HiveRootDeactivatedAt == null)
            {
                return true;
            }

            if(HiveRootActivatedAt == null)
            {
                //if we reach here, deactivated isn't null
                return false;
            }

            return DateTime.Compare(HiveRootDeactivatedAt.Value, HiveRootActivatedAt.Value) < 0;
        }

        public void Deactivate()
        {
            SetTimeStamps(null, TimeStamp.NowUTC());
        }

        public void Activate()
        {
            SetTimeStamps(TimeStamp.NowUTC(), null);
        }


        #region "equality"

        public bool Equals(HiveRoot other)
        {
            if (other == null) return false;

            return HiveRootName.ToLower().Equals(other.HiveRootName.ToLower());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as HiveRoot);
        }

        public override int GetHashCode()
        {
            return HiveRootName.ToLower().GetHashCode();
        }

        #endregion
    }
}
