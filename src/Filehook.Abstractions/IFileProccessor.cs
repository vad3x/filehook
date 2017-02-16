using System.Collections.Generic;
using System.IO;

namespace Filehook.Abstractions
{
    public interface IFileProccessor
    {
        bool CanProccess(string fileExtension, byte[] bytes);

        IDictionary<string, MemoryStream> Proccess(byte[] bytes, IEnumerable<FileStyle> styles);
    }
}
