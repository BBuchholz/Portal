using System;
using NineWorldsDeep.Mnemosyne.V5;
using System.Collections.Generic;

namespace NineWorldsDeep.Hive
{
    public class HiveFileGrouping
    {
        List<DevicePath> _devicePaths;
        bool _devicePathsLoaded = false;

        public string Name { private set; get; }
        public List<DevicePath> DevicePaths
        {
            private set
            {
                _devicePaths = value;
            }

            get
            {
                if (!_devicePathsLoaded)
                {
                    LoadDevicePaths();                    
                }

                return _devicePaths;
            }
        }
        
        public HiveFileGrouping(string name)
        {
            this.Name = name;
            this.DevicePaths = new List<DevicePath>();
        }

        //supports lazy loading
        private void LoadDevicePaths()
        {
            //mockup
            for (int j = 0; j < 5; j++)
            {
                Add(new DevicePath()
                {
                    DeviceName = "laptop",
                    DevicePathValue = @"C:\NWD-SNDBX\dummyFile" + j + ".mock"
                });
            }

            _devicePathsLoaded = true;
        }


        public void Add(DevicePath devicePath)
        {
            //todo: logic to prevent duplicates
            _devicePaths.Add(devicePath);
        }
    }
}