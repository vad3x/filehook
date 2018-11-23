namespace Filehook.Abstractions
{
    public sealed class FilehookUploadingResult
    {
        public static FilehookUploadingResult Success(
            FilehookBlob blob,
            string storageName,
            string absoluteLocation)
        {
            return new FilehookUploadingResult
            {
                Succeeded = true,
                Blob = blob,
                StorageName = storageName,
                AbsoluteLocation = absoluteLocation
            };
        }

        private FilehookUploadingResult()
        {
        }

        public bool Succeeded { get; private set; }

        public FilehookBlob Blob { get; private set; }
        public string StorageName { get; private set; }
        public string AbsoluteLocation { get; private set; }
    }
}
