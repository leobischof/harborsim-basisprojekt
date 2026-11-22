# Lösung zu Aufgabe 10 – Schichtende nach 8 Stunden

Die Änderung steht als Patch in [aenderungen.patch](aenderungen.patch). Betroffen sind
`Staff/Worker.cs`, `Staff/StaffPool.cs`, `Simulation/Task.cs` und `Simulation/Harbor.cs`.

## Idee

**1. Der Mitarbeiter kennt seine Arbeitszeit und seine Grenze** (`Worker.cs`):

```csharp
private static readonly TimeSpan MaxWorkTime = TimeSpan.FromHours(8);

public TimeSpan WorkTime { get; private set; }   // im Konstruktor: TimeSpan.Zero

public bool HasTimeFor(TimeSpan duration)
{
    return WorkTime + duration <= MaxWorkTime;
}

public void StartTask(TimeSpan duration)          // vorher ohne Parameter
{
    ...
    IsBusy = true;
    WorkTime += duration;
}
```

`static readonly` ist eine Konstante, die erst beim Programmstart berechnet wird. `const` geht
hier nicht, weil `TimeSpan.FromHours(8)` ein Methodenaufruf ist.

**2. `Task.StartTask` gibt die Dauer weiter:** `worker.StartTask(GetDuration());`

**3. Die Auswahl fragt zusätzlich nach der Restzeit** (`StaffPool.cs`):

```csharp
public bool TrySelect(SkillMatrix requirement, TimeSpan duration, out List<Worker> selected)
...
bool fits = !worker.IsBusy
    && worker.SkillType == skillType
    && worker.SkillLevel >= skillLevel
    && !selected.Contains(worker)
    && worker.HasTimeFor(duration);
```

`Harbor.CanStartTask` ruft sie mit `ship.GetDuration(task.Type)` auf.

**4. Ausgabe:** `StaffPool.GetWorkTimeReport()` sortiert `_workers` (Level, dann Id), überspringt
alle mit `TimeSpan.Zero` und baut die Zeilen zusammen. `Harbor.PrintSummary` schreibt die
Überschrift und den Text gleich nach `Abgeschlossene Tasks`.

## Warum so

- **Die Regel steht beim Mitarbeiter:** `HasTimeFor` ist die einzige Stelle, die die 8 h kennt.
  `StaffPool` fragt nur nach und muss nicht wissen, wie die Regel lautet.
- **`<=` statt `<`:** Genau 8 h sind erlaubt. Das Szenario `genau-acht-stunden` prüft genau diese
  Grenze: Mit `<` dürfte Anton die Lena nicht mehr ablegen, und die Simulation endet anders.
- **Arbeitszeit beim Start zählen:** Die Auswahl muss schon vor dem Start wissen, ob der ganze
  Task noch in die Schicht passt. Würde man erst beim Ende addieren, wäre die Zeit eines gerade
  laufenden Tasks bei der nächsten Auswahl noch nicht dabei. Das stört hier nur deshalb nicht,
  weil ein beschäftigter Mitarbeiter sowieso nicht ausgewählt wird. Klarer ist es trotzdem so.
- **Bericht in `StaffPool`:** Nur dort ist die Liste aller Mitarbeiter. Das ist dasselbe Muster wie
  `GetDescription` für die Übersicht.

## Prüfen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 4-sehr-schwer/10-schichtende
```
