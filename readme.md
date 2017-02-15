# Filehook

Filehook is a file attachment library for dotnet inspired by [Paperclip](https://github.com/thoughtbot/paperclip).

# Quick Start

## Models

Mark properties with special attributes.

```csharp
    public class Article
    {
        public int Id { get; set; }

        [HasImageStyle("small", 0, 200)]
        [HasImageStyle("large", 0, 300)]
        public string CoverImageFileName { get; set; }

        public string AttachmentFileName { get; set; }
    }
```

## Startup.cs

```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFilehook(options =>
            {
                options.DefaultStorageName = FileSystemConsts.FileSystemStorageName;
            })
                .AddKebabLocationParamFormatter(o => o.Postfix = "FileName")
                .AddRegularLocationTemplateParser()
                .AddImageProccessor()
                .AddFallbackFileProccessor(o => o.AllowedExtensions = new[] { "pdf", "txt" })
                .AddFileSystemStorage(options =>
                {
                    options.BasePath = "./bin";
                    options.CdnUrl = "";
                });
        }
```

```csharp
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "./bin/public")),
                RequestPath = new PathString("/public")
            });
        }
```

# Image Proccessing

Filehook uses [ImageSharp](https://github.com/JimBobSquarePants/ImageSharp) internaly to proccess your images that marked with `[HasImageStyle]` attribute

# Storages

Only `FileSystemStorage` available for now. The storage allows to save files to file system

# TODO List

* Validators ?
* Tests

# License

The MIT License (MIT)
