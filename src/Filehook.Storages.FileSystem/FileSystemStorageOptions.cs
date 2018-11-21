using System;
using System.IO;
using Filehook.Abstractions;

namespace Filehook.Storages.FileSystem
{
    public delegate string FileNameBuilder(string key, string checksum, FilehookFileInfo fileInfo);

    public delegate string LocationBuilder(
        string root,
        string normilizedEntityType,
        string normilizedEntityId,
        string normilizedAttachmentName,
        string blobKey,
        string normilizedFileName);

    public class FileSystemStorageOptions
    {
        public string Name { get; set; } = FileSystemConsts.FileSystemStorageName;

        public string HostUrl { get; set; } = string.Empty;

        public string Root { get; set; } = string.Empty;

        public LocationBuilder Location { get; set; } =
            (string root, string _, string __, string ___, string blobKey, string normilizedFileName)
                => $"{root}/{blobKey.Substring(0, 2)}/{blobKey.Substring(2, 2)}/{normilizedFileName}";

        public FileNameBuilder FileName { get; set; } =
            (string key, string _, FilehookFileInfo fileInfo) =>
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FileName);
                var fileExtension = Path.GetExtension(fileInfo.FileName).TrimStart('.');

                return $"{fileNameWithoutExtension.GenerateSlug()}-{key.Substring(0, 6)}.{fileExtension}";
            };
    }
}
