package com.autogenai.llmservice.service.stubs;

import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;
import java.util.stream.Stream;

@Service
public class BedrockClient {
    public record Delta(String text) {}
    public record ContentBlockDelta(Integer contentBlockIndex, Delta delta) {}

    public record ContentPart(String text) {}
    public record Message(String role, List<ContentPart> content) {}
    
    public record InferenceConfig(Optional<Double> temperature, Optional<Integer> maxTokens) {}
    
    public record BedrockRequest(
            String modelId,
            Optional<String> system,
            List<Message> messages,
            InferenceConfig inferenceConfig
    ) {}

    public record BedrockChunk(ContentBlockDelta contentBlockDelta) {}

    public Stream<BedrockChunk> streamChat(BedrockRequest request) {
        return Stream.of(
                "Hello ", "from ", "Bedrock"
        ).map(text -> new BedrockChunk(
                new ContentBlockDelta(0, new Delta(text))
        ));
    }
}
