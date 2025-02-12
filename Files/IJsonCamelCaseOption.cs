using System.Text.Json;

namespace OfficeFileAccessor.Files;

public interface IJsonCamelCaseOption
{
    JsonSerializerOptions Get();
}