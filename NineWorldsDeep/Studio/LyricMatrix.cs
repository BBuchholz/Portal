using System.Collections.Generic;

namespace NineWorldsDeep.Studio
{
    public class LyricMatrix
    {
        private Dictionary<int, LyricBit> lyricBits;

        public LyricMatrix()
        {
            lyricBits = new Dictionary<int, LyricBit>();
        }

        private int GenerateNewId()
        {
            int i = 1;

            while (lyricBits.ContainsKey(i))
            {
                i++;
            }

            return i;
        }

        public LyricBit CreateNewLyricBit()
        {
            LyricBit lb = new LyricBit(GenerateNewId());
            lyricBits[lb.Id] = lb;
            return lb;
        }

        public IEnumerable<LyricBit> All()
        {
            return lyricBits.Values;
        }
    }
}