using System.Collections.Generic;

namespace NineWorldsDeep.Mtp
{
    public class NwdPortableDeviceFolder : NwdPortableDeviceObject
    {
        private List<NwdPortableDeviceObject> _files;

        public NwdPortableDeviceFolder(string id,
                                       string name)
            : base(id, name)
        {

        }

        public IList<NwdPortableDeviceObject> Files
        {
            get
            {
                if (_files == null)
                {
                    _files = new List<NwdPortableDeviceObject>();
                }

                return _files;
            }
        }

        public override string ToString()
        {
            return "Folder: " + Name;
        }

        //public void Refresh(NwdPortableDevice device)
        //{
        //    Files.Clear();

        //    if (device != null)
        //    {
        //        device.FillFolder(this);
        //    }
        //}
    }
}