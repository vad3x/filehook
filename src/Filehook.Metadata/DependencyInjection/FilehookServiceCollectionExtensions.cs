using System;
using Filehook.Abstractions;
using Filehook.Core.DependencyInjection;
using Filehook.Metadata;
using Filehook.Metadata.Builders;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FilehookServiceCollectionExtensions
    {
        public static IFilehookBuilder AddMetadata(this IFilehookBuilder builder, Action<ModelBuilder> setupAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            var modelBuilder = new ModelBuilder();

            setupAction(modelBuilder);

            builder.Services.AddSingleton(modelBuilder.Metadata);

            builder.Services.AddTransient<IFileStorageNameResolver, MetadataFileStorageNameResolver>();
            builder.Services.AddTransient<IFileStyleResolver, MetadataFileStyleResolver>();
            builder.Services.AddTransient<IParamNameResolver, MetadataParamNameResolver>();

            return builder;
        }
    }
}
