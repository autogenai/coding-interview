package com.autogenai.llmservice.controller;

import com.autogenai.llmservice.dto.ChatRequest;
import com.autogenai.llmservice.dto.ChatResponse;
import com.autogenai.llmservice.service.ChatService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.mvc.method.annotation.ResponseBodyEmitter;

@RestController
@RequestMapping("/chat")
public class ChatController {

    private final ChatService chatService;

    public ChatController(ChatService chatService) {
        this.chatService = chatService;
    }

    @Operation(summary = "Request a chat response")
    @ApiResponse(responseCode = "200", description = "Get text response", content = @Content(mediaType = MediaType.APPLICATION_JSON_VALUE))
    @PostMapping(value = "/", consumes = MediaType.APPLICATION_JSON_VALUE, produces = MediaType.APPLICATION_JSON_VALUE)
    public ChatResponse chat(@RequestBody ChatRequest request) {
        return chatService.getChatResponse(request);
    }

    @Operation(summary = "Stream a chat response")
    @ApiResponse(responseCode = "200", description = "Streaming text response", content = @Content(mediaType = MediaType.TEXT_PLAIN_VALUE))
    @PostMapping(value = "/stream", consumes = MediaType.APPLICATION_JSON_VALUE, produces = MediaType.TEXT_PLAIN_VALUE)
    public ResponseBodyEmitter streamChat(@RequestBody ChatRequest request) {
        var response = chatService.streamChatResponse(request);
        ResponseBodyEmitter emitter = new ResponseBodyEmitter(300000L);

        new Thread(() -> {
            try {
                response.stream().forEach(chunk -> {
                    try {
                        emitter.send(chunk.delta().content());
                    } catch (Exception e) {
                        emitter.completeWithError(e);
                    }
                });
                emitter.complete();
            } catch (Exception e) {
                emitter.completeWithError(e);
            }
        }).start();

        return emitter;
    }
}
