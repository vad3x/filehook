using System.Threading;
using System.Threading.Tasks;

namespace Filehook.Abstractions
{
    public interface IFilehookService
    {
        Task PurgeAsync<TEntity>(
            TEntity entity,
            string attachmentName = null,
            FilehookAttachmentOptions filehookAttachmentOptions = null,
            CancellationToken cancellationToken = default) where TEntity : class;

        Task PurgeAsync(
            FilehookBlob[] blobs,
            CancellationToken cancellationToken = default);

        Task<FilehookAttachment[]> GetAttachmentsAsync<TEntity>(
            TEntity[] entities,
            string[] attachmentNames = null,
            FilehookAttachmentOptions filehookAttachmentOptions = null,
            CancellationToken cancellationToken = default) where TEntity : class;

        Task<FilehookAttachment> SetAttachmentAsync<TEntity>(
            TEntity entity,
            string attachmentName,
            FilehookFileInfo fileInfo,
            FilehookAttachmentOptions filehookAttachmentOptions = null,
            CancellationToken cancellationToken = default) where TEntity : class;

        Task<FilehookAttachment> AddAttachmentAsync<TEntity>(
            TEntity entity,
            string attachmentName,
            FilehookFileInfo fileInfo,
            FilehookAttachmentOptions filehookAttachmentOptions = null,
            CancellationToken cancellationToken = default) where TEntity : class;

        Task<FilehookAttachment> AttachAsync<TEntity>(
            TEntity entity,
            string attachmentName,
            FilehookBlob blob,
            FilehookAttachmentOptions filehookAttachmentOptions = null,
            CancellationToken cancellationToken = default) where TEntity : class;
    }
}
