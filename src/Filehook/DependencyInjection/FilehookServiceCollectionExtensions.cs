using System;
using Filehook.Core.DependencyInjection;
using Filehook.Proccessors.Image.ImageSharpProccessor;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FilehookServiceCollectionExtensions
    {
        public static IFilehookBuilder AddFilehook(
            this IServiceCollection services,
            string defaultStorageName,
            Action<ImageSharpImageProccessorOptions> action = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = services.AddFilehookCore(options => {
                options.DefaultStorageName = defaultStorageName;
            });

            builder.AddDataAnnotations();

            builder.AddKebabLocationParamFormatter();

            builder.AddRegularLocationTemplateParser();
            builder.AddImageSharpImageProccessor(action);
            builder.AddFallbackFileProccessor();

            return builder;
        }
    }
}
