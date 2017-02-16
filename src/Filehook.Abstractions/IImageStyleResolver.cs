using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Filehook.Abstractions
{
    public interface IFileStyleResolver
    {
        IEnumerable<FileStyle> Resolve<TEntity>(Expression<Func<TEntity, string>> propertyExpression);
    }
}
