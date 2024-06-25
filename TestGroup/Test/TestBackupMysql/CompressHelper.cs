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
        public static async Task<bool> CompressData(string sourceFile, string destination)
        {
            var result = false;
            using (FileStream fs = new(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4 * 1024, FileOptions.Asynchronous))
            using (FileStream output = new(destination, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 1024, FileOptions.Asynchronous))
            using (DeflateStream dstream = new(output, CompressionLevel.Optimal))
            {
                await fs.CopyToAsync(dstream);
                result = true;
            }
            return result;
        }

        public static async Task<bool> DecompressData(string sourceFile, string destination)
        {
            var result = false;
            using (FileStream fs = new(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4 * 1024, FileOptions.Asynchronous))
            using (FileStream output = new(destination, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 1024, FileOptions.Asynchronous))
            using (DeflateStream dstream = new(fs, CompressionMode.Decompress))
            {
                await dstream.CopyToAsync(output);
                result = true;
            }
            return result;
        }



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
