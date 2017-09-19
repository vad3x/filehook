using System;
using Filehook.Abstractions;
using Filehook.Core.DependencyInjection;
using Filehook.Proccessors.Image.MagickNetProccessor;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ImageSharpFilehookBuilderExtensions
    {
        public static IFilehookBuilder AddImageMagickNetProccessor(this IFilehookBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.TryAddTransient<IImageTransformer, MagickNetImageTransformer>();
            builder.Services.AddTransient<IFileProccessor, MagickNetImageProccessor>();

            return builder;
        }
    }
}
