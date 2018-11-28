using System.Collections.Generic;
using System.Linq;

using Dawn;

using Filehook.Abstractions;

namespace Filehook
{
    public static class FilehookAttachmentEnumerableExtensions
    {
        private static readonly FilehookAttachmentOptions _defaultFilehookAttachmentOptions = new FilehookAttachmentOptions();

        public static FilehookBlob[] FindBlobs<TEntity>(
            this IEnumerable<FilehookAttachment> attachments,
            TEntity entity,
            string attachmentName,
            FilehookAttachmentOptions options = null) where TEntity : class
        {
            Guard.Argument(attachments, nameof(attachments)).NotNull();
            Guard.Argument(entity, nameof(entity)).NotNull();
            Guard.Argument(attachmentName, nameof(attachmentName)).NotNull().NotEmpty();

            options = options ?? _defaultFilehookAttachmentOptions;

            string entityType = options.ResolveEntityType(entity.GetType());
            string entityId = options.ResolveEntityId(entity);

            return attachments
                .Where(x => x.Name == attachmentName && x.EntityId == entityId && x.EntityType == entityType)
                .Select(x => x.Blob)
                .ToArray();
        }

        public static FilehookBlob FindBlob<TEntity>(
            this IEnumerable<FilehookAttachment> attachments,
            TEntity entity,
            string attachmentName,
            FilehookAttachmentOptions options = null) where TEntity : class
        {
            return FindBlobs(attachments, entity, attachmentName, options).FirstOrDefault();
        }
    }
}
