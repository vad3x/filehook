using System;
using System.IO;
using System.Security.Cryptography;

namespace Filehook.Abstractions
{
    public static class StreamExtensions
    {
        public static long GetByteSize(this Stream stream)
        {
            return stream.Length;
        }

        public static string GetMD5Checksum(this Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                stream.Position = 0;

                var hash = md5.ComputeHash(stream);

                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
