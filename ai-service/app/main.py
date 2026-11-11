from fastapi import FastAPI

app = FastAPI(
    title="FixTrack AI Service",
    description="Repair cost and completion time prediction service",
    version="0.1.0",
)


@app.get("/health", tags=["Health"])
def health():
    return {"status": "healthy"}
