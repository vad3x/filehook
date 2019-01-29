using System;

using Filehook.Abstractions;
using Filehook.Analyzers.ImageSharp;
using Filehook.Core.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ImageSharpFilehookBuilderExtensions
    {
        public static IFilehookBuilder AddImageSharpBlobAnalyzer(this IFilehookBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddTransient<IBlobAnalyzer, ImageSharpBlobAnalyzer>();

            return builder;
        }
    }
}
