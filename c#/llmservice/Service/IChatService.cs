using llmservice.dto;

namespace llmservice.Services
{
    public interface IChatService
    {
        Task<ChatResponse> GetChatResponseAsync(ChatRequest request);
        Task<StreamedChatResponse> StreamChatResponseAsync(ChatRequest request);
    }
}