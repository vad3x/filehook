using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filehook.Abstractions.Stores
{
    public interface IBlobStore
    {
        Task<FilehookBlob> CreateAsync(
            string key,
            string fileName,
            string contentType,
            long byteSize,
            string checksum,
            IDictionary<string, string> metadata = null);
    }
}
