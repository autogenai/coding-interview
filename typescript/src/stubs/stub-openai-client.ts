import { OpenAI } from "openai";
import { Stream } from "openai/streaming";

const chunks = ["Hello ", "from ", "OpenAI!"];

async function* stubGenerator(
  model: string
): AsyncGenerator<OpenAI.Chat.ChatCompletionChunk> {
  for (const [index, output] of chunks.entries()) {
    yield {
      id: index.toString(),
      choices: [
        {
          index: 0,
          delta: {
            role: "assistant",
            content: output,
          },
          finish_reason: null,
        },
      ],
      model: model,
      object: "chat.completion.chunk",
      created: Date.now(),
    };
  }
}

export function openAiChatCompletion(
  input: OpenAI.Chat.ChatCompletionCreateParamsStreaming
): {
  confidence: number;
  stream: Stream<OpenAI.Chat.ChatCompletionChunk>;
} {
  return {
    confidence: (Math.floor(Math.random() * 10) + 1) / 10,
    stream: new Stream(() => stubGenerator(input.model), new AbortController()),
  };
}
