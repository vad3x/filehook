using System.Collections.Generic;

namespace Filehook.Abstractions
{
    public class FilehookBlob
    {
        public FilehookBlob(
            string key,
            string fileName,
            string contentType,
            long byteSize,
            string checksum,
            IDictionary<string, string> metadata = null)
        {
            Key = key;
            FileName = fileName;
            ContentType = contentType;
            Metadata = metadata ?? new Dictionary<string, string>();
            ByteSize = byteSize;
            Checksum = checksum;
        }

        public string Key { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public IDictionary<string, string> Metadata { get; set; }

        public long ByteSize { get; set; }

        public string Checksum { get; set; }
    }
}
