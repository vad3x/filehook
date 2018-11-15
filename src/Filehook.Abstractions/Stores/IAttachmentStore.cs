using System.Threading.Tasks;

namespace Filehook.Abstractions.Stores
{
    public interface IAttachmentStore
    {
        Task<FilehookBlob[]> GetBlobsAsync(string name, string entityId, string entityType);
    }
}
