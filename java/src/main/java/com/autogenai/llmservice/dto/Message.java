package com.autogenai.llmservice.dto;

import io.swagger.v3.oas.annotations.media.Schema;

import java.util.List;

@Schema(name = "Message", description = "A part of a conversation")
public record Message(String role, List<ContentPart> content) {}
