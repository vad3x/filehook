using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Filehook.Abstractions.Stores
{
    public interface IFilehookStore
    {
        Task<FilehookBlob> CreateBlobAsync(
            string key,
            string fileName,
            string contentType,
            long byteSize,
            string checksum,
            IDictionary<string, string> metadata = null,
            CancellationToken cancellationToken = default);

        Task PurgeAsync(
            FilehookBlob[] blobs,
            CancellationToken cancellationToken = default);

        Task<FilehookAttachment[]> GetAttachmentsAsync(
            EntityMetadata[] entities,
            string[] names = null,
            CancellationToken cancellationToken = default);

        Task<FilehookAttachment> AddAttachmentAsync(
            string name,
            EntityMetadata entity,
            FilehookBlob blob,
            CancellationToken cancellationToken = default);

        Task<FilehookAttachment> SetAttachmentAsync(
            string name,
            EntityMetadata entity,
            FilehookBlob blob,
            CancellationToken cancellationToken = default);
    }
}
