using Filehook.Core.DependencyInjection;
using Filehook.Core.Internal;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Filehook.Abstractions;
using Filehook.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FilehookServiceCollectionExtensions
    {
        public static IFilehookBuilder AddFilehook(this IServiceCollection services, Action<FileStorageNameResolverOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = new FilehookBuilder(services);

            builder.Services.Configure(setupAction);

            builder.Services.TryAddTransient<IFilehookService, RegularFilehookService>();

            builder.Services.TryAddTransient<IFileStorageNameResolver, AttributeFileStorageNameResolver>();

            return builder;
        }

        public static IFilehookBuilder AddKebabLocationParamFormatter(this IFilehookBuilder builder, Action<KebabLocationParamFormatterOptions> setupAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.Configure(setupAction);

            builder.Services.TryAddTransient<ILocationParamFormatter, KebabLocationParamFormatter>();

            return builder;
        }

        public static IFilehookBuilder AddRegularLocationTemplateParser(this IFilehookBuilder builder, Action<RegularLocationTemplateParserOptions> setupAction = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction != null)
            {
                builder.Services.Configure(setupAction);
            }

            builder.Services.TryAddTransient<ILocationTemplateParser, RegularLocationTemplateParser>();

            return builder;
        }

        public static IFilehookBuilder AddFallbackFileProccessor(this IFilehookBuilder builder, Action<FallbackFileProccessorOptions> setupAction = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction != null)
            {
                builder.Services.Configure(setupAction);
            }

            builder.Services.AddTransient<IFileProccessor, FallbackFileProccessor>();

            return builder;
        }
    }
}
