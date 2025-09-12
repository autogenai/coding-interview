package com.autogenai.llmservice.service.stubs;

import org.springframework.stereotype.Service;

import java.util.Arrays;
import java.util.Date;
import java.util.List;
import java.util.Optional;
import java.util.stream.Stream;

@Service
public class OpenAiClient {
    public record Delta(String role, String content) {}
    public record Choice(Integer index, Delta delta, String finishReason) {}
    public record ContentPart(String type, String content) {}
    public record Message(String role, List<ContentPart> content) {}

    public record OpenAiRequest(
            String model,
            List<Message> messages,
            Boolean stream,
            Optional<Double> temperature,
            Optional<Integer> maxTokens
    ) {}

    public record OpenAiChunk(
            String id,
            List<Choice> choices,
            String model,
            String object,
            Date created
    ) {}

    public Stream<OpenAiChunk> streamChat(OpenAiRequest request) {
        String[] parts = {"Hello ", "from ", "Open", "AI"};
        return Arrays.stream(parts)
                .map(part -> new OpenAiChunk(
                        "test-id",
                        List.of(new Choice(0, new Delta(null, part), part.equals("AI") ? "stop" : null)),
                        request.model,
                        "chat.completion.chunk",
                        new Date()
                ));
    }
}
