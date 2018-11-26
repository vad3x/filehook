using System.Threading;
using System.Threading.Tasks;

namespace Filehook.Abstractions
{
    public interface IFilehookService
    {
        Task PurgeAsync<TEntity>(
            TEntity entity,
            string attachmentName = null,
            CancellationToken cancellationToken = default) where TEntity : class;

        Task PurgeAsync(
            FilehookBlob[] blobs,
            CancellationToken cancellationToken = default);

        FilehookBlob GetSingleBlob<TEntity>(
            TEntity entity,
            string attachmentName,
            FilehookAttachment[] attachments) where TEntity : class;

        FilehookBlob[] GetManyBlobs<TEntity>(
            TEntity entity,
            string attachmentName,
            FilehookAttachment[] attachments) where TEntity : class;

        Task<FilehookAttachment[]> GetAttachmentsAsync<TEntity>(
            TEntity[] entities,
            string[] attachmentNames = null,
            CancellationToken cancellationToken = default) where TEntity : class;

        Task<FilehookAttachment> SetOneAsync<TEntity>(
            TEntity entity,
            string attachmentName,
            FilehookFileInfo fileInfo,
            CancellationToken cancellationToken = default) where TEntity : class;

        Task<FilehookAttachment> AddManyAsync<TEntity>(
            TEntity entity,
            string attachmentName,
            FilehookFileInfo fileInfo,
            CancellationToken cancellationToken = default) where TEntity : class;

        Task<FilehookUploadingResult> UploadAsync(
            FilehookFileInfo fileInfo,
            CancellationToken cancellationToken = default);

        Task<FilehookAttachment> AttachAsync<TEntity>(
            TEntity entity,
            string attachmentName,
            FilehookBlob blob,
            CancellationToken cancellationToken = default) where TEntity : class;
    }
}
