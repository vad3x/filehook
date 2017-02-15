using Filehook.Core.DependencyInjection;
using Filehook.Proccessors.Image.ImageSharpProccessor;
using Filehook.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Filehook.Proccessors.Image.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ImageSharpFilehookBuilderExtensions
    {
        public static IFilehookBuilder AddImageProccessor(this IFilehookBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.TryAddTransient<IImageStyleResolver, AttributeImageStyleResolver>();
            builder.Services.TryAddTransient<IImageTransformer, ImageSharpImageTransformer>();
            builder.Services.AddTransient<IFileProccessor, ImageSharpImageProccessor>();

            return builder;
        }
    }
}
