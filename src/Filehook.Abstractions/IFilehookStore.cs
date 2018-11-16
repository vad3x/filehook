using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Filehook.Abstractions.Stores
{
    public interface IFilehookStore
    {
        Task<FilehookBlob[]> GetBlobsAsync(
            string name,
            string entityId,
            string entityType,
            CancellationToken cancellationToken = default);

        Task<FilehookAttachment> CreateAttachmentAsync(
            string name,
            string entityId,
            string entityType,
            FilehookBlob blob,
            CancellationToken cancellationToken = default);

        Task<FilehookBlob> CreateBlobAsync(
            string key,
            string fileName,
            string contentType,
            long byteSize,
            string checksum,
            IDictionary<string, string> metadata = null,
            CancellationToken cancellationToken = default);
    }
}
