﻿namespace Filehook.Storages.FileSystem
{
    public class FileSystemStorageOptions
    {
        public string Name { get; set; } = FileSystemConsts.FileSystemStorageName;

        public string CdnUrl { get; set; } = string.Empty;

        public string BasePath { get; set; } = string.Empty;
    }
}
