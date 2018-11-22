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

        public LocationBuilder Location { get; set; } = DefaultLocation;

        private static string DefaultLocation(
            string root,
            string normilizedEntityType,
            string normilizedEntityId,
            string normilizedAttachmentName,
            string blobKey,
            string normilizedFileName)
        {
            return $"{root}/{blobKey.Substring(0, 2)}/{blobKey.Substring(2, 2)}/{blobKey}";
        }
    }
}
