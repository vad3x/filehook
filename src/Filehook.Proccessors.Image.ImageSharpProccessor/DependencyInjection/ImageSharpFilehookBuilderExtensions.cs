using Filehook.Abstractions;
using Filehook.Core.DependencyInjection;
using Filehook.Proccessors.Image.ImageSharpProccessor;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

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

            builder.Services.TryAddTransient<IImageTransformer, ImageSharpImageTransformer>();
            builder.Services.AddTransient<IFileProccessor, ImageSharpImageProccessor>();

            return builder;
        }
    }
}
