using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBackupMysql
{
    internal class CompressHelper
    {
        public byte[] CompressData(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public byte[] DecompressData(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

        public static byte[] CompressData2(byte[] data)
        {
            byte[] result = null;
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
                {
                    dstream.Write(data, 0, data.Length);
                }
                result = output.ToArray();
            }
            return result;
        }

        public static byte[] DecompressData2(byte[] data)
        {
            byte[] result = null;
            using (MemoryStream input = new MemoryStream(data))
            {
                using (MemoryStream output = new MemoryStream())
                {
                    using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
                    {
                        dstream.CopyTo(output);
                    }
                    result = output.ToArray();
                }
            }
            return result;

        }
    }
}
