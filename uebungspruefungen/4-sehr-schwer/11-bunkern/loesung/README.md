# Lösung zu Aufgabe 11 – Neuer Task Bunkern

Die Änderung steht als Patch in [aenderungen.patch](aenderungen.patch). Betroffen sind
`Simulation/TaskType.cs`, `Ships/Ship.cs`, `Simulation/Task.cs` und `Simulation/Harbor.cs`.

## Idee

**1. Neuer Wert im Enum** (`TaskType.cs`): `Bunkering` zwischen `Loading` und `Undocking`.

**2. Das Schiff weiß, was Bunkern braucht** (`Ship.cs`):

```csharp
public SkillMatrix BunkeringStaff
{
    get
    {
        SkillMatrix staff = new SkillMatrix();
        AddStaffWithLeader(staff, SkillType.DockWorker, 2, SkillLevel.Qualified);
        return staff;
    }
}

// in GetDuration:
case TaskType.Bunkering:
    return TimeSpan.FromMinutes(40);

public void Bunker()
{
    Console.WriteLine($"       -> {Name}: Treibstoff gebunkert");
}
```

**3. `Task.EndTask` ruft `Ship.Bunker()` auf**, wenn der Task ein Bunkering ist. Das ist derselbe
Weg wie `Ship.Load()` beim Loading.

**4. Der Hafen legt vier Tasks an und kennt die Reihenfolge-Regeln** (`Harbor.cs`):

```csharp
_pendingTasks.Add(new Task(TaskType.Bunkering, ship));   // nach Loading, vor Undocking

// in CanStartTask:
if (task.Type == TaskType.Bunkering && HasPendingTask(ship, TaskType.Loading))
{
    return false;
}
if (task.Type == TaskType.Undocking
    && (HasPendingTask(ship, TaskType.Loading) || HasPendingTask(ship, TaskType.Bunkering)))
{
    return false;
}
...
else if (task.Type == TaskType.Bunkering)
{
    requirement = ship.BunkeringStaff;
}
```

Aus `HasPendingLoadingTask(ship)` wird `HasPendingTask(ship, type)`. Der Tasktyp ist jetzt ein
Parameter, so braucht man nicht für jeden Typ eine eigene, fast gleiche Methode.

## Warum so

- **Bedarf und Dauer gehören zum Schiff:** Dort stehen auch `DockingStaff`, `LoadingStaff` und
  `GetDuration`. `BunkeringStaff` ist nicht `virtual`, weil die Regel für alle Schiffstypen gleich
  ist. `CanDoTask` muss nicht geändert werden: Alles außer Docking geht nur im angedockten Zustand,
  das passt auch fürs Bunkern.
- **Die Reihenfolge braucht beide Regeln.** Ohne die Bunkern-Regel würde ein Schiff bunkern, bevor
  es beladen ist (das Bunkering steht zwar später in der Liste, aber während des Beladens ist das
  Schiff nur beschäftigt, danach wird die Liste von vorne durchsucht). Ohne die Ablegen-Regel fällt
  der Fehler nur auf, wenn das Bunkern gerade kein Personal bekommt, das Ablegen aber schon. Genau
  das prüft das Szenario `ein-qualifizierter`: Quirin bunkert noch die Ida, also müsste die Jola
  sonst ohne Treibstoff ablegen.
- **Eine Hilfsmethode mit Parameter** statt zwei Kopien: `HasPendingTask` ist die einzige Stelle,
  die die offene Liste durchsucht.

## Prüfen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 4-sehr-schwer/11-bunkern
```
