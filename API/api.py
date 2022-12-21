from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from datetime import datetime as d

app = FastAPI()

origins = [
    "zoppellettosimone.github.io"
]

app.add_middleware(
    CORSMiddleware,
    allow_origins=origins,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

#Generic GET request
@app.get("")
async def root():
    print("[" + d.now().strftime("%Y-%m-%d %H:%M:%S") + "] GET Generic request")
    return {"response": "MyTCGCard API"}

#Route to find if the API work normally or not
@app.get("/iAmLive")
async def root():
    print("[" + d.now().strftime("%Y-%m-%d %H:%M:%S") + "] GET iAmLive request")
    return {"response": True}