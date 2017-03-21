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
            services.AddFilehook("./wwwroot", "http://localhost:5000");
        }
```

```csharp
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "./wwwroot/public")),
                RequestPath = new PathString("/public")
            });
        }
```

# Configuration

## Image Proccessing

Filehook uses [ImageSharp](https://github.com/JimBobSquarePants/ImageSharp) on module `Filehook.Proccessors.Image.ImageSharpProccessor` to proccess your images that marked with `[HasImageStyle]` attribute

## Storages

Only `FileSystemStorage` available for now. The storage allows to save files to file system

## Location

Default location template is `:base/public/:class/:attachmentName/:attachmentId/:style/:filename`

# TODO List

* Validators ?
* Tests

# License

The MIT License (MIT)
