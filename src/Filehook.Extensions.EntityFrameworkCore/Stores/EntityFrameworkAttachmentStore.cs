using System.Linq;
using System.Threading.Tasks;

using Dawn;

using Filehook.Abstractions;
using Filehook.Abstractions.Stores;

using Microsoft.EntityFrameworkCore;

namespace Filehook.Extensions.EntityFrameworkCore.Stores
{
    public class EntityFrameworkAttachmentStore : IAttachmentStore
    {
        private readonly IFilehookDbContext _filehookDbContext;

        public EntityFrameworkAttachmentStore(IFilehookDbContext filehookDbContext)
        {
            _filehookDbContext = Guard.Argument(filehookDbContext, nameof(filehookDbContext)).NotNull().Value;
        }

        public Task<FilehookBlob[]> GetBlobsAsync(string name, string entityId, string entityType)
        {
            Guard.Argument(name, nameof(name)).NotNull().NotEmpty();
            Guard.Argument(entityId, nameof(entityId)).NotNull().NotEmpty();
            Guard.Argument(entityType, nameof(entityType)).NotNull().NotEmpty();

            return _filehookDbContext.FilehookAttachments
                .Where(x => x.Name == name && x.EntityId == entityId && x.EntityType == entityType)
                .Select(x => (FilehookBlob)x.Blob)
                .ToArrayAsync();
        }
    }
}
