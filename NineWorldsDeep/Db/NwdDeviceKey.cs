using System;

namespace NineWorldsDeep.Db
{
    public class NwdDeviceKey
    {
        public string Description { get; internal set; }
        public string DeviceType { get; internal set; }
        public string FriendlyName { get; internal set; }
        public string Model { get; internal set; }

        public NwdDeviceKey()
        {

        }

        public NwdDeviceKey(string description,
                                    string friendlyName,
                                    string model,
                                    string deviceType)
        {
            this.Description = description;
            this.FriendlyName = friendlyName;
            this.Model = model;
            this.DeviceType = deviceType;
        }

        public override bool Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast return false.
            NwdDeviceKey key = obj as NwdDeviceKey;
            if (key == null)
            {
                return false;
            }

            // Return true if the fields match:
            return key.Equals(this);
        }

        public bool Equals(NwdDeviceKey key)
        {
            // If parameter is null return false:
            if ((object)key == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (key.Description.Equals(Description) &&
                key.FriendlyName.Equals(FriendlyName) &&
                key.DeviceType.Equals(DeviceType) &&
                key.Model.Equals(Model));
        }

        public override int GetHashCode()
        {
            return Description.GetHashCode() ^
                FriendlyName.GetHashCode() ^
                DeviceType.GetHashCode() ^
                Model.GetHashCode();
        }
    }
}