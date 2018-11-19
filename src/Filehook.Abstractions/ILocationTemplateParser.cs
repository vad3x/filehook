namespace Filehook.Abstractions
{
    public interface ILocationTemplateParser
    {
         string Parse(
             string className = null,
             string propertyName = null,
             string objectId = null,
             string style = null,
             string filename = null,
             string locationTemplate = null);

        string SetRoot(string locationTemplate, string rootLocation);
    }
}
