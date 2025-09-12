package com.autogenai.llmservice.dto;

import java.util.stream.Stream;

public record StreamedChatResponse(String model, Stream<Chunk> stream) {}
