# Aufgabe 11 – Neuer Task Bunkern

**Stufe:** sehr schwer · **Zielzeit:** 25 Minuten · **Stoff:** Enums, `switch`, Properties,
Methoden mit Parametern, Listen (UE03–UE07)

## Ausgangslage

Jedes Schiff durchläuft bisher drei Tasks: Docking, Loading, Undocking. Bevor ein Schiff den Hafen
verlässt, muss es aber noch Treibstoff tanken. Das heißt **Bunkern**.

## Anforderungen

1. Es gibt einen neuen Tasktyp `Bunkering`. Jedes Schiff bekommt jetzt **vier** Tasks in dieser
   Reihenfolge: Docking, Loading, Bunkering, Undocking.
2. Bunkern dauert für jedes Schiff **40 Minuten**.
3. Zum Bunkern braucht man **2 Hafenarbeiter, einer davon mindestens qualifiziert** (Qualified).
   Das gilt für alle Schiffstypen gleich.
4. Bunkern geht nur, wenn das Schiff angedockt ist, gerade nichts anderes macht und
   **fertig beladen** ist (kein Loading mehr offen).
5. Ablegen geht erst, wenn das Schiff beladen **und gebunkert** ist.
6. Am Ende des Bunkerns erscheint die Zeile `       -> <Name>: Treibstoff gebunkert`
   (7 Leerzeichen, wie die anderen `->`-Zeilen).
7. Die Zusammenfassung zählt die neuen Tasks automatisch mit (`... von 40` bei 10 Schiffen).

## Beispielausgabe

Mit den normalen Daten und 3 Docks (Ausschnitt):

```
06:30  START  Loading   | Baltic Venture | 06:30-07:12 | Elias, Bianca, Oliver
...
07:12  ENDE   Loading   | Baltic Venture | 06:30-07:12 | Elias, Bianca, Oliver
       -> Baltic Venture: 120 Container verladen
07:12  START  Bunkering | Baltic Venture | 07:12-07:52 | Elias, Bianca
...
07:52  ENDE   Bunkering | Baltic Venture | 07:12-07:52 | Elias, Bianca
       -> Baltic Venture: Treibstoff gebunkert
...
Simulation blockiert: Keiner der offenen Tasks kann gestartet werden.
Nicht durchführbare Tasks:
  Loading   | Grand Meridian
  Bunkering | Grand Meridian
  Undocking | Grand Meridian

=== Zusammenfassung ===
Ende der Simulation: Montag, 23.11.2026 17:16
Dauer: 11 h 16 min
Abgeschlossene Tasks: 37 von 40
Nicht durchgeführte Tasks: 3 (Liste siehe oben)
Noch angedockt: Grand Meridian
```

Grand Meridian blockiert wie im Basisprojekt schon beim Beladen.

## Testszenarien

- `test/normale-daten/`: Eingaben `2`, `0`. Ganze Simulation mit 3 Docks.
- `test/ein-dock/`: Eingaben `1`, `3`, `1`, `0`, `2`, `0`. Nur ein Dock.
- `test/ein-qualifizierter/`: Eingaben `2`, `0`, eigene `ships.csv` (Ida und Jola, je 50 m) und
  `workers.csv` (nur Quirin ist qualifiziert). Jola ist um 07:40 fertig beladen, Quirin bunkert da
  noch die Ida. Jola muss bis 07:45 warten und darf nicht vorher ablegen, obwohl genug Anfänger
  zum Ablegen frei wären.

## Hinweise

- Betroffen: `TaskType`, `Task` und `Harbor` (Ordner `Simulation/`), `Ship` (Ordner `Ships/`).
- Bei einem `enum` legt die Reihenfolge der Werte nichts fest. Die Reihenfolge der Tasks entsteht
  dort, wo `Harbor` sie in `_pendingTasks` einfügt.
- Die Dauer steht in `Ship.GetDuration` (ein `switch` über den Tasktyp).
- Für den Personalbedarf gibt es in `Ship` schon die Hilfsmethode `AddStaffWithLeader`
  (einer mit bestimmtem Level, die anderen Beginner).
- `Harbor.CanStartTask` entscheidet, ob ein Task starten darf und welches Personal er braucht.
  Dort steht schon die Regel „Ablegen erst nach dem Beladen“. Die Hilfsmethode dafür lässt sich so
  umbauen, dass sie für jeden Tasktyp funktioniert.

## Testen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 4-sehr-schwer/11-bunkern -Eigene
```
