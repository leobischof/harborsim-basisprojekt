# Aufgabe 6 – Fehlerprotokoll in eine Datei

**Stufe:** mittel · **Zielzeit:** 15 Minuten · **Stoff:** Listen, Dateien schreiben, statische Felder (UE05, UE07)

## Ausgangslage

Kaputte Zeilen in `ships.csv` oder `workers.csv` meldet das Programm nur auf der Konsole. Sobald
das Menü erscheint, sind die Meldungen weggescrollt. Die Hafenleitung will sie **in einer Datei**
nachlesen können.

## Anforderungen

1. Beim Programmstart schreibt das Programm die Datei **`fehler.log`** neben die `.exe`
   (gleicher Ordner wie der Ordner `Data/`).
2. In `fehler.log` steht jede übersprungene Zeile mit **genau derselben Meldung** wie auf der
   Konsole, eine Meldung pro Zeile, in derselben Reihenfolge. Der Hinweis zur fehlenden Kopfzeile
   gehört **nicht** dazu.
3. Die Datei wird bei jedem Start **neu geschrieben**, nicht angehängt. Ohne Fehler ist sie leer
   (sie wird trotzdem geschrieben).
4. Gibt es mindestens eine übersprungene Zeile, steht direkt nach der Zeile
   `… Mitarbeiter eingelesen.` zusätzlich: `N fehlerhafte Zeilen in fehler.log protokolliert.`
5. Die Simulation liest die CSV-Dateien bei jedem Lauf noch einmal ein. Das darf `fehler.log`
   **nicht** verändern: Jede Meldung steht dort genau einmal.

## Beispielausgabe (Daten aus `test/kaputte-daten/`)

```
Hinweis: ships.csv hat keine Kopfzeile, Zeile 1 wird als Daten gelesen.
Zeile 2 in ships.csv übersprungen: Name "Baltic Venture" kommt doppelt vor
...
Zeile 7 in workers.csv übersprungen: zu wenige Spalten (erwartet: Name,SkillType,SkillLevel)
HarborSim – Hafensimulation
2 Schiffe und 2 Mitarbeiter eingelesen.
11 fehlerhafte Zeilen in fehler.log protokolliert.
```

`fehler.log` danach:

```
Zeile 2 in ships.csv übersprungen: Name "Baltic Venture" kommt doppelt vor
...
Zeile 7 in workers.csv übersprungen: zu wenige Spalten (erwartet: Name,SkillType,SkillLevel)
```

## Testszenarien

Jedes Szenario hat einen Ordner `erwartete-dateien/` mit der Soll-Datei `fehler.log`.

- `test/kaputte-daten/`: kaputte CSV-Dateien, Eingabe `0`.
- `test/kaputte-daten-mit-lauf/`: dieselben Daten, Eingaben `2`, `0` (Anforderung 5).
- `test/normale-daten/`: normale Daten, Eingabe `0`. `fehler.log` muss leer sein.

## Hinweise

- Alle Meldungen über übersprungene Zeilen laufen durch eine einzige Methode in `Program.cs`.
- Den Pfad neben der `.exe` liefert `AppContext.BaseDirectory`. So sind auch `ShipsPath` und
  `WorkersPath` gebaut.
- `File.WriteAllLines(pfad, liste)` schreibt eine `List<string>` Zeile für Zeile in eine Datei
  und überschreibt sie dabei (Python: `open(pfad, "w").write("\n".join(liste))`).
- Überleg dir, **wann** du die Datei schreibst, damit Anforderung 5 stimmt.

## Testen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 2-mittel/06-fehlerprotokoll -Eigene
```
