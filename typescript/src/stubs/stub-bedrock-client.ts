import {
  ConverseStreamOutput,
  ConverseStreamCommandInput,
} from "@aws-sdk/client-bedrock-runtime";

const chunks = ["Hello ", "from ", "Bedrock!"];

async function* stubGenerator(): AsyncGenerator<ConverseStreamOutput> {
  for (const [index, output] of chunks.entries()) {
    yield {
      contentBlockDelta: {
        contentBlockIndex: index,
        delta: {
          text: output,
        },
      },
    };
  }
}

export function bedrockChatCompletion(_: ConverseStreamCommandInput): {
  confidence: number;
  stream: AsyncIterable<ConverseStreamOutput>;
} {
  return {
    confidence: (Math.floor(Math.random() * 10) + 1) / 10,
    stream: stubGenerator(),
  };
}
