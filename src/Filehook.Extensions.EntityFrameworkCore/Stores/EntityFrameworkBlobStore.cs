using Dawn;

using Filehook.Abstractions.Stores;

using Microsoft.EntityFrameworkCore;

namespace Filehook.Extensions.EntityFrameworkCore.Stores
{
    public class EntityFrameworkBlobStore : IBlobStore
    {
        private readonly IFilehookDbContext _filehookDbContext;

        public EntityFrameworkBlobStore(IFilehookDbContext filehookDbContext)
        {
            _filehookDbContext = Guard.Argument(filehookDbContext, nameof(filehookDbContext)).NotNull().Value;
        }
    }
}
