using System.IO;

namespace Filehook.Abstractions
{
    public class FileProccessingResult
    {
        public FileStyle Style { get; set; }

        public MemoryStream Stream { get; set; }

        public object Meta { get; set; }
    }
}