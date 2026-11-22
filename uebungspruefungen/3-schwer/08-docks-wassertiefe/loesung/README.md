# Lösung zu Aufgabe 8 – Docks mit Wassertiefe

Die Änderung steht als Patch in [aenderungen.patch](aenderungen.patch). Betroffen ist nur
`Simulation/Harbor.cs`.

## Idee

Neben dem Array `_dockedShips` (wer liegt an welchem Dock) kommt ein zweites Array
`_dockDepths` mit **demselben Index**: `_dockDepths[i]` ist die Tiefe von Dock `i + 1`.
Der Konstruktor füllt es mit einer Schleife:

```csharp
_dockDepths = new double[dockCount];            // Python: [0.0] * dock_count
for (int i = 0; i < dockCount; i++)
{
    _dockDepths[i] = FirstDockDepth + i * DockDepthStep;   // 6, 8.5, 11, ...
}
```

Drei kleine Methoden machen den Rest:

```csharp
private bool FitsDock(Ship ship, int index)      // passt das Schiff in Dock index?
{
    return ship.Draft < _dockDepths[index];
}

private bool FitsAnyDock(Ship ship) { ... }     // passt es in irgendein Dock?

private int FindFreeDock(Ship ship)             // erstes freies Dock, das passt, sonst -1
{
    for (int i = 0; i < _dockedShips.Length; i++)
    {
        if (_dockedShips[i] == null && FitsDock(ship, i))
        {
            return i;
        }
    }
    return -1;
}
```

`FindFreeDock` ersetzt das alte `HasFreeDock`:

- `CanStartTask` fragt `FindFreeDock(ship) < 0` statt `!HasFreeDock()`. Das Ergebnis hängt jetzt vom
  Schiff ab, deshalb braucht die Methode das Schiff als Parameter.
- `AddShipToDock` setzt das Schiff an `FindFreeDock(ship)`.

`PrintOverview` baut die Tiefen mit einer Schleife zu einem Text zusammen (Python wäre das
`", ".join(...)`) und gibt danach die Schiffe aus, für die `FitsAnyDock` `false` liefert.

## Warum so

- **Eine Methode für "welches Dock":** Start-Prüfung und Andocken müssen dieselbe Regel verwenden.
  Steht die Regel zweimal im Code, prüft `CanStartTask` irgendwann etwas anderes, als
  `AddShipToDock` macht. Dann wirft `AddShipToDock` die `InvalidOperationException`.
- **`-1` als "kein Dock":** Ein gültiger Index ist nie negativ (Python würde `None` zurückgeben).
- **Zahlen als Konstanten:** `6.0` und `2.5` stehen genau einmal oben in der Klasse.
- **Schiffe, die nirgends passen:** Für sie braucht es keinen Sonderfall in der Simulation. Ihr
  Docking-Task startet nie, also endet die Simulation mit der vorhandenen Blockiert-Meldung.

## Prüfen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 3-schwer/08-docks-wassertiefe
```
