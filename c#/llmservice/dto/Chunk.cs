using System.Text.Json.Serialization;

namespace llmservice.dto
{
    public record Chunk([property: JsonPropertyName("delta")] ChunkDelta Delta);
}
