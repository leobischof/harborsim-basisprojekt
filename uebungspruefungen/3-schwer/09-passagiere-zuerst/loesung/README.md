# Lösung zu Aufgabe 9 – Passagierschiffe haben Vorrang

Die Änderung steht als Patch in [aenderungen.patch](aenderungen.patch). Betroffen sind
`Ships/Ship.cs`, `Ships/PassengerShip.cs`, `Simulation/Harbor.cs` und die neue Datei
`Ships/DockingOrderComparer.cs`.

## Idee

**1. Die Schiffe sagen selbst, ob sie Vorrang haben.** `Ship` bekommt eine virtuelle Property,
`PassengerShip` überschreibt sie:

```csharp
// Ship.cs
public virtual bool HasPriority
{
    get { return false; }
}

// PassengerShip.cs
public override bool HasPriority
{
    get { return true; }
}
```

In Python wäre das ein Klassenattribut `has_priority = False` in `Ship` und `True` in
`PassengerShip`.

**2. Der Comparer legt die Reihenfolge fest.** `Compare` liefert eine negative Zahl, wenn `x`
zuerst kommt, und eine positive, wenn `y` zuerst kommt:

```csharp
public int Compare(Ship x, Ship y)
{
    if (x.HasPriority && !y.HasPriority)
    {
        return -1;                  // x hat Vorrang, y nicht
    }
    if (!x.HasPriority && y.HasPriority)
    {
        return 1;                   // y hat Vorrang, x nicht
    }
    return x.Id.CompareTo(y.Id);    // gleicher Vorrang: CSV-Reihenfolge
}
```

Python: `ships.sort(key=lambda s: (not s.has_priority, s.id))`.

**3. `Harbor` sortiert eine Kopie, bevor es die Tasks anlegt:**

```csharp
List<Ship> taskOrder = new List<Ship>(_ships);   // Python: list(ships)
taskOrder.Sort(new DockingOrderComparer());
foreach (Ship ship in taskOrder)
{
    ... // drei Tasks wie bisher
}
```

Der Rest der Simulation bleibt, wie er ist: Sie geht `_pendingTasks` von vorne nach hinten durch
und startet alles, was gerade möglich ist.

## Warum so

- **Property statt `if (ship is PassengerShip)`:** Ein neuer Schiffstyp mit Vorrang (z. B. ein
  Rettungsschiff) überschreibt einfach `HasPriority`. `Harbor` muss dafür nicht geändert werden.
- **Eigene Comparer-Klasse:** genau wie `ShipComparer` für die Übersicht. So gibt es zwei
  Sortierregeln nebeneinander, ohne dass `Ship` eine davon fest einbaut.
- **Id als Gleichstand-Regel:** `List.Sort` ist nicht stabil. Bis 16 Elemente sortiert .NET zwar
  so, dass gleiche Elemente in Ruhe bleiben, ab 17 ist das nicht mehr sicher. Ohne den
  Id-Vergleich besteht `normale-daten` (10 Schiffe) trotzdem, `viele-schiffe` (18 Schiffe) aber
  nicht: Dort legt dann Nereus vor Helios an.
- **Kopie sortieren:** `_ships` bleibt in CSV-Reihenfolge. Wer die Liste sonst noch durchgeht,
  bekommt nicht plötzlich eine andere Reihenfolge.

## Prüfen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 3-schwer/09-passagiere-zuerst
```
