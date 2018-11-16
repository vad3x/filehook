using System;
using Filehook.Abstractions;
using Filehook.Core.DependencyInjection;
using Filehook.Proccessors.Image.ImageSharpProccessor;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ImageSharpFilehookBuilderExtensions
    {
        public static IFilehookBuilder AddImageSharpImageProccessor(this IFilehookBuilder builder, Action<ImageSharpImageProccessorOptions> action = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (action != null)
            {
                builder.Services.Configure(action);
            }

            builder.Services.TryAddTransient<IImageTransformer, ImageSharpImageTransformer>();
            builder.Services.AddTransient<IFileProccessor, ImageSharpImageProccessor>();
            builder.Services.AddTransient<IBlobMetadataExtender, ImageSharpImageBlobMetadataExtender>();

            return builder;
        }
    }
}
