using System.Text.Json.Serialization;

namespace llmservice.dto
{
    public record ChatResponse([property: JsonPropertyName("model")] string Model, [property: JsonPropertyName("content")] string Content);
}
