using System;
using System.IO;
using Filehook.Abstractions;

namespace Filehook.Storages.FileSystem
{
    public class FileSystemStorageOptions
    {
        public string Name { get; set; } = FileSystemConsts.FileSystemStorageName;

        public string HostUrl { get; set; } = string.Empty;

        public string Root { get; set; } = string.Empty;

        public Func<string, string, FilehookFileInfo, string> FileName { get; set; } =
            (string key, string _, FilehookFileInfo fileInfo) =>
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FileName);
                var fileExtension = Path.GetExtension(fileInfo.FileName).TrimStart('.');

                return $"{fileNameWithoutExtension.GenerateSlug()}-{key.Substring(0, 6)}.{fileExtension}";
            };
    }
}
