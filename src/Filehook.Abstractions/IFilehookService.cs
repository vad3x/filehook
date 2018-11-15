using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        Task<IDictionary<string, FilehookSavingResult>> SaveAsync<TEntity>(
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
        Task<FilehookBlob[]> GetBlobsAsync<TEntity>(
            TEntity entity,
            string name) where TEntity : class;

        Task<FilehookBlob> SaveAsync<TEntity>(
            TEntity entity,
            string name,
            FilehookFileInfo fileInfo) where TEntity : class;

        //Task RemoveAsync<TEntity>(
        //    TEntity entity,
        //    string name) where TEntity : class;
    }
}
