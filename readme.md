# Filehook

Filehook is a file attachment library for dotnet inspired by [Paperclip](https://github.com/thoughtbot/paperclip).

# Quick Start

## Startup.cs

```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFilehook(FileSystemConsts.FileSystemStorageName);
            // TODO specify storage
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

## Models

### Data Annotations

Mark properties with special attributes:

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

### Metadata

The same using metadata:

```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFilehook(FileSystemConsts.FileSystemStorageName)
                .AddMetadata(builder => {
                    builder.Entity<Article>(entity => {
                        entity.HasName("MyArticle");

                        entity.Property(x => x.CoverImageFileName)
                            .HasName("FileName")
                            .HasImageStyle(new ImageStyle("small", new ImageResizeOptions { Height = 200 }))
                            .HasImageStyle(new ImageStyle("large", new ImageResizeOptions { Height = 300 }));
                    });
                });
        }
```

# Configuration

## Image Proccessing

Filehook uses [ImageSharp](https://github.com/JimBobSquarePants/ImageSharp) on module `Filehook.Proccessors.Image.ImageSharpProccessor` to proccess your images that marked with `[HasImageStyle]` attribute

## Storages

### FileSystemStorage

The storage allows to save files to file system.

```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFilehook(FileSystemConsts.FileSystemStorageName)
                .AddFileSystemStorage(options =>
                {
                    options.BasePath = "./wwwroot";
                    options.CdnUrl = "http://localhost:5000";
                });
        }
```

### S3

Allows to store files on S3.

```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFilehook(S3Consts.S3StorageName)
                 .AddS3Storage(options =>
                 {
                     options.AccessKeyId = Configuration["Filehook:S3:AccessKeyId"];
                     options.SecretAccessKey = Configuration["Filehook:S3:SecretAccessKey"];
                     options.BucketName = Configuration["Filehook:S3:BucketName"];
                     options.Region = Configuration["Filehook:S3:Region"];
                 });
        }
```
## Location

Default location template is `:base/public/:class/:attachmentName/:attachmentId/:style/:filename`

# TODO List

* Validators ?
* Tests

# License

The MIT License (MIT)
