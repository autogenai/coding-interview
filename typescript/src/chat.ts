import { z } from "npm:zod";
import { openAiChatCompletion } from "./stubs/stub-openai-client.ts";
import { bedrockChatCompletion } from "./stubs/stub-bedrock-client.ts";
import OpenAI from "openai";
import { Stream } from "openai/streaming";
import { ConverseStreamOutput, Message } from "@aws-sdk/client-bedrock-runtime";

const contentPart = z.object({
  type: z.literal("text"),
  text: z.string(),
});

const messageSchema = z.object({
  role: z.enum(["user", "assistant", "system"]),
  content: z.array(contentPart),
});

export const chatRequestSchema = z.object({
  messages: z.array(messageSchema),
  temperature: z.number().optional(),
  max_tokens: z.number().optional(),
});
type ChatRequest = z.infer<typeof chatRequestSchema>;

interface ChatResponse {
  model: string;
  content: string;
  confidence?: number;
}

export interface Chunk {
  delta: {
    content: string;
  };
}

export async function getChatResponse(
  chatRequest: ChatRequest
): Promise<ChatResponse> {
  const response = await streamChatResponse(chatRequest);

  let content = "";
  for await (const chunk of response.stream) {
    content += chunk.delta.content;
  }

  return {
    model: "gpt-4o",
    confidence: response.confidence,
    content,
  };
}

export async function streamChatResponse(chatRequest: ChatRequest): Promise<{
  model: string;
  confidence?: number;
  stream: AsyncIterable<Chunk>;
}> {
  try {
    const { confidence, stream } = await openAiChatCompletion({
      model: "gpt-4o",
      messages: chatRequest.messages,
      stream: true,
      temperature: chatRequest.temperature,
      max_tokens: chatRequest.max_tokens,
    });
    return {
      model: "gpt-4o",
      confidence,
      stream: chunksFromOpenAiStream(stream),
    };
  } catch (e) {
    console.warn("Error streaming chat response from OpenAI", e);

    const systemMessage = chatRequest.messages.find(
      (message) => message.role === "system"
    );
    const { confidence, stream } = await bedrockChatCompletion({
      modelId: "anthropic.claude-3-sonnet-20240229-v1:0",
      system: systemMessage
        ? [
            {
              text: systemMessage.content[0].text,
            },
          ]
        : undefined,
      messages: chatRequest.messages.map(
        (message) =>
          ({
            role: "user",
            content: [
              {
                text: message.content[0].text,
              },
            ],
          } as Message)
      ),
      inferenceConfig: {
        temperature: chatRequest.temperature,
        maxTokens: chatRequest.max_tokens,
      },
    });
    return {
      model: "claude-3-sonnet",
      confidence,
      stream: chunksFromBedrockStream(stream),
    };
  }
}

async function* chunksFromOpenAiStream(
  stream: Stream<OpenAI.Chat.ChatCompletionChunk>
): AsyncGenerator<Chunk> {
  for await (const chunk of stream) {
    yield {
      delta: {
        content: chunk.choices[0].delta.content!,
      },
    };
  }
}

async function* chunksFromBedrockStream(
  stream: AsyncIterable<ConverseStreamOutput>
): AsyncGenerator<Chunk> {
  for await (const chunk of stream) {
    yield {
      delta: {
        content: chunk.contentBlockDelta!.delta!.text!,
      },
    };
  }
}
