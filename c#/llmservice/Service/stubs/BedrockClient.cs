using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace llmservice.Services.Stubs
{
    public interface IBedrockClient
    {
        IAsyncEnumerable<BedrockChunk> StreamChatAsync(BedrockRequest request);
        public record Delta(string Text);

        public record ContentBlockDelta(int ContentBlockIndex, Delta Delta);

        public record ContentPart(string Text);

        public record Message(string Role, List<ContentPart> Content);

        public record InferenceConfig(double? Temperature, int? MaxTokens);

        public record BedrockRequest(
            string ModelId,
            string? System,
            List<Message> Messages,
            InferenceConfig InferenceConfig
        );
        public record BedrockChunk(ContentBlockDelta ContentBlockDelta);
    }


    public class BedrockClient : IBedrockClient
    {
        public record Delta(string Text) { }

        public record ContentBlockDelta(int ContentBlockIndex, Delta Delta) { }

        public record ContentPart(string Text) { }

        public record Message(string Role, List<ContentPart> Content) { }

        public record InferenceConfig(double? Temperature, int? MaxTokens) { }

        public record BedrockRequest(
                string ModelId,
                string? System,
                List<Message> Messages,
                InferenceConfig InferenceConfig
            )
        { }
        public record BedrockChunk(ContentBlockDelta ContentBlockDelta) { }


        public async IAsyncEnumerable<IBedrockClient.BedrockChunk> StreamChatAsync(IBedrockClient.BedrockRequest request)
        {
            await Task.Delay(10);

            var textChunks = new List<string> { "Hello ", "from ", "Bedrock" };

            int index = 0;

            foreach (var text in textChunks)
            {
                await Task.Delay(50);

                yield return new IBedrockClient.BedrockChunk(
                    new IBedrockClient.ContentBlockDelta(index++, new IBedrockClient.Delta(text))
                );
            }
        }
    }
}