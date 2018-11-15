namespace Filehook.Abstractions
{
    public interface IEntityIdResolver
    {
         string Resolve<TEntity>(TEntity entity);
    }
}
