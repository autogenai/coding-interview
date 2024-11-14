from openai import OpenAI, Stream
from openai.types import ChatModel
from openai.types.chat import ChatCompletionMessageParam, ChatCompletionChunk
from openai.types.chat.chat_completion_chunk import Choice, ChoiceDelta
from typing import Generator
import time
from datetime import datetime


chunks = ["Hello ", "from ", "OpenAI!"]

def openai_chat_completion(model: ChatModel, messages: list[ChatCompletionMessageParam], temperature: float, max_tokens: float, stream: bool) -> Generator[ChatCompletionChunk, None, None]:
    for index, chunk in enumerate(chunks):
        yield ChatCompletionChunk(
            id=str(index),
            choices=[
                Choice(
                    index=0,
                    delta=ChoiceDelta(
                        role="assistant",
                        content=chunk
                    )
                )
            ],
            created=datetime.now().timestamp().is_integer(),
            model=model,
            object="chat.completion.chunk",
        )
        time.sleep(1)
    
