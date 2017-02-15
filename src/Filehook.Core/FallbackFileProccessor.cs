using Filehook.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace Filehook.Core
{
    public class FallbackFileProccessor : IFileProccessor
    {
        private readonly FallbackFileProccessorOptions _options;

        public FallbackFileProccessor(IOptions<FallbackFileProccessorOptions> options)
        {
            _options = options.Value;
        }

        public bool CanProccess(string fileExtension, byte[] bytes)
        {
            if (_options.AllowedExtensions == null)
            {
                return true;
            }

            return _options.AllowedExtensions.Any(e => e == fileExtension);
        }

        public Dictionary<string, MemoryStream> Proccess<TEntity>(TEntity entity, Expression<Func<TEntity, string>> propertyExpression, byte[] bytes) where TEntity : class
        {
            return new Dictionary<string, MemoryStream> { { FilehookConsts.OriginalStyleName, new MemoryStream(bytes, false) } };
        }

        public MemoryStream ProccessStyle(byte[] bytes, IFileStyle style)
        {
            return new MemoryStream(bytes, false);
        }
    }
}
