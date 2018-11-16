using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filehook.Abstractions
{

    public interface IFileProccessor
    {
        bool CanProccess(string fileExtension, byte[] bytes);

        Task<IEnumerable<FileProccessingResult>> ProccessAsync(byte[] bytes, IEnumerable<FileStyle> styles);
    }
}
