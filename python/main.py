from fastapi import FastAPI
from fastapi.responses import StreamingResponse, Response
from .chat import get_chat_response, stream_chat_response, ChatRequest

app = FastAPI()

@app.post('/chat')
def post_chat(chat_request: ChatRequest, response: Response):
    result = get_chat_response(chat_request=chat_request)
    response.headers['x-model'] = result.model
    return get_chat_response(chat_request=chat_request)

@app.post('/chat/stream')
def post_chat_stream(chat_request: ChatRequest, response: Response):
    result = stream_chat_response(chat_request=chat_request)
    headers = { 'x-model': result.model }
    return StreamingResponse(result.stream, headers=headers, media_type='text/plain')