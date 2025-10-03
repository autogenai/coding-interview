using Microsoft.AspNetCore.Mvc;
using llmservice.Services;
using System.Net.Mime;
using llmservice.dto;

namespace llmservice.Controllers
{
    [ApiController]
    [Route("chat")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
        {
            var response = await _chatService.GetChatResponseAsync(request);
            return Ok(response);
        }

        [HttpPost("stream")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Text.Plain)]
        public async Task StreamChat([FromBody] ChatRequest request)
        {
            var response = HttpContext.Response;
            response.ContentType = MediaTypeNames.Text.Plain;
            response.Headers.Add("Cache-Control", "no-cache");
            response.Headers.Add("Connection", "keep-alive");
            response.Headers.Add("X-Content-Type-Options", "nosniff");

            await response.Body.FlushAsync();

            try
            {
                var streamedResponse = await _chatService.StreamChatResponseAsync(request);

                await foreach (var chunk in streamedResponse.Stream)
                {
                    if (chunk?.Delta?.Content is string content)
                    {
                        await response.WriteAsync(content);
                        await response.Body.FlushAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while streaming chat response.");
                throw;
            }
        }
    }
}