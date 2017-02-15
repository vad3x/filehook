using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Filehook.Proccessors.Image.Abstractions
{
    public interface IImageStyleResolver
    {
        IEnumerable<ImageStyle> Resolve<TEntity>(Expression<Func<TEntity, string>> propertyExpression);
    }
}
