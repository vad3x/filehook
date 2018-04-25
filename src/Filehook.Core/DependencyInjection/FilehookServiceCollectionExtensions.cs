using System;
using Filehook.Abstractions;
using Filehook.Core;
using Filehook.Core.DependencyInjection;
using Filehook.Core.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FilehookServiceCollectionExtensions
    {
        public static IFilehookBuilder AddFilehookCore(this IServiceCollection services, Action<FilehookOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = new FilehookBuilder(services);

            builder.Services.Configure(setupAction);

            builder.Services.AddTransient<IFilehookService, RegularFilehookService>();

            return builder;
        }

        public static IFilehookBuilder AddKebabLocationParamFormatter(this IFilehookBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddTransient<ILocationParamFormatter, KebabLocationParamFormatter>();

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

            builder.Services.AddTransient<ILocationTemplateParser, RegularLocationTemplateParser>();

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
