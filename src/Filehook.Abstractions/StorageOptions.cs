namespace Filehook.Abstractions
{
    public class StorageOptions
    {
        public delegate string LocationBuilder(string root, string blobKey);

        public virtual string Name { get; set; }

        public virtual LocationBuilder RelativeLocation { get; set; } = DefaultLocation;

        private static string DefaultLocation(string root, string blobKey)
        {
            return $"{root}/{blobKey.Substring(0, 2)}/{blobKey.Substring(2, 2)}/{blobKey}";
        }
    }
}
