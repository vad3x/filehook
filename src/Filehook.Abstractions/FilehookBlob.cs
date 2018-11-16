using System.Collections.Generic;

namespace Filehook.Abstractions
{
    public class FilehookSavingResult
    {
        public static FilehookSavingResult Success(
            FilehookBlob blob,
            string storageName,
            string absoluteLocation)
        {
            return new FilehookSavingResult
            {
                Succeeded = true,
                Blob = blob,
                StorageName = storageName,
                AbsoluteLocation = absoluteLocation
            };
        }

        private FilehookSavingResult()
        {
        }

        public bool Succeeded { get; private set; }

        public FilehookBlob Blob { get; private set; }
        public string StorageName { get; private set; }
        public string AbsoluteLocation { get; private set; }
    }

    public class FilehookBlob
    {
        public string Key { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public IDictionary<string, string> Metadata { get; set; }

        public long ByteSize { get; set; }

        public string Checksum { get; set; }
    }
}
