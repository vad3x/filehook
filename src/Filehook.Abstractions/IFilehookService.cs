using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Filehook.Abstractions
{
    public interface IFilehookService
    {
        Task<bool> ExistsAsync<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression,
            string style) where TEntity : class;

        IDictionary<string, string> GetUrls<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression) where TEntity : class;

        string GetUrl<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression,
            string style) where TEntity : class;

        Task<IDictionary<string, FilehookSavingResult1>> SaveAsync<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression,
            string filename,
            byte[] bytes) where TEntity : class;

        bool CanProccess(string fileExtension, byte[] bytes);

        Task RemoveAsync<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression) where TEntity : class;
    }

    public interface INewFilehookService
    {
        Task PurgeAsync(
            FilehookBlob blob,
            CancellationToken cancellationToken = default);

        Task<FilehookBlob[]> GetBlobsAsync<TEntity>(
            TEntity entity,
            string attachmentName,
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
