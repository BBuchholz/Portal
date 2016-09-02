using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Sqlite.Model
{
    //TODO: should be moved to NineWorldsDeep.Model (mark this one obsolete)
    // mimic Gauntlet ModelItem and adjust internal implementation accordingly
    [Obsolete("migrating to new package NineWorldsDeep.Model")]
    public class HashModelItem
    {

        private String mFileId, mHash, mHashedAt;

        public HashModelItem(String hash, String hashedAt)
            : this(null, hash, hashedAt)
        {
            //chained constructor, deliberately empty
        }

        public HashModelItem(String fileId, String hashValue, String hashedAt)
        {
            SetFileId(fileId);
            SetHash(hashValue);
            SetHashedAt(hashedAt);
        }

        public String GetFileId()
        {
            return this.mFileId;
        }

        public void SetFileId(String mFileId)
        {
            this.mFileId = mFileId;
        }

        public String GetHash()
        {
            return this.mHash;
        }

        public void SetHash(String hash)
        {
            this.mHash = hash;
        }

        public String GetHashedAt()
        {
            return this.mHashedAt;
        }

        public void SetHashedAt(String hashedAt)
        {
            this.mHashedAt = hashedAt;
        }
    }
}
