using System.Text.Json.Serialization;

namespace llmservice.dto
{
    public record ContentPart([property: JsonPropertyName("type")] string Type, [property: JsonPropertyName("text")] string Content);
}
