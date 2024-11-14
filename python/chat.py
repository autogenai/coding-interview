from pydantic import BaseModel
from .stubs.stub_openai_client import openai_chat_completion
from .stubs.stub_bedrock_client import bedrock_chat_completion, InferenceConfig, Message as BedrockMessage, MessageContent as BedrockMessageContent
from typing import Generator


class MessageContent(BaseModel):
    type: str
    text: str

class Message(BaseModel):
    role: str
    content: list[MessageContent]

class ChatRequest(BaseModel):
    messages: list[Message]
    temperature: float | None
    max_tokens: int | None

class ChatResponse(BaseModel):
    model: str
    content: str

class ChatStreamChunkDelta(BaseModel):
    content: str

class ChatStreamChunk(BaseModel):
    delta: ChatStreamChunkDelta
    
    def encode(self, encoding):
        return self.delta.content.encode(encoding)

class ChatStreamResponse(BaseModel):
    model: str
    stream: Generator[ChatStreamChunk, None, None]

def get_chat_response(chat_request: ChatRequest) -> ChatResponse:
    response = stream_chat_response(chat_request)

    print(response)
    
    content = ''
    for chunk in response.stream:
        content += chunk.delta.content
    
    return ChatResponse(model=response.model, content=content)


def stream_chat_response(chat_request: ChatRequest) -> ChatStreamResponse:
    try:
        stream = openai_chat_completion(
            model='gpt-4o',
            messages=[message for message in chat_request.messages],
            temperature=chat_request.temperature, 
            max_tokens=chat_request.max_tokens,
            stream=True,
        )
        return ChatStreamResponse(model='gpt-4o', stream=chunks_from_openai_stream(stream))
    except Exception as err:
        print("Error streaming chat response from OpenAI", err)

        system_messages = [message for message in chat_request.messages if message.role == 'system']
        stream = bedrock_chat_completion(
            model_id='anthropic.claude-3-sonnet-20240229-v1:0',
            system=[{ text: system_messages[0].content[0].text }] if system_messages else None,
            messages=[BedrockMessage(
                role=message.role, 
                content=[
                    BedrockMessageContent(text=message.content[0].text)
                ]
            ) for message in chat_request.messages],
            inference_config=InferenceConfig(
                temperature=chat_request.temperature,
                max_tokens=chat_request.max_tokens
            )
        )   
        return ChatStreamResponse(model='claude-3-sonnet', stream=chunks_from_bedrock_stream(stream)) 


def chunks_from_openai_stream(stream) -> Generator[ChatStreamChunk, None, None]:
    for chunk in stream:
        yield ChatStreamChunk(
            delta=ChatStreamChunkDelta(
                content=chunk.choices[0].delta.content
            )
        )

def chunks_from_bedrock_stream(stream) -> Generator[ChatStreamChunk, None, None]:
    for chunk in stream:
        yield ChatStreamChunk(
            delta=ChatStreamChunkDelta(
                content=chunk.contentBlockDelta.delta.text
            )
        )

