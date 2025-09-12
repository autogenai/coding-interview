package com.autogenai.llmservice.dto;

import io.swagger.v3.oas.annotations.media.Schema;

import java.util.List;
import java.util.Optional;

@Schema(name = "ChatRequest", description = "Request body for chat streaming")
public record ChatRequest(List<Message> messages, Optional<Double> temperature, Optional<Integer> maxTokens) {}


