using System;

namespace Filehook.Metadata.Builders
{
    public class ModelBuilder
    {
        public ModelBuilder()
        {
            Metadata = new ModelMetadata();
        }

        public ModelMetadata Metadata { get; private set; }

        public ModelBuilder Entity<TEntity>(Action<EntityTypeBuilder<TEntity>> buildAction) where TEntity : class
        {
            var type = typeof(TEntity);

            var entityMetadata = Metadata.AddType<TEntity>();

            buildAction(new EntityTypeBuilder<TEntity>(entityMetadata));

            return this;
        }
    }
}
