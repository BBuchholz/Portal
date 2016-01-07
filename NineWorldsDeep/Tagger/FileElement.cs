namespace NineWorldsDeep.Tagger
{
    public class FileElement
    {
        public static FileElement FromPath(string path)
        {
            return new FileElement
            {
                Path = path,
                Name = System.IO.Path.GetFileName(path)
            };
        }

        public string Path { get; set; }
        public string Name { get; set; }

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
        {
            //from: http://stackoverflow.com/questions/1008633/gethashcode-problem-using-xor

            int hash = 23;
            hash = hash * 37 + Name.GetHashCode();
            hash = hash * 37 + Path.GetHashCode();
            return hash;

        }
    }
      
}