using Filehook.DataAnnotations.Abstractions;

namespace Filehook.DataAnnotations.Tests.Fixtures
{
    public class EntityWithDefinedStorage
    {
        [UseStorage("regular")]
        public string Name { get; set; }
    }
}
