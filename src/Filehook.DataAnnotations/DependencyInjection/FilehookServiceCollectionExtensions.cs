using System;
using Filehook.Abstractions;
using Filehook.Core.DependencyInjection;
using Filehook.DataAnnotations;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FilehookServiceCollectionExtensions
    {
        public static IFilehookBuilder AddDataAnnotations(this IFilehookBuilder builder, Action<FileStorageNameResolverOptions> setupAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.Configure(setupAction);

            builder.Services.AddTransient<IFileStorageNameResolver, AttributeFileStorageNameResolver>();
            builder.Services.AddTransient<IFileStyleResolver, AttributeFileStyleResolver>();
            builder.Services.AddTransient<IParamNameResolver, AttributeParamNameResolver>();

            return builder;
        }
    }
}
