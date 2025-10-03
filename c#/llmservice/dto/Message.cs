using System.Text.Json.Serialization;

namespace llmservice.dto
{
    public record Message([property: JsonPropertyName("role")] string Role, [property: JsonPropertyName("content")] List<ContentPart> Content);
}
