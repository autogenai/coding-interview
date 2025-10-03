using llmservice.dto;
using llmservice.Services.Stubs;

namespace llmservice.Services
{
    public class ChatService : IChatService
    {
        private readonly IOpenAiClient _openAiClient;
        private readonly IBedrockClient _bedrockClient;
        private readonly ILogger<ChatService> _logger;

        public ChatService(IOpenAiClient openAiClient, IBedrockClient bedrockClient, ILogger<ChatService> logger)
        {
            _openAiClient = openAiClient;
            _bedrockClient = bedrockClient;
            _logger = logger;
        }

        public async Task<ChatResponse> GetChatResponseAsync(ChatRequest request)
        {
            var streamedResponse = await this.StreamChatResponseAsync(request);

            var contentBuilder = new System.Text.StringBuilder();

            await foreach (var chunk in streamedResponse.Stream)
            {
                if (chunk?.Delta?.Content is string content && !string.IsNullOrEmpty(content))
                {
                    contentBuilder.Append(content);
                }
            }

            return new ChatResponse(streamedResponse.Model, contentBuilder.ToString());
        }

        public async Task<StreamedChatResponse> StreamChatResponseAsync(ChatRequest request)
        {
            try
            {
                var openAiRequest = new IOpenAiClient.OpenAiRequest(
                    "gpt-4o",
                    request.Messages.Select(message =>
                        new IOpenAiClient.Message(
                            message.Role,
                            message.Content.Select(contentPart => new IOpenAiClient.ContentPart(
                                contentPart.Type,
                                contentPart.Content
                            )).ToList()
                        )
                    ).ToList(),
                    true,
                    request.Temperature,
                    request.MaxTokens
                );

                var openAiStream = _openAiClient.StreamChatAsync(openAiRequest);

                return new StreamedChatResponse(
                    Model: "gpt-4o",
                    Stream: ChunksFromOpenAiStream(openAiStream)
                );
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Error streaming chat response from OpenAI. Falling back to Bedrock.");

                var systemMessage = request.Messages.FirstOrDefault(m => m.Role.Equals("system", StringComparison.OrdinalIgnoreCase))
                                                    ?.Content.FirstOrDefault()?.Content;

                var bedrockMessages = request.Messages
                    .Where(m => !m.Role.Equals("system", StringComparison.OrdinalIgnoreCase))
                    .Select(message => new IBedrockClient.Message(
                        message.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase) ? "assistant" : "user",
                        message.Content.Select(contentPart => new IBedrockClient.ContentPart(
                            contentPart.Content
                        )).ToList()
                    )).ToList();

                var bedrockRequest = new IBedrockClient.BedrockRequest(
                    "anthropic.claude-3-sonnet-20240229-v1:0",
                    systemMessage,
                    bedrockMessages,
                    new IBedrockClient.InferenceConfig(
                        request.Temperature,
                        request.MaxTokens
                    )
                );

                var bedrockStream = _bedrockClient.StreamChatAsync(bedrockRequest);

                return new StreamedChatResponse(
                    Model: "claude-3-sonnet",
                    Stream: ChunksFromBedrockStream(bedrockStream)
                );
            }
        }

        private async IAsyncEnumerable<Chunk> ChunksFromOpenAiStream(IAsyncEnumerable<IOpenAiClient.OpenAiChunk> input)
        {
            await foreach (var chunk in input)
            {
                var content = chunk.Choices.FirstOrDefault()?.Delta.Content;

                if (!string.IsNullOrEmpty(content))
                {
                    yield return new Chunk(new ChunkDelta(content));
                }
            }
        }

        private async IAsyncEnumerable<Chunk> ChunksFromBedrockStream(IAsyncEnumerable<IBedrockClient.BedrockChunk> input)
        {
            await foreach (var chunk in input)
            {
                var content = chunk.ContentBlockDelta?.Delta?.Text;

                if (!string.IsNullOrEmpty(content))
                {
                    yield return new Chunk(new ChunkDelta(content));
                }
            }
        }
    }
}