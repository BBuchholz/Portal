using NineWorldsDeep.Mtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Parser
{
    public class NwdUriProcessEntry
    {
        private NwdUri nwdUri;
        private NwdPortableDeviceObject pdo;

        public NwdUriProcessEntry(NwdUri nwdUri)
        {
            this.nwdUri = nwdUri;
            Processed = false;
        }

        public string URI { get { return nwdUri.URI; } }
        public bool FoundOnDevice { get { return pdo != null; } }
        public bool Processed { get; set; }
        public NwdPortableDeviceObject DeviceObject
        {
            get { return pdo; }
            set { pdo = value; }
        }

        public NwdUri NwdUri { get { return nwdUri; } }

        public string Path { get { return nwdUri.Path; } }
        public string Hash { get { return nwdUri.Hash; } }

        public int PathId { get; set; }
        public int HashId { get; set; }
    }
}
