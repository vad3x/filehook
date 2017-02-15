namespace Filehook.Core.Tests.Fixtures
{
    public class EntityWithDefinedStorage
    {
        [UseStorage("regular")]
        public string Name { get; set; }
    }
}
