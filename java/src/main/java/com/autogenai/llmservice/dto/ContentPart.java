package com.autogenai.llmservice.dto;

import io.swagger.v3.oas.annotations.media.Schema;

@Schema(name = "ContentPart", description = "A part of a message")
public record ContentPart(String type, String content) {}
