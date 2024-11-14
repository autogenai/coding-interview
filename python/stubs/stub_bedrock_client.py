from typing import Generator, Optional
from pydantic import BaseModel
import time


chunks = ["Hello ", "from ", "Bedrock!"]

class SystemMessage(BaseModel):
    text: str

class MessageContent(BaseModel):
    text: str

class Message(BaseModel):
    role: str
    content: list[MessageContent]

class ContentBlockDelta(BaseModel):
    delta: MessageContent

class ChatCompletionChunk(BaseModel):
    contentBlockDelta: ContentBlockDelta

class InferenceConfig(BaseModel):
    temperature: float
    max_tokens: int

def bedrock_chat_completion(model_id: str, system: Optional[list[SystemMessage]], messages: list[Message], inference_config: InferenceConfig) -> Generator[ChatCompletionChunk, None, None]:
    for chunk in chunks:
        yield ChatCompletionChunk(
            contentBlockDelta=ContentBlockDelta(
                delta=MessageContent(
                    text=chunk
                )
            )
        )
        time.sleep(1)