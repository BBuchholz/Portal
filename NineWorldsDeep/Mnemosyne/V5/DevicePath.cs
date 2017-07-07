using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Mnemosyne.V5
{
    public class DevicePath
    {
        public string DevicePathValue { get; set; }
        public string DeviceName { get; set; }
        public int DevicePathId { get; set; }
        public int MediaId { get; set; }
        public int MediaDeviceId { get; set; }
        public int MediaPathId { get; set; }
        public DateTime? VerifiedPresent { get; private set; }
        public DateTime? VerifiedMissing { get; private set; }

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
        /// <param name="newVerifiedPresent"></param>
        /// <param name="newVerifiedMissing"></param>
        public void SetTimeStamps(DateTime? newVerifiedPresent,
                                  DateTime? newVerifiedMissing)
        {
            if (newVerifiedPresent != null)
            {
                if (newVerifiedPresent.Value.Kind != DateTimeKind.Utc)
                {
                    newVerifiedPresent = newVerifiedPresent.Value.ToUniversalTime();
                }

                if (VerifiedPresent == null ||
                   DateTime.Compare(VerifiedPresent.Value, 
                                    newVerifiedPresent.Value) < 0)
                {
                    //VerifiedPresent is older or null
                    VerifiedPresent = newVerifiedPresent;
                }
            }

            if (newVerifiedMissing != null)
            {
                if (newVerifiedMissing.Value.Kind != DateTimeKind.Utc)
                {
                    newVerifiedMissing = newVerifiedMissing.Value.ToUniversalTime();
                }

                if (VerifiedMissing == null ||
                   DateTime.Compare(VerifiedMissing.Value, 
                                    newVerifiedMissing.Value) < 0)
                {
                    //VerifiedMissing is older or null
                    VerifiedMissing = newVerifiedMissing;
                }
            }

        }

        public void VerifyPresent()
        {
            SetTimeStamps(TimeStamp.NowUTC(), null);
        }

        public void VerifyMissing()
        {
            SetTimeStamps(null, TimeStamp.NowUTC());
        }
    }
}
