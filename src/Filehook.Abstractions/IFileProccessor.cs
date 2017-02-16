using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace Filehook.Abstractions
{
    public interface IFileProccessor
    {
        bool CanProccess(string fileExtension, byte[] bytes);

        Dictionary<string, MemoryStream> Proccess<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression,
            byte[] bytes) where TEntity : class;

        MemoryStream ProccessStyle(byte[] bytes, IFileStyle style);
    }
}
