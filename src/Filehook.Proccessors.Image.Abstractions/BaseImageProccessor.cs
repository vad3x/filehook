using Filehook.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Linq;

namespace Filehook.Proccessors.Image.Abstractions
{
    public abstract class BaseImageProccessor : IFileProccessor
    {
        protected IImageStyleResolver ImageStyleResolver { get; private set; }

        public BaseImageProccessor(IImageStyleResolver imageStyleResolver)
        {
            if (imageStyleResolver == null)
            {
                throw new ArgumentNullException(nameof(imageStyleResolver));
            }

            ImageStyleResolver = imageStyleResolver;
        }

        public Dictionary<string, MemoryStream> Proccess<TEntity>(TEntity entity, Expression<Func<TEntity, string>> propertyExpression, byte[] bytes) where TEntity : class
        {
            var styles = ImageStyleResolver.Resolve(propertyExpression);

            var result = styles.ToDictionary(style => style.Name, style => ProccessStyle(bytes, style));
            return result;
        }

        public abstract MemoryStream ProccessStyle(byte[] bytes, IFileStyle style);

        public abstract bool CanProccess(string fileExtension, byte[] bytes);
    }
}
