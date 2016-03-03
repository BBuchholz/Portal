using PortableDeviceApiLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Mtp
{
    public class NwdPortableDeviceCollection : Collection<NwdPortableDevice>
    {
        private readonly PortableDeviceManager _deviceManager;

        public NwdPortableDeviceCollection()
        {
            this._deviceManager = new PortableDeviceManager();
        }

        public void Refresh()
        {
            _deviceManager.RefreshDeviceList();

            //determine number of connected devices
            string[] deviceIds = new string[1];
            uint count = 1;
            this._deviceManager.GetDevices(null, ref count);

            //retrieve the ids for each connected device
            deviceIds = new string[count];
            _deviceManager.GetDevices(deviceIds, ref count);
            foreach (var deviceId in deviceIds)
            {
                Add(new NwdPortableDevice(deviceId));
            }
        }
    }
}
