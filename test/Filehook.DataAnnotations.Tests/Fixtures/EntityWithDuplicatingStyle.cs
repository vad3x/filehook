using Filehook.DataAnnotations.Abstractions;

namespace Filehook.DataAnnotations.Tests.Fixtures
{
    public class EntityWithDuplicatingStyleName
    {
        [HasFileStyle("regular")]
        [HasFileStyle("regular")]
        public string FileName { get; set; }
    }
}
