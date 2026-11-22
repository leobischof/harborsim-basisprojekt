# Aufgabe 10 – Schichtende nach 8 Stunden

**Stufe:** sehr schwer · **Zielzeit:** 25 Minuten · **Stoff:** Klassen, Properties, Methoden mit
Parametern, `TimeSpan`, Listen (UE03–UE07)

## Ausgangslage

Bisher arbeiten die Mitarbeiter, solange die Simulation läuft, auch zehn Stunden am Stück. Ab
jetzt gilt: Niemand arbeitet insgesamt mehr als **8 Stunden**.

## Anforderungen

1. Jeder Mitarbeiter merkt sich seine **Arbeitszeit**: die Summe der Dauer aller Tasks, an denen er
   mitgearbeitet hat. Am Anfang ist sie 0.
2. Die Dauer eines Tasks zählt **beim Start** voll zur Arbeitszeit aller eingeteilten Mitarbeiter
   (ein gestarteter Task läuft immer bis zum Ende).
3. Ein Mitarbeiter darf für einen Task nur ausgewählt werden, wenn er danach **höchstens 8 h**
   gearbeitet hat (Arbeitszeit + Dauer des Tasks ≤ 8 h). Genau 8 h sind also noch erlaubt.
   Passt er nicht, wird wie bisher der nächste passende Mitarbeiter genommen. Gibt es keinen
   mehr, kann der Task jetzt nicht starten.
4. In der Zusammenfassung steht direkt nach `Abgeschlossene Tasks: ...` die Zeile
   `Arbeitszeit je Mitarbeiter (höchstens 8 h):` und darunter je Mitarbeiter, der gearbeitet hat,
   eine Zeile `  <Name>: <h> h <min> min`. Mitarbeiter mit 0 min fehlen. Die Reihenfolge ist
   dieselbe wie bei der Personalauswahl: nach SkillLevel (Beginner zuerst), dann nach Id.
5. Kann wegen der Arbeitszeit am Ende nichts mehr starten, blockiert die Simulation mit der
   vorhandenen Meldung.

## Beispielausgabe

Mit den normalen Daten und 3 Docks (Ausschnitt):

```
13:17  START  Docking   | Grand Horizon | 13:17-13:47 | Martin, Elias, Freya, Gabriel
...
Simulation blockiert: Keiner der offenen Tasks kann gestartet werden.
Nicht durchführbare Tasks:
  Loading   | Grand Meridian
  Undocking | Grand Meridian
  Loading   | Grand Horizon
  Undocking | Grand Horizon

=== Zusammenfassung ===
Ende der Simulation: Montag, 23.11.2026 14:08
Dauer: 8 h 8 min
Abgeschlossene Tasks: 26 von 30
Arbeitszeit je Mitarbeiter (höchstens 8 h):
  Elias: 6 h 4 min
  Freya: 6 h 26 min
  Gabriel: 7 h 47 min
  ...
  Martin: 4 h 40 min
  Nora: 3 h 55 min
  Tina: 0 h 51 min
Nicht durchgeführte Tasks: 4 (Liste siehe oben)
Noch angedockt: Grand Meridian
Noch angedockt: Grand Horizon
```

Grand Horizon (316 m) braucht zum Beladen 7 Hafenarbeiter. So viele mit genug Restzeit gibt es
am Nachmittag nicht mehr.

## Testszenarien

- `test/normale-daten/`: Eingaben `2`, `0`. Ganze Simulation mit 3 Docks.
- `test/ein-dock/`: Eingaben `1`, `3`, `1`, `0`, `2`, `0`. Nur ein Dock, die Schiffe kommen
  nacheinander dran.
- `test/genau-acht-stunden/`: Eingaben `2`, `0`, eigene `ships.csv` (5 gleiche Frachtschiffe) und
  `workers.csv` (3 Hafenarbeiter, 1 Kranführer). Anton erreicht beim Ablegen der Lena genau 8 h 0 min.
  Das ist noch erlaubt, danach kann die Mara nicht mehr andocken.

## Hinweise

- Betroffen: `Worker` und `StaffPool` (Ordner `Staff/`), `Task` und `Harbor` (Ordner `Simulation/`).
- Die Personalauswahl (`StaffPool.TrySelect`) kennt bisher die Dauer des Tasks nicht. Sie muss sie
  bekommen. Die Dauer liefert `ship.GetDuration(task.Type)`.
- Rechnen mit Zeiträumen: `TimeSpan.Zero`, `TimeSpan.FromHours(8)`, `+`, `<=` funktionieren wie
  bei Zahlen. `(int)zeit.TotalHours` sind die ganzen Stunden, `zeit.Minutes` die restlichen
  Minuten (so wird auch schon `Dauer:` ausgegeben).
- `StaffPool` sortiert die Mitarbeiter bei jeder Auswahl mit `_workers.Sort()`, das ist genau die
  verlangte Reihenfolge.

## Testen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 4-sehr-schwer/10-schichtende -Eigene
```
