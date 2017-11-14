using System;
using Amazon;
using Amazon.S3;
using Filehook.Abstractions;
using Filehook.Core.DependencyInjection;
using Filehook.Storages.S3;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class S3FilehookBuilderExtensions
    {
        public static IFilehookBuilder AddS3Storage(this IFilehookBuilder builder, Action<S3StorageOptions> setupAction = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction != null)
            {
                builder.Services.Configure(setupAction);
            }

            builder.Services.Configure(setupAction);

            builder.Services.AddTransient<IFileStorage, S3Storage>();
            builder.Services.AddScoped<IAmazonS3>(x =>
            {
                var options = x.GetRequiredService<IOptions<S3StorageOptions>>().Value;

                var client = new AmazonS3Config();

                if (options.Region != null)
                {
                    client.RegionEndpoint = RegionEndpoint.GetBySystemName(options.Region);
                }

                return new AmazonS3Client(options.AccessKeyId, options.SecretAccessKey, client);
            });

            return builder;
        }
    }
}
