using Dawn;

namespace Filehook.Abstractions
{
    public class FileStorageSavingResult
    {
        public static FileStorageSavingResult Success(
            string absoluteLocation,
            string checksum,
            long byteSize)
        {
            Guard.Argument(absoluteLocation, nameof(absoluteLocation)).NotNull().NotEmpty();
            Guard.Argument(checksum, nameof(checksum)).NotNull().NotEmpty();
            Guard.Argument(byteSize, nameof(byteSize)).NotZero().NotNegative();

            return new FileStorageSavingResult
            {
                Succeeded = true,
                AbsoluteLocation = absoluteLocation,
                Checksum = checksum,
                ByteSize = byteSize
            };
        }

        private FileStorageSavingResult()
        {
        }

        public bool Succeeded { get; private set; }

        public string AbsoluteLocation { get; private set; }
        public string Checksum { get; private set; }
        public long ByteSize { get; set; }
    }
}
