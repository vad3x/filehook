using System;
using System.Linq.Expressions;

namespace Filehook.Abstractions
{
    public interface IFileStorageNameResolver
    {
        string Resolve<TEntity>(Expression<Func<TEntity, string>> propertyExpression);
    }
}
