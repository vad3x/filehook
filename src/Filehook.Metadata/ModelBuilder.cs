using System;

namespace Filehook.Metadata
{
    public class ModelBuilder
    {
        public ModelBuilder Entity<TEntity>(Action<EntityTypeBuilder<TEntity>> buildAction) where TEntity : class
        {
            return this;
        }
    }
}
