using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NineWorldsDeep.Warehouse
{
    public class Hashes
    {
        public static string Sha1ForFilePath(string filePath)
        {
            if (File.Exists(filePath))
            {
                //from: http://stackoverflow.com/questions/1993903/how-do-i-do-a-sha1-file-checksum-in-c
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                using (BufferedStream bs = new BufferedStream(fs))
                {
                    using (SHA1Managed sha1 = new SHA1Managed())
                    {
                        byte[] hash = sha1.ComputeHash(bs);
                        StringBuilder formatted = new StringBuilder(2 * hash.Length);
                        foreach (byte b in hash)
                        {
                            formatted.AppendFormat("{0:X2}", b);
                        }

                        return formatted.ToString();
                    }
                }
            }

            return "";
        }

        public static string Sha1ForStringValue(string strValue)
        {
            using (Stream s = GenerateStreamFromString(strValue))
            using (BufferedStream bs = new BufferedStream(s))
            {
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    byte[] hash = sha1.ComputeHash(bs);
                    StringBuilder formatted = new StringBuilder(2 * hash.Length);
                    foreach (byte b in hash)
                    {
                        formatted.AppendFormat("{0:X2}", b);
                    }

                    return formatted.ToString();
                }
            }
        }

        private static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}