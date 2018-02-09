//using PortableDeviceApiLib;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NineWorldsDeep.Mtp
{
    //public class PortableDevice
    //{
    //    protected bool _isConnected;
    //    protected readonly PortableDeviceClass _device;

    //    public PortableDevice(string deviceId)
    //    {
    //        this._device = new PortableDeviceClass();
    //        this.DeviceId = deviceId;
    //    }

    //    public string DeviceId { get; set; }

    //    public void Connect()
    //    {
    //        if (this._isConnected)
    //        { return; }

    //        var clientInfo =
    //            (IPortableDeviceValues)
    //            new PortableDeviceTypesLib.PortableDeviceValuesClass();

    //        this._device.Open(this.DeviceId, clientInfo);
    //        this._isConnected = true;
    //    }

    //    public void Disconnect()
    //    {
    //        if (!this._isConnected)
    //        { return; }

    //        this._device.Close();
    //        this._isConnected = false;
    //    }

    //    public string FriendlyName
    //    {
    //        get
    //        {
    //            // identify property to retrieve
    //            _tagpropertykey property =
    //                new _tagpropertykey();
    //            property.fmtid = new Guid(0x26D4979A, 0xE643, 0x4626,
    //                                      0x9E, 0x2B, 0x73, 0x6D,
    //                                      0xC0, 0xC9, 0x2F, 0xDC);
    //            property.pid = 12;

    //            return GetStringProperty(property);
    //        }

    //    }

    //    public string Model
    //    {
    //        get
    //        {
    //            // identify property to retrieve
    //            _tagpropertykey property =
    //                new _tagpropertykey();
    //            property.fmtid = new Guid(0x26D4979A, 0xE643, 0x4626,
    //                                      0x9E, 0x2B, 0x73, 0x6D,
    //                                      0xC0, 0xC9, 0x2F, 0xDC);
    //            property.pid = 15;

    //            return GetStringProperty(property);
    //        }

    //    }

    //    public string DeviceType
    //    {
    //        get
    //        {
    //            // identify property to retrieve
    //            _tagpropertykey property =
    //                new _tagpropertykey();
    //            property.fmtid = new Guid(0x26D4979A, 0xE643, 0x4626,
    //                                      0x9E, 0x2B, 0x73, 0x6D,
    //                                      0xC0, 0xC9, 0x2F, 0xDC);
    //            property.pid = 8;

    //            return GetStringProperty(property);
    //        }

    //    }

    //    private string GetStringProperty(_tagpropertykey key)
    //    {
    //        if (!this._isConnected)
    //        {
    //            String msg = "Not connected to device.";
    //            throw new InvalidOperationException(msg);
    //        }

    //        // retrieve device properties
    //        IPortableDeviceContent content;
    //        IPortableDeviceProperties properties;
    //        this._device.Content(out content);
    //        content.Properties(out properties);

    //        // retrieve property values
    //        IPortableDeviceValues propertyValues;
    //        properties.GetValues("DEVICE", null, out propertyValues);

    //        // retrieve the friendly name
    //        string propertyValue;
    //        propertyValues.GetStringValue(ref key,
    //                                      out propertyValue);

    //        return propertyValue;
    //    }

    //    public PortableDeviceFolder GetContents()
    //    {
    //        var root = new PortableDeviceFolder("DEVICE", "DEVICE");

    //        IPortableDeviceContent content;

    //        this._device.Content(out content);
    //        EnumerateContents(ref content, root);

    //        return root;
    //    }

    //    /// <summary>
    //    /// recursive enumeration of all objects in the given folder's hierarchy
    //    /// </summary>
    //    /// <param name="content"></param>
    //    /// <param name="parent"></param>
    //    private static void
    //        EnumerateContents(ref IPortableDeviceContent content,
    //                              PortableDeviceFolder parent)
    //    {
    //        // Get the properties of the object
    //        IPortableDeviceProperties properties;
    //        content.Properties(out properties);

    //        // Enumerate the items contained by the current object
    //        IEnumPortableDeviceObjectIDs objectIds;
    //        content.EnumObjects(0, parent.Id, null, out objectIds);

    //        uint fetched = 0;
    //        do
    //        {
    //            string objectId;

    //            objectIds.Next(1, out objectId, ref fetched);

    //            if (fetched > 0)
    //            {
    //                var currentObject = WrapObject(properties, objectId);

    //                parent.Files.Add(currentObject);

    //                if (currentObject is PortableDeviceFolder)
    //                {
    //                    EnumerateContents(ref content,
    //                        (PortableDeviceFolder)currentObject);
    //                }
    //            }

    //        } while (fetched > 0);
    //    }

    //    protected static PortableDeviceObject
    //        WrapObject(IPortableDeviceProperties properties,
    //                   string objectId)
    //    {
    //        IPortableDeviceKeyCollection keys;
    //        properties.GetSupportedProperties(objectId, out keys);

    //        IPortableDeviceValues values;
    //        properties.GetValues(objectId, keys, out values);

    //        //Get the name of the object
    //        string name;
    //        var property = new _tagpropertykey();
    //        property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A,
    //                                  0xAF, 0xFC, 0xDA, 0x8B, 0x60,
    //                                  0xEE, 0x4A, 0x3C);
    //        property.pid = 4;
    //        values.GetStringValue(property, out name);

    //        // Get the type of the object
    //        Guid contentType;
    //        property = new _tagpropertykey();
    //        property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A,
    //                                  0xAF, 0xFC, 0xDA, 0x8B, 0x60,
    //                                  0xEE, 0x4A, 0x3C);
    //        property.pid = 7;
    //        values.GetGuidValue(property, out contentType);

    //        var folderType = new Guid(0x27E2E392, 0xA111, 0x48E0,
    //                                  0xAB, 0x0C, 0xE1, 0x77, 0x05,
    //                                  0xA0, 0x5F, 0x85);
    //        var functionalType = new Guid(0x99ED0160, 0x17FF, 0x4C44,
    //                                      0x9D, 0x98, 0x1D, 0x7A, 0x6F,
    //                                      0x94, 0x19, 0x21);

    //        if (contentType == folderType ||
    //            contentType == functionalType)
    //        {
    //            return new PortableDeviceFolder(objectId, name);
    //        }

    //        return new PortableDeviceFile(objectId, name);
    //    }

    //    public void DownloadFile(PortableDeviceFile file,
    //                             string saveToFolderPath)
    //    {
    //        IPortableDeviceContent content;
    //        this._device.Content(out content);

    //        IPortableDeviceResources resources;
    //        content.Transfer(out resources);

    //        PortableDeviceApiLib.IStream wpdStream;
    //        uint optimalTransferSize = 0;

    //        var property = new _tagpropertykey();
    //        property.fmtid = new Guid(0xE81E79BE, 0x34F0, 0x41BF,
    //                                  0xB5, 0x3F, 0xF1, 0xA0,
    //                                  0x6A, 0xE8, 0x78, 0x42);
    //        property.pid = 0;

    //        resources.GetStream(file.Id,
    //                            ref property,
    //                            0,
    //                            ref optimalTransferSize,
    //                            out wpdStream);

    //        System.Runtime.InteropServices.ComTypes.IStream sourceStream =
    //            (System.Runtime.InteropServices.ComTypes.IStream)wpdStream;

    //        var filename = Path.GetFileName(file.Id);
    //        FileStream targetStream =
    //            new FileStream(Path.Combine(saveToFolderPath, filename),
    //                           FileMode.Create,
    //                           FileAccess.Write);

    //        unsafe
    //        {
    //            var buffer = new byte[1024];
    //            int bytesRead;
    //            do
    //            {
    //                sourceStream.Read(buffer, 1024, new IntPtr(&bytesRead));
    //                targetStream.Write(buffer, 0, 1024);
    //            } while (bytesRead > 0);

    //            targetStream.Close();
    //        }

    //        Marshal.ReleaseComObject(sourceStream);
    //        Marshal.ReleaseComObject(wpdStream);
    //    }

    //    public void DeleteFile(PortableDeviceFile file)
    //    {
    //        IPortableDeviceContent content;
    //        this._device.Content(out content);

    //        var variant = new tag_inner_PROPVARIANT();
    //        StringToPropVariant(file.Id, out variant);

    //        IPortableDevicePropVariantCollection objectIds =
    //            new PortableDeviceTypesLib.PortableDevicePropVariantCollection()
    //            as IPortableDevicePropVariantCollection;
    //        objectIds.Add(variant);

    //        content.Delete(0, objectIds, null);
    //    }

    //    protected static void StringToPropVariant(string value,
    //        out PortableDeviceApiLib.tag_inner_PROPVARIANT propvarValue)
    //    {
    //        PortableDeviceApiLib.IPortableDeviceValues pValues =
    //            (PortableDeviceApiLib.IPortableDeviceValues)
    //                new PortableDeviceTypesLib.PortableDeviceValuesClass();

    //        var WPD_OBJECT_ID = new _tagpropertykey();
    //        WPD_OBJECT_ID.fmtid =
    //            new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA,
    //                        0x8B, 0x60, 0xEE, 0x4A, 0x3C);
    //        WPD_OBJECT_ID.pid = 2;

    //        pValues.SetStringValue(ref WPD_OBJECT_ID, value);

    //        pValues.GetValue(ref WPD_OBJECT_ID, out propvarValue);
    //    }

    //    public void TransferContentToDevice(string fileWithPath,
    //                                        string parentObjectId)
    //    {
    //        IPortableDeviceContent content;
    //        this._device.Content(out content);

    //        IPortableDeviceValues values =
    //            GetRequiredPropertiesForContentType(fileWithPath,
    //                                                parentObjectId);

    //        IStream tempStream;
    //        uint optimalTransferSizeBytes = 0;
    //        content.CreateObjectWithPropertiesAndData(
    //            values,
    //            out tempStream,
    //            ref optimalTransferSizeBytes,
    //            null);

    //        System.Runtime.InteropServices.ComTypes.IStream targetStream =
    //            (System.Runtime.InteropServices.ComTypes.IStream)tempStream;

    //        try
    //        {
    //            using (var sourceStream =
    //                new FileStream(fileWithPath,
    //                               FileMode.Open,
    //                               FileAccess.Read))
    //            {
    //                var buffer = new byte[optimalTransferSizeBytes];
    //                int bytesRead;
    //                do
    //                {
    //                    bytesRead = sourceStream.Read(
    //                        buffer, 0, (int)optimalTransferSizeBytes);
    //                    IntPtr pcbWritten = IntPtr.Zero;

    //                    //changed per comments on original tutorial
    //                    //was getting "the data area passed to a system
    //                    //call is too small" error
    //                    if (bytesRead < (int)optimalTransferSizeBytes)
    //                    {
    //                        targetStream.Write(buffer,
    //                                           bytesRead,
    //                                           pcbWritten);
    //                    }
    //                    else
    //                    {
    //                        targetStream.Write(buffer,
    //                                           (int)optimalTransferSizeBytes,
    //                                           pcbWritten);
    //                    }

    //                } while (bytesRead > 0);
    //            }
    //            targetStream.Commit(0);
    //        }
    //        finally
    //        {
    //            Marshal.ReleaseComObject(tempStream);
    //        }
    //    }

    //    protected IPortableDeviceValues
    //        GetRequiredPropertiesForContentType(string fileName,
    //                                            string parentObjectId)
    //    {
    //        IPortableDeviceValues values =
    //            new PortableDeviceTypesLib.PortableDeviceValues()
    //            as IPortableDeviceValues;

    //        var WPD_OBJECT_PARENT_ID = new _tagpropertykey();
    //        WPD_OBJECT_PARENT_ID.fmtid =
    //            new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
    //                        0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
    //        WPD_OBJECT_PARENT_ID.pid = 3;
    //        values.SetStringValue(ref WPD_OBJECT_PARENT_ID, parentObjectId);

    //        FileInfo fileInfo = new FileInfo(fileName);
    //        var WPD_OBJECT_SIZE = new _tagpropertykey();
    //        WPD_OBJECT_SIZE.fmtid =
    //            new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
    //                        0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
    //        WPD_OBJECT_SIZE.pid = 11;
    //        values.SetUnsignedLargeIntegerValue(WPD_OBJECT_SIZE, (ulong)fileInfo.Length);

    //        var WPD_OBJECT_ORIGINAL_FILE_NAME = new _tagpropertykey();
    //        WPD_OBJECT_ORIGINAL_FILE_NAME.fmtid =
    //            new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
    //                        0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
    //        WPD_OBJECT_ORIGINAL_FILE_NAME.pid = 12;
    //        values.SetStringValue(WPD_OBJECT_ORIGINAL_FILE_NAME, Path.GetFileName(fileName));

    //        var WPD_OBJECT_NAME = new _tagpropertykey();
    //        WPD_OBJECT_NAME.fmtid =
    //            new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
    //                        0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
    //        WPD_OBJECT_NAME.pid = 4;
    //        values.SetStringValue(WPD_OBJECT_NAME, Path.GetFileName(fileName));

    //        //From Original Tutorial: 
    //        //  "Remark: To keep it easy I extracted the filename and 
    //        //   path of the book and used that to set both the 
    //        //   WPD_OBJECT_ORIGINAL_FILE_NAME and 
    //        //   WPD_OBJECT_NAME properties.

    //        return values;
    //    }
    //}
}