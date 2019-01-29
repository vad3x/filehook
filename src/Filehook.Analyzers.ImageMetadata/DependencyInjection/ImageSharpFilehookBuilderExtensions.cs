using System;

using Filehook.Abstractions;
using Filehook.Analyzers.ImageMetadata;
using Filehook.Core.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ImageSharpFilehookBuilderExtensions
    {
        public static IFilehookBuilder AddImageMetadataBlobAnalyzer(this IFilehookBuilder builder, Action<ImageMetadataBlobAnalyzerOptions> setupAction = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            
            if (setupAction != null)
            {
                builder.Services.Configure(setupAction);
            }

            builder.Services.AddTransient<IBlobAnalyzer, ImageMetadataBlobAnalyzer>();

            return builder;
        }
    }
}
