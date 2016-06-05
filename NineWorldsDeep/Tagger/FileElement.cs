namespace NineWorldsDeep.Tagger
{
    public class FileElement
    {
        /// <summary>
        /// will return a file element with tag string, 
        /// path, and name populated from supplied
        /// TagMatrix
        /// </summary>
        /// <param name="path"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static FileElement FromPath(string path, TagMatrix matrix)
        {
            return new FileElement
            {
                Path = path,
                Name = System.IO.Path.GetFileName(path),
                TagString = matrix.GetTagString(path)
            };
        }

        /// <summary>
        /// will return a file element with path and 
        /// name populated, TagString will remain empty
        /// </summary>
        /// <param name="path"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static FileElement FromPath(string path)
        {
            return new FileElement
            {
                Path = path,
                Name = System.IO.Path.GetFileName(path)
            };
        }

        public void PopulateTagString(TagMatrix tm)
        {
            TagString = tm.GetTagString(Path);
        }

        public string Path { get; set; }
        public string Name { get; set; }
        public string TagString { get; set; }

        public override bool Equals(object obj)
        {
            if(obj ==null || GetType() != obj.GetType())
            {
                return false;
            }

            FileElement fe = (FileElement)obj;
            return fe.Name.Equals(Name) && fe.Path.Equals(Path);
        }

        public override int GetHashCode()
        {//TODO: LICENSE NOTES
            //from: http://stackoverflow.com/questions/1008633/gethashcode-problem-using-xor

            int hash = 23;
            hash = hash * 37 + Name.GetHashCode();
            hash = hash * 37 + Path.GetHashCode();
            return hash;

        }
    }
      
}