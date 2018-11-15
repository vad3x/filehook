namespace Filehook.Abstractions
{
    public class FilehookAttachment
    {
        public string Name { get; set; }

        public string EntityId { get; set; }

        public string EntityType { get; set; }

        public FilehookBlob Blob { get; set; }
    }
}
