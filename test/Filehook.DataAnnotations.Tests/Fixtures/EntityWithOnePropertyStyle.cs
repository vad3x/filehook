using Filehook.DataAnnotations.Abstractions;

namespace Filehook.DataAnnotations.Tests.Fixtures
{
    public class EntityWithOnePropertyStyle
    {
        [HasFileStyle("regular")]
        public string FileName { get; set; }
    }
}
