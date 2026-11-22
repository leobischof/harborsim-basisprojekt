# Aufgabe 4 – CSV aus dem österreichischen Excel

**Stufe:** mittel · **Zielzeit:** 15 Minuten · **Stoff:** Strings, Arrays, Dateien einlesen (UE03, UE07)

## Ausgangslage

Die Hafenleitung pflegt die Listen jetzt in Excel und speichert sie als CSV. Ein österreichisch
eingestelltes Excel schreibt dabei **`;` als Trennzeichen** und **`,` als Dezimalzeichen**:

```
Name;Type;Length;Beam;Draft;Val1
Baltic Venture;Cargo;92;16,2;5,4;120
```

Das Basisprojekt erkennt die Kopfzeile nicht und überspringt jede Zeile.

## Anforderungen

1. `ships.csv` **und** `workers.csv` dürfen im Excel-Format vorliegen.
2. Erkannt wird das Format an der **Kopfzeile**: Enthält sie ein `;`, ist die ganze Datei im
   Excel-Format. Sonst ist sie im bisherigen Format.
3. Eine Datei im Excel-Format wird **genau so** eingelesen wie dieselbe Datei im normalen Format:
   gleiche Schiffe, gleiche Werte, gleiche Fehlermeldungen bei kaputten Zeilen.
4. Dateien im bisherigen Format funktionieren unverändert.

## Testdaten

`test/excel-format/` enthält `ships.csv` und `workers.csv` im Excel-Format, darunter eine kaputte
Zeile (`Broken Line;Cargo;100;abc;5;10`). `test/normales-format/` prüft die normalen Daten.

## Beispielausgabe (Anfang)

```
Zeile 6 in ships.csv übersprungen: Beam "abc" ist keine Zahl
HarborSim – Hafensimulation
4 Schiffe und 13 Mitarbeiter eingelesen.
...
Schiffe, sortiert nach Länge:
  #5 Baltic Venture, 92 m lang, 16,2 m breit, 5,4 m Tiefgang, Frachtschiff mit 120 Containern
```

Die Soll-Ausgabe wurde mit dem **unveränderten** Basisprojekt erzeugt, und zwar mit denselben
Daten im normalen Format. Genau das verlangt Anforderung 3.

## Hinweise

- Alle Zeilen einer Datei liest eine einzige Methode in `Program.cs`. Danach geht jede Zeile
  durch `FirstDataLine`, `CheckUniqueName` und `ParseShip`/`ParseWorker`.
- Es gibt zwei Wege: das Trennzeichen überall durchreichen, oder die Zeilen gleich nach dem
  Einlesen ins normale Format umbauen. Der zweite Weg ist deutlich kürzer.
- `string.Replace(alt, neu)` ersetzt alle Vorkommen (Python: `text.replace(alt, neu)`).
  Überleg dir, in welcher Reihenfolge du ersetzen musst.

## Testen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 2-mittel/04-excel-csv -Eigene
```
