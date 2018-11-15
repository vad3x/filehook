using System;
using System.Linq.Expressions;

namespace Microsoft.EntityFrameworkCore
{
    public interface IFileInfo
    {
        string FileName { get; }
    }

    public static class FilehookEntityTypeBuilderExtensions
    {
        public static void HasAttachment<TEntity, TRelatedEntity>(
            this Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> entityTypeBuilder,
            Expression<Func<TEntity, TRelatedEntity>> navigationExpression)
            where TEntity : class
            where TRelatedEntity : class, IFileInfo
        {
            entityTypeBuilder.OwnsOne(navigationExpression);
        }
    }
}
