# Filehook

Filehook is a file attachment library for dotnet inspired by [Paperclip](https://github.com/thoughtbot/paperclip).

# TODO List

* Validators ?
* Tests

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

# License

The MIT License (MIT)
