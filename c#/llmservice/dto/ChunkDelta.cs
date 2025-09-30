using System.Text.Json.Serialization;

namespace llmservice.dto
{
    public record ChunkDelta([property: JsonPropertyName("content")] string Content);
}
