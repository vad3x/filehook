namespace Filehook.Abstractions.Stores
{
    public class EntityMetadata
    {
        public EntityMetadata(string id, string type)
        {
            Id = id;
            Type = type;
        }

        public string Id { get; }

        public string Type { get; }
    }
}
