package com.autogenai.llmservice.service;

import com.autogenai.llmservice.dto.*;

import java.util.stream.Stream;

import com.autogenai.llmservice.service.stubs.BedrockClient;
import com.autogenai.llmservice.service.stubs.OpenAiClient;
import org.springframework.stereotype.Service;


@Service
public class ChatService {
    private final OpenAiClient openAiClient;
    private final BedrockClient bedrockClient;

    public ChatService(OpenAiClient openAiClient, BedrockClient bedrockClient) {
        this.openAiClient = openAiClient;
        this.bedrockClient = bedrockClient;
    }


    public ChatResponse getChatResponse(ChatRequest request) {
        var streamedChatResponse = streamChatResponse(request);

        StringBuilder content = new StringBuilder();
        streamedChatResponse.stream().forEach(chunk -> content.append(chunk.delta().content()));

        return new ChatResponse("gpt-4o", content.toString());
    }

    public StreamedChatResponse streamChatResponse(ChatRequest request) {
        try {
            var stream = this.openAiClient.streamChat(new OpenAiClient.OpenAiRequest(
                    "gpt-4o",
                    request.messages().stream().map(message ->
                            new OpenAiClient.Message(
                                    message.role(),
                                    message.content().stream().map(contentPart -> new OpenAiClient.ContentPart(
                                            contentPart.type(),
                                            contentPart.content()
                                    )).toList()
                            )
                    ).toList(),
                    true,
                    request.temperature(),
                    request.maxTokens()
            ));
            return new StreamedChatResponse("gpt-4o", this.chunksFromOpenAiStream(stream));
        } catch (Exception e) {
            System.out.printf("Error streaming chat response %s", e.getMessage());

            var systemMessage = request.messages().stream()
                    .filter(message -> message.role().equals("system"))
                    .map(message -> message.content().getFirst().content())
                    .findFirst();

            var stream = this.bedrockClient.streamChat(new BedrockClient.BedrockRequest(
                    "anthropic.claude-3-sonnet-20240229-v1:0",
                    systemMessage,
                    request.messages().stream().map(message -> new BedrockClient.Message(
                            message.role(),
                            message.content().stream().map(contentPart -> new BedrockClient.ContentPart(
                                    contentPart.content()
                            )).toList()
                    )).toList(),
                    new BedrockClient.InferenceConfig(
                            request.temperature(),
                            request.maxTokens()
                    )
            ));
            return new StreamedChatResponse("claude-3-sonnet", this.chunksFromBedrockStream(stream));
        }
    }

    private Stream<Chunk> chunksFromOpenAiStream(Stream<OpenAiClient.OpenAiChunk> input) {
        return input.map(chunk -> new Chunk(
                new ChunkDelta(chunk.choices().stream().findFirst().get().delta().content())
        ));
    }

    private Stream<Chunk> chunksFromBedrockStream(Stream<BedrockClient.BedrockChunk> input) {
        return input.map(chunk -> new Chunk(
                new ChunkDelta(chunk.contentBlockDelta().delta().text())
        ));
    }
}
