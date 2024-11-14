import {
  chatRequestSchema,
  Chunk,
  getChatResponse,
  streamChatResponse,
} from "./chat.ts";

const CHAT_ROUTE = new URLPattern({ pathname: "/chat" });
const STREAM_CHAT_ROUTE = new URLPattern({ pathname: "/chat/stream" });

async function handleChatRequest(req: Request): Promise<Response> {
  const body = await req.json();
  const parsedBody = chatRequestSchema.safeParse(body);

  if (parsedBody.success) {
    const response = await getChatResponse(parsedBody.data);
    return new Response(JSON.stringify(response), {
      status: 200,
      headers: { "content-type": "application/json", "x-model": response.model },
    });
  } else {
    return new Response(JSON.stringify(parsedBody.error));
  }
}

async function handleStreamChatRequest(req: Request): Promise<Response> {
  const body = await req.json();
  const parsedBody = chatRequestSchema.safeParse(body);

  if (parsedBody.success) {
    const response = await streamChatResponse(parsedBody.data);
    return new Response(toUint8ArrayStream(response.stream), {
      status: 200,
      headers: { "content-type": "text/plain", "x-model": response.model },
    });
  } else {
    return new Response(JSON.stringify(parsedBody.error));
  }
}

function toUint8ArrayStream(
  iterable: AsyncIterable<Chunk>
): ReadableStream<Uint8Array> {
  return new ReadableStream<Uint8Array>({
    async start(controller) {
      for await (const chunk of iterable) {
        controller.enqueue(new TextEncoder().encode(JSON.stringify(chunk)));
      }
      controller.close();
    },
  });
}

Deno.serve({ port: 3000 }, (req) => {
  if (CHAT_ROUTE.test(req.url) && req.method === "POST") {
    return handleChatRequest(req);
  }
  if (STREAM_CHAT_ROUTE.test(req.url) && req.method === "POST") {
    return handleStreamChatRequest(req);
  }

  return new Response("Not Found", { status: 404 });
});
