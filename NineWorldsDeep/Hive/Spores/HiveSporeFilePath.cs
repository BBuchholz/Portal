using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Hive.Lobes;

namespace NineWorldsDeep.Hive.Spores
{
    public class HiveSporeFilePath : HiveSpore, IEquatable<HiveSporeFilePath>
    {
        public string FilePath { get; private set; }
        public override HiveSporeType HiveSporeType { get; protected set; }

        public HiveSporeFilePath(string filePath, HiveLobe parentLobe)
            : base(parentLobe)
        {
            Name = System.IO.Path.GetFileName(filePath);
            FilePath = filePath;

            HiveSporeType = UtilsHive.SporeTypeFromFilePath(FilePath);          
        }
        
        #region "equality"

        public bool Equals(HiveSporeFilePath other)
        {
            if (other == null) return false;

            return FilePath.ToLower().Equals(other.FilePath.ToLower());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as HiveSporeFilePath);
        }

        public override int GetHashCode()
        {
            return FilePath.ToLower().GetHashCode();
        }

        #endregion
    }
}
