using System.Text.Json.Serialization;

namespace llmservice.dto
{
    public record ChatRequest([property: JsonPropertyName("messages")] List<Message> Messages, [property: JsonPropertyName("temperature")] double? Temperature, [property: JsonPropertyName("maxTokens")] int? MaxTokens);
}
