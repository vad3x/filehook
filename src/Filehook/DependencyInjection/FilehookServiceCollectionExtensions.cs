﻿using System;
using Filehook.Core.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FilehookServiceCollectionExtensions
    {
        public static IFilehookBuilder AddFilehook(
            this IServiceCollection services,
            string defaultStorageName)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = services.AddFilehookCore();

            builder.AddDataAnnotations(options => {
                options.DefaultStorageName = defaultStorageName;
            });

            builder.AddKebabLocationParamFormatter();

            builder.AddRegularLocationTemplateParser();
            builder.AddImageProccessor();
            builder.AddFallbackFileProccessor();

            return builder;
        }
    }
}
