using System.Text.Json.Serialization;

namespace llmservice.dto
{
    public record StreamedChatResponse([property: JsonPropertyName("model")] string Model, [property: JsonPropertyName("stream")] IAsyncEnumerable<Chunk> Stream) { }
}
