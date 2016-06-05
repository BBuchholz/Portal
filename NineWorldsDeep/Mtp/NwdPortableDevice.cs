using PortableDeviceApiLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Mtp
{
    public class NwdPortableDevice : PortableDevice
    {
        public NwdPortableDevice(string deviceId)
            : base(deviceId)
        {
            Refresh();
        }

        public new string FriendlyName { get; set; }
        public new string Model { get; set; }
        public new string DeviceType { get; set; }
        public NwdPortableDeviceFolder RootFolder { get; private set; }
        public bool IsConnected { get { return _isConnected; } }

        public string Description { get; set; }

        public void Refresh()
        {
            Connect();

            //get all properties
            this.FriendlyName = base.FriendlyName;
            this.Model = base.Model;
            this.DeviceType = base.DeviceType;

            this.RootFolder = GetRootFolder();

            Disconnect();
        }

        private NwdPortableDeviceFolder GetRootFolder()
        {
            var root = new NwdPortableDeviceFolder("DEVICE", "DEVICE");

            IPortableDeviceContent content;
            this._device.Content(out content);
            GetChildren(ref content, root);

            return root;
        }

        public void FillFolder(NwdPortableDeviceFolder folder)
        {
            if (!_isConnected)
            {
                Connect();
            }

            IPortableDeviceContent content;
            this._device.Content(out content);
            GetChildren(ref content, folder);

            Disconnect();
        }

        /// <summary>
        /// this is just PortableDevice.EnumerateContents() without the recursion
        /// </summary>
        /// <param name="content"></param>
        /// <param name="parent"></param>
        private static void GetChildren(ref IPortableDeviceContent content,
                                        NwdPortableDeviceFolder parent)
        {
            // Get the properties of the object
            IPortableDeviceProperties properties;
            content.Properties(out properties);

            // Enumerate the items contained by the current object
            IEnumPortableDeviceObjectIDs objectIds;
            content.EnumObjects(0, parent.Id, null, out objectIds);

            uint fetched = 0;
            do
            {
                string objectId;

                objectIds.Next(1, out objectId, ref fetched);

                if (fetched > 0)
                {
                    var currentObject = WrapObject(properties, objectId);

                    parent.Files.Add(currentObject);
                }

            } while (fetched > 0);
        }

        protected new static NwdPortableDeviceObject
            WrapObject(IPortableDeviceProperties properties,
                       string objectId)
        {
            IPortableDeviceKeyCollection keys;
            properties.GetSupportedProperties(objectId, out keys);

            IPortableDeviceValues values;
            properties.GetValues(objectId, keys, out values);

            //Get the name of the object
            string name;
            var property = new _tagpropertykey();
            property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A,
                                      0xAF, 0xFC, 0xDA, 0x8B, 0x60,
                                      0xEE, 0x4A, 0x3C);
            property.pid = 4;
            values.GetStringValue(property, out name);

            // Get the type of the object
            Guid contentType;
            property = new _tagpropertykey();
            property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A,
                                      0xAF, 0xFC, 0xDA, 0x8B, 0x60,
                                      0xEE, 0x4A, 0x3C);
            property.pid = 7;
            values.GetGuidValue(property, out contentType);

            var folderType = new Guid(0x27E2E392, 0xA111, 0x48E0,
                                      0xAB, 0x0C, 0xE1, 0x77, 0x05,
                                      0xA0, 0x5F, 0x85);
            var functionalType = new Guid(0x99ED0160, 0x17FF, 0x4C44,
                                          0x9D, 0x98, 0x1D, 0x7A, 0x6F,
                                          0x94, 0x19, 0x21);

            if (contentType == folderType ||
                contentType == functionalType)
            {
                return new NwdPortableDeviceFolder(objectId, name);
            }
            //TODO: LICENSE NOTES
            //begin test edit
            //per: http://stackoverflow.com/questions/18059234/get-full-name-of-a-file-on-a-windows-portable-device
            property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A,
                                      0xAF, 0xFC, 0xDA, 0x8B, 0x60,
                                      0xEE, 0x4A, 0x3C);
            property.pid = 12; //WPD_OBJECT_ORIGINAL_FILE_NAME
            values.GetStringValue(property, out name);
            //end test edit

            return new NwdPortableDeviceFile(objectId, name);
        }

        public void DownloadFile(NwdPortableDeviceFile file,
                                 string saveToFolderPath)
        {
            if (!_isConnected)
            {
                Connect();
            }

            IPortableDeviceContent content;
            this._device.Content(out content);

            IPortableDeviceResources resources;
            content.Transfer(out resources);

            PortableDeviceApiLib.IStream wpdStream;
            uint optimalTransferSize = 0;

            var property = new _tagpropertykey();
            property.fmtid = new Guid(0xE81E79BE, 0x34F0, 0x41BF,
                                      0xB5, 0x3F, 0xF1, 0xA0,
                                      0x6A, 0xE8, 0x78, 0x42);
            property.pid = 0;

            resources.GetStream(file.Id,
                                ref property,
                                0,
                                ref optimalTransferSize,
                                out wpdStream);

            System.Runtime.InteropServices.ComTypes.IStream sourceStream =
                (System.Runtime.InteropServices.ComTypes.IStream)wpdStream;

            var filename = Path.GetFileName(file.Name); //uses the name with extension (differs from tutorial version)
            FileStream targetStream =
                new FileStream(Path.Combine(saveToFolderPath, filename),
                               FileMode.Create,
                               FileAccess.Write);

            unsafe
            {
                //TODO: play with this value (bufferSize)
                // (I get different entries missing/invalid based on the 
                // number used, there has to be a connection, probably 
                // an encoding thing, need more research)

                // int bufferSize = 1024;
                //int bufferSize = 4096;
                int bufferSize = 8192;

                var buffer = new byte[bufferSize];
                int bytesRead;
                do
                {
                    sourceStream.Read(buffer, bufferSize, new IntPtr(&bytesRead));
                    targetStream.Write(buffer, 0, bytesRead);
                    targetStream.Flush();

                } while (bytesRead > 0);

                targetStream.Close();

            }
            //TODO: LICENSE NOTES
            //see comments on: https://cgeers.wordpress.com/2011/08/13/wpd-transferring-content/
            Marshal.ReleaseComObject(sourceStream);
            Marshal.ReleaseComObject(wpdStream);

            Disconnect();
        }

        public override string ToString()
        {
            return "Friendly Name: " + FriendlyName +
                   " Model: " + Model +
                   " Device Type: " + DeviceType;
        }

        public void DeleteFile(NwdPortableDeviceFile file)
        {
            Connect();

            IPortableDeviceContent content;
            this._device.Content(out content);

            var variant = new tag_inner_PROPVARIANT();
            StringToPropVariant(file.Id, out variant);

            IPortableDevicePropVariantCollection objectIds =
                new PortableDeviceTypesLib.PortableDevicePropVariantCollection()
                as IPortableDevicePropVariantCollection;
            objectIds.Add(variant);

            content.Delete(0, objectIds, null);

            Disconnect();
        }

        //only real difference is the tutorial version assumes the connect 
        //and disconnect will be handled outside the method
        //I've add a boolean flag to support both versions, and
        //changed the default to connect and disconnect inside the method
        public new void TransferContentToDevice(string fileWithPath,
                                                string parentObjectId)
        {
            TransferContentToDevice(fileWithPath, parentObjectId, true);
        }

        public void TransferContentToDevice(string fileWithPath,
                                            string parentObjectId,
                                            bool connectAndDisconnect)
        {
            if (!connectAndDisconnect)
            {
                base.TransferContentToDevice(fileWithPath, parentObjectId);
            }
            else
            {
                Connect();
                base.TransferContentToDevice(fileWithPath, parentObjectId);
                Disconnect();
            }
        }
    }
}
