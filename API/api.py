from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from datetime import datetime as d

app = FastAPI()

origins = [
    "*"
]

app.add_middleware(
    CORSMiddleware,
    allow_origins=origins,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

#Route to find if the API work normally or not
@app.get("/iAmLive")
async def root():
    print("[" + d.now().strftime("%Y-%m-%d %H:%M:%S") + "] iAmLive request")
    return {"response": True}