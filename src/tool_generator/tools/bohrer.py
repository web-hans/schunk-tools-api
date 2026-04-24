"""
Geometrie-Modul für Spiralbohrer.

Erstellt ein vereinfachtes 3D-Modell eines Spiralbohrers bestehend aus:
  1. Schaft          – Zylinder mit Schaftdurchmesser d2
  2. Bohrerkörper    – Zylinder mit Bohrdurchmesser d1 (Spannutenbereich)
  3. Bohrerspitze    – Kegel mit dem angegebenen Schneidenwinkel

Hinweis: Spiralnuten (Spannuten) sind in dieser Version nicht modelliert.
Sie können in einer Erweiterung über build123d Helix + Sweep hinzugefügt werden.
"""

import math
import os
import tempfile

from build123d import (
    Align,
    BuildPart,
    Cone,
    Cylinder,
    Locations,
    export_step,
)


def generate(params: dict) -> bytes:
    """
    Generiert einen Spiralbohrer als STEP-Datei und gibt die rohen Bytes zurück.

    Parameter (alle Längenangaben in mm):
        d1              – Bohrdurchmesser (Durchmesser des Schneidenbereichs)
        L1              – Gesamtlänge des Bohrers
        L2              – Schneidenlänge (für spätere Nutgeometrie vorgemerkt)
        L3              – Spannutenlänge (Länge des Bereichs mit vollem Bohrdurchmesser)
        d2              – Schaftdurchmesser
        schneidenwinkel – Kegelwinkel an der Spitze in Grad (Standard: 140°)
    """
    d1: float = float(params["d1"])
    L1: float = float(params["L1"])
    L3: float = float(params["L3"])
    d2: float = float(params["d2"])
    winkel: float = float(params.get("schneidenwinkel", 140.0))

    # ── Geometrie-Berechnungen ────────────────────────────────────────────────

    # Halbwinkel von der Kegelachse zur Flanke (z. B. 140° → 70°)
    halbwinkel_rad = math.radians(winkel / 2.0)

    # Spitzenhöhe aus tan(α) = Radius / Höhe  →  Höhe = Radius / tan(α)
    spitzen_hoehe: float = (d1 / 2.0) / math.tan(halbwinkel_rad)

    # Schaft: alles unterhalb der Spannuten
    schaft_laenge: float = L1 - L3

    # Zylindrischer Spannutenbereich: Spannutenlänge abzüglich der Kegelspitze
    zyl_laenge: float = L3 - spitzen_hoehe

    # Plausibilitätsprüfung: Kegelspitze darf nicht länger als Spannute sein
    if zyl_laenge <= 0:
        raise ValueError(
            f"Spitzenhöhe ({spitzen_hoehe:.2f} mm) ist größer als Spannutenlänge "
            f"({L3} mm). Bitte Durchmesser d1 oder Schneidenwinkel anpassen."
        )

    # ── Geometrie-Aufbau mit build123d ────────────────────────────────────────
    #
    # Koordinatensystem: Bohrer steht entlang der Z-Achse.
    #   z = 0           → unteres Ende des Schafts
    #   z = schaft_laenge → Übergang Schaft → Bohrerkörper
    #   z = schaft_laenge + zyl_laenge → Basis der Kegelspitze
    #   z = L1          → Spitze (Apex des Kegels)

    with BuildPart() as bohrer:

        # 1. Schaft ──────────────────────────────────────────────────────────
        # Zylinder mit dem größeren Schaftdurchmesser d2.
        # align=MIN bedeutet: Basis des Zylinders liegt am aktuellen Locations-Punkt.
        with Locations((0, 0, 0)):
            Cylinder(
                radius=d2 / 2.0,
                height=schaft_laenge,
                align=(Align.CENTER, Align.CENTER, Align.MIN),
            )

        # 2. Bohrerkörper (Spannutenbereich) ─────────────────────────────────
        # Zylinder mit dem kleineren Bohrdurchmesser d1, direkt auf dem Schaft.
        with Locations((0, 0, schaft_laenge)):
            Cylinder(
                radius=d1 / 2.0,
                height=zyl_laenge,
                align=(Align.CENTER, Align.CENTER, Align.MIN),
            )

        # 3. Kegelspitze ──────────────────────────────────────────────────────
        # Kegel mit bottom_radius=d1/2 und top_radius=0 (echte Spitze).
        # Der Kegelwinkel ergibt sich implizit aus Radius und Höhe.
        with Locations((0, 0, schaft_laenge + zyl_laenge)):
            Cone(
                bottom_radius=d1 / 2.0,
                top_radius=0.0,
                height=spitzen_hoehe,
                align=(Align.CENTER, Align.CENTER, Align.MIN),
            )

    # ── Export als STEP ───────────────────────────────────────────────────────
    # Temporäre Datei verwenden, damit keine Rückstände auf dem Dateisystem bleiben.
    tmp_path: str | None = None
    try:
        with tempfile.NamedTemporaryFile(suffix=".step", delete=False) as tmp:
            tmp_path = tmp.name

        export_step(bohrer.part, tmp_path)

        with open(tmp_path, "rb") as step_file:
            return step_file.read()

    finally:
        # Temporäre Datei immer aufräumen, auch bei Fehlern
        if tmp_path and os.path.exists(tmp_path):
            os.unlink(tmp_path)
