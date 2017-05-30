namespace Filehook.Metadata
{
    public class PropertyBuilder<TProperty>
    {
        public PropertyBuilder<TProperty> HasName(string name)
        {
            return this;
        }

        public PropertyBuilder<TProperty> HasPostfix(string postfix)
        {
            return this;
        }

        public PropertyBuilder<TProperty> HasFileStyle(string name)
        {
            return this;
        }

        public PropertyBuilder<TProperty> HasImageStyle(string name)
        {
            return this;
        }

        public PropertyBuilder<TProperty> UseStorage(string storageName)
        {
            return this;
        }
    }
}