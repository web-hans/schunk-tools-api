# ── Python Tool Generator Service ─────────────────────────────────────────────
# build123d benötigt OCP (OpenCASCADE Python Binding), das als Pre-built Wheel
# geliefert wird. Das Image kann daher beim ersten Build einige Minuten brauchen.

FROM python:3.12-slim

WORKDIR /app

# Systemabhängigkeiten: libgomp wird von OCP/OpenCASCADE benötigt
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgomp1 \
    && rm -rf /var/lib/apt/lists/*

# Erst requirements, dann Code – damit Docker den pip-Layer cachen kann
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

EXPOSE 8000

# --workers 1 ist bewusst: build123d ist nicht thread-safe bei parallelen Aufrufen
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000", "--workers", "1"]
