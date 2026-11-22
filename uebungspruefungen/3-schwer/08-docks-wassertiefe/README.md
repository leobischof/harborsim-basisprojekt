# Aufgabe 8 – Docks mit Wassertiefe

**Stufe:** schwer · **Zielzeit:** 20 Minuten · **Stoff:** Arrays, Methoden, Schleifen (UE03–UE06)

## Ausgangslage

Bisher passt jedes Schiff an jedes Dock. In Wirklichkeit sind die Docks unterschiedlich tief, und
ein Schiff mit großem Tiefgang würde im flachen Dock auf Grund laufen.

## Anforderungen

1. Jedes Dock hat eine **Wassertiefe**: Dock 1 ist 6 m tief, jedes weitere Dock 2,5 m tiefer
   (Dock 2: 8,5 m, Dock 3: 11 m, ...). Das gilt für jede Anzahl Docks, die im Menü eingestellt ist.
   Speichere die Tiefen in einem `double[]`.
2. Ein Schiff passt in ein Dock, wenn sein **Tiefgang kleiner** als die Wassertiefe ist.
3. Beim Andocken nimmt ein Schiff das **erste freie Dock, in das es passt**. Ist keines frei, wartet
   es (der Docking-Task startet noch nicht), genau wie bisher bei vollen Docks.
4. In der Übersicht wird die Zeile `Docks: 3` zu
   `Docks: 3 (Wassertiefe: 6 m, 8,5 m, 11 m)`.
5. Nach der Schiffsliste der Übersicht (vor `Mitarbeiter:`) steht für jedes Schiff, das in **kein**
   Dock passt, eine Zeile `Passt in kein Dock: <Name> (<Tiefgang> m Tiefgang)`, in der Reihenfolge
   der CSV. Passen alle, gibt es keine solche Zeile.
6. Ein Schiff, das in kein Dock passt, wird nie angedockt. Am Ende blockiert die Simulation mit
   seinen drei Tasks; das erledigt der vorhandene Code von selbst.

## Beispielausgabe

Mit den normalen Daten und 3 Docks:

```
=== Übersicht ===
Start: Montag, 23.11.2026 06:00
Docks: 3 (Wassertiefe: 6 m, 8,5 m, 11 m)
Schiffe, sortiert nach Länge:
  ...
Passt in kein Dock: Grand Meridian (12,1 m Tiefgang)
Mitarbeiter:
  ...
06:00  START  Docking   | Silver Mariner | 06:00-06:30 | Linda, Helena, Isaac, Bianca
       -> Silver Mariner liegt an Dock 2
...
08:25  START  Docking   | Oceanic Horizon | 08:25-08:55 | Martin, Helena, Isaac, Bianca
       -> Oceanic Horizon liegt an Dock 3
...
Simulation blockiert: Keiner der offenen Tasks kann gestartet werden.
Nicht durchführbare Tasks:
  Docking   | Grand Meridian
  Loading   | Grand Meridian
  Undocking | Grand Meridian
```

Oceanic Horizon (10,4 m Tiefgang) passt nur in Dock 3 und wartet, bis genau dieses frei wird,
obwohl Dock 2 schon vorher frei ist.

## Testszenarien

- `test/normale-daten/`: Eingaben `2`, `0`. Ganze Simulation mit 3 Docks.
- `test/fuenf-docks/`: Eingaben `1`, `3`, `5`, `0`, `2`, `0`. Erst 5 Docks einstellen, dann
  simulieren. Jetzt passt auch Grand Meridian (in Dock 4, 13,5 m).

## Hinweise

- Alles passiert in `Harbor.cs` (Ordner `Simulation/`). Die Docks sind dort schon ein Array:
  `_dockedShips` (`null` = frei). Ein zweites Array mit demselben Index passt gut dazu.
- Betroffen sind `HasFreeDock`, `AddShipToDock` und `PrintOverview`. Eine Methode, die den Index
  des passenden freien Docks liefert (oder `-1`), erspart doppelten Code.
- Ein `double` in `$"..."` wird mit Komma ausgegeben (`8,5`), weil das Programm auf Deutsch
  (Österreich) eingestellt ist. `6.0` erscheint als `6`.

## Testen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 3-schwer/08-docks-wassertiefe -Eigene
```
