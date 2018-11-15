using System.IO;
using Dawn;

namespace Filehook.Abstractions
{
    public class FilehookFileInfo
    {
        public FilehookFileInfo(
            string contentType,
            string fileName,
            Stream fileStream)
        {
            ContentType = Guard.Argument(contentType, nameof(contentType)).NotNull().NotEmpty().Value;
            FileName = Guard.Argument(fileName, nameof(fileName)).NotNull().NotEmpty().Value;
            FileStream = Guard.Argument(fileStream, nameof(fileStream)).NotNull().Value;
        }

        public string ContentType { get; }

        public string FileName { get; }

        public Stream FileStream { get; }
    }
}
