# Aufgabe 5 – Schiffe nach Name sortieren

**Stufe:** mittel · **Zielzeit:** 15 Minuten · **Stoff:** Klassen, Interfaces, Listen, Menüs (UE04–UE06)

## Ausgangslage

Die Hafenleitung sucht ein Schiff oft nach seinem Namen. Im Programm gibt es bisher nur eine
Liste, die nach **Länge** sortiert ist (in der Übersicht der Simulation).

## Anforderungen

1. Das Hauptmenü bekommt einen neuen Punkt `3  Schiffe nach Name` (zwischen `2` und `0`).
2. Punkt 3 gibt eine Leerzeile, die Überschrift `=== Schiffe nach Name ===` und danach alle
   eingelesenen Schiffe aus, **alphabetisch nach Name** sortiert. Jede Zeile hat zwei Leerzeichen
   Einrückung und dieselbe Beschreibung wie in der Übersicht.
3. Sortiert wird mit einer **eigenen Klasse**, die `IComparer<Ship>` implementiert.
   Verglichen wird zeichenweise (`StringComparison.Ordinal`).
4. Die Reihenfolge der Schiffe im Programm ändert sich dadurch **nicht**: Unter
   `Verfügbarkeit ändern` → `Schiffe` stehen die Schiffe danach weiter in der Reihenfolge der CSV.

## Beispielausgabe

```
=== Hauptmenü ===
1  Verfügbarkeit ändern
2  Simulation starten
3  Schiffe nach Name
0  Beenden
Auswahl: 3

=== Schiffe nach Name ===
  #1 Baltic Venture, 92 m lang, 16,2 m breit, 5,4 m Tiefgang, Frachtschiff mit 120 Containern
  #6 Coastal Breeze, 108 m lang, 18,2 m breit, 4,8 m Tiefgang, Passagierschiff mit 160 Passagieren
  #3 Emerald Trader, 194 m lang, 28,7 m breit, 8,8 m Tiefgang, Frachtschiff mit 650 Containern
  ...
  #2 Silver Mariner, 146 m lang, 22,8 m breit, 7,1 m Tiefgang, Frachtschiff mit 340 Containern
```

## Testszenarien

- `test/normale-daten/`: Eingaben `3`, `2`, `0`. Sortierte Liste, danach die ganze Simulation.
- `test/reihenfolge-bleibt/`: Eingaben `3`, `1`, `2`, `0`, `0`, `0`. Sortierte Liste, danach die
  Schiffsliste unter `Verfügbarkeit ändern` (Anforderung 4).

## Hinweise

- Das Hauptmenü steht in `Program.cs` (`Main`); die eingelesenen Schiffe liegen in `_menuShips`.
- Vorbild für den Vergleich ist `ShipComparer` im Ordner `Ships/` (sortiert nach Länge).
- `list.Sort(...)` sortiert die Liste selbst um (Python: `list.sort()`, nicht `sorted()`).
  Wie bekommst du eine sortierte Liste, ohne `_menuShips` zu verändern?
- `ReadNumber` lässt nur Zahlen in einem Bereich zu. Den musst du anpassen.

## Testen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 2-mittel/05-sortieren-nach-name -Eigene
```
