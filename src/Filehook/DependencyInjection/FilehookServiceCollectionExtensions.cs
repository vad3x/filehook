using Filehook.Core.DependencyInjection;
using Filehook.Storages.FileSystem;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FilehookServiceCollectionExtensions
    {
        // params must be declared other way
        public static IFilehookBuilder AddFilehook(
            this IServiceCollection services,
            string locationParamPostfix = null,
            string storageBasePath = null,
            string storageCdnUrl = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = services.AddFilehookCore(options => {
                options.DefaultStorageName = FileSystemConsts.FileSystemStorageName;
            });

            builder.AddKebabLocationParamFormatter(options => {
                options.Postfix = locationParamPostfix;
            });

            builder.AddRegularLocationTemplateParser();
            builder.AddImageProccessor();
            builder.AddFallbackFileProccessor();
            builder.AddFileSystemStorage(options =>
            {
                options.BasePath = storageBasePath;
                options.CdnUrl = storageCdnUrl;
            });

            return builder;
        }
    }
}
