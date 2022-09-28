from fastapi import FastAPI
from datetime import datetime as d

app = FastAPI()

#Route to find if the API work normally or not
@app.get("/iAmLive")
async def root():
    print("[" + datetime.now().strftime("%Y-%m-%d %H:%M:%S") + "] iAmLive request")
    return {"response": True}