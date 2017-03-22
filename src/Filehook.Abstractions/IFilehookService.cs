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
            string id,
            string style) where TEntity : class;

        IDictionary<string, string> GetUrls<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression,
            string id) where TEntity : class;

        string GetUrl<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression,
            string id,
            string style) where TEntity : class;

        Task<IDictionary<string, FilehookSavingResult>> SaveAsync<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression,
            byte[] bytes,
            string id) where TEntity : class;

        bool CanProccess(string fileExtension, byte[] bytes);
    }
}
