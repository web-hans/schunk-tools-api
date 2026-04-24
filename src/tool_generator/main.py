"""
FastAPI-Einstiegspunkt für den Schunk Tool Generator Service.
Nimmt Werkzeug-Parameter entgegen und gibt STEP-Dateien zurück.
"""

from fastapi import FastAPI, HTTPException
from fastapi.responses import Response
from pydantic import BaseModel, Field

from tools import bohrer

app = FastAPI(
    title="Schunk Tool Generator",
    description="Generiert parametrisierte 3D-Modelle für spanende Werkzeuge als STEP-Dateien",
    version="1.0.0",
)


# ── Datenmodell für Bohrer-Parameter ─────────────────────────────────────────

class BohrerParams(BaseModel):
    d1: float = Field(..., gt=0, le=100,  description="Bohrdurchmesser in mm (z. B. 2.0)")
    L1: float = Field(..., gt=0, le=1000, description="Gesamtlänge in mm (z. B. 57.0)")
    L2: float = Field(..., gt=0, le=500,  description="Schneidenlänge in mm (z. B. 16.0)")
    L3: float = Field(..., gt=0, le=500,  description="Spannutenlänge in mm (z. B. 21.0)")
    d2: float = Field(..., gt=0, le=100,  description="Schaftdurchmesser in mm (z. B. 3.0)")
    schneidenwinkel: float = Field(
        default=140.0, ge=60, le=180,
        description="Schneidenwinkel (Kegelwinkel) an der Spitze in Grad"
    )


# ── Endpunkte ────────────────────────────────────────────────────────────────

@app.post("/bohrer", summary="Spiralbohrer als STEP-Datei generieren")
async def generate_bohrer(params: BohrerParams) -> Response:
    """
    Erstellt die Geometrie eines Spiralbohrers mit build123d und gibt
    das Ergebnis als binäre STEP-Datei zurück.
    """
    try:
        step_bytes = bohrer.generate(params.model_dump())
    except Exception as exc:
        raise HTTPException(
            status_code=500,
            detail=f"Fehler bei der Geometrie-Erstellung: {exc}"
        ) from exc

    filename = f"bohrer_d{params.d1}_L{params.L1}.step"
    return Response(
        content=step_bytes,
        media_type="application/octet-stream",
        headers={"Content-Disposition": f'attachment; filename="{filename}"'},
    )


@app.get("/health", summary="Healthcheck")
def health() -> dict:
    """Prüft ob der Service läuft. Wird von Docker-Compose und Load Balancern genutzt."""
    return {"status": "ok"}
