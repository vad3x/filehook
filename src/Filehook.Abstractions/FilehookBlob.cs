using System;
using System.Collections.Generic;

namespace Filehook.Abstractions
{
    public class FilehookBlob
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public IDictionary<string, string> Metadata { get; set; }

        public long ByteSize { get; set; }

        public string Checksum { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
