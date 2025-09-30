using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace llmservice.Services.Stubs
{
    public interface IOpenAiClient
    {
        IAsyncEnumerable<OpenAiChunk> StreamChatAsync(OpenAiRequest request);
        public record Delta(string? Role, string? Content);

        public record Choice(int Index, Delta Delta, string? FinishReason);

        public record ContentPart(string Type, string Content);

        public record Message(string Role, List<ContentPart> Content);

        public record OpenAiRequest(
            string Model,
            List<Message> Messages,
            bool Stream,
            double? Temperature,
            int? MaxTokens
        );

        public record OpenAiChunk(
            string Id,
            List<Choice> Choices,
            string Model,
            string Object,
            DateTime Created
        );
    }

    public class OpenAiClient : IOpenAiClient
    {
        public record Delta(string? Role, string? Content) { }

        public record Choice(int Index, Delta Delta, string? FinishReason) { }

        public record ContentPart(string Type, string Content) { }

        public record Message(string Role, List<ContentPart> Content) { }

        public record OpenAiRequest(
            string Model,
            List<Message> Messages,
            bool Stream,
            double? Temperature,
            int? MaxTokens
        )
        { }

        public record OpenAiChunk(
            string Id,
            List<Choice> Choices,
            string Model,
            string Object,
            DateTime Created
        )
        { }
        public async IAsyncEnumerable<IOpenAiClient.OpenAiChunk> StreamChatAsync(IOpenAiClient.OpenAiRequest request)
        {
            string[] parts = { "Hello ", "from ", "Open", "AI" };

            foreach (var part in parts)
            {
                await Task.Delay(50);

                string? finishReason = part == "AI" ? "stop" : null;

                yield return new IOpenAiClient.OpenAiChunk(
                    Id: "test-id",
                    Choices: new List<IOpenAiClient.Choice>
                    {
                        new IOpenAiClient.Choice(
                            Index: 0,
                            Delta: new IOpenAiClient.Delta(
                                Role: null,
                                Content: part
                            ),
                            FinishReason: finishReason
                        )
                    },
                    Model: request.Model,
                    Object: "chat.completion.chunk",
                    Created: DateTime.UtcNow
                );
            }
        }
    }
}