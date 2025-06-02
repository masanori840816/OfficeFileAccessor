using System.Text.Json;

namespace OfficeFileAccessor.Files;

public class JsonCamelCaseOption: IJsonCamelCaseOption
{
    private readonly JsonSerializerOptions options;
    public JsonCamelCaseOption()
    {
        this.options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }
    public JsonSerializerOptions Get()
    {
        return options;
    }
}