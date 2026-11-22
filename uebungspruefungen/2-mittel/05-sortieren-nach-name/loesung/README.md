# Lösung zu Aufgabe 5 – Schiffe nach Name sortieren

Die Änderung steht als Patch in [aenderungen.patch](aenderungen.patch). Betroffen sind
`Program.cs` (Menü und neue Methode `PrintShipsByName`) und die neue Datei
`Ships/ShipNameComparer.cs`.

## Idee

Eine zweite Sortierregel bekommt eine **eigene Comparer-Klasse**, genau wie `ShipComparer` für die
Länge:

```csharp
public class ShipNameComparer : IComparer<Ship>
{
    public int Compare(Ship x, Ship y)
    {
        return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
    }
}
```

`Compare` liefert eine negative Zahl, wenn `x` vor `y` kommt. In Python entspricht das grob
`sorted(ships, key=lambda s: s.name)`; C# fragt stattdessen immer zwei Schiffe gegeneinander ab.

Im Menü kommt `case 3` dazu, und `ReadNumber` erlaubt jetzt `0` bis `3`. Die neue Methode sortiert
eine **Kopie** der Liste:

```csharp
List<Ship> sorted = new List<Ship>(_menuShips);   // Python: sorted = list(menu_ships)
sorted.Sort(new ShipNameComparer());
```

## Warum so

- `Sort` verändert die Liste selbst. Würde man `_menuShips` direkt sortieren, wären die Schiffe
  unter `Verfügbarkeit ändern` → `Schiffe` danach anders nummeriert (Anforderung 4). Genau das
  prüft das Szenario `reihenfolge-bleibt`. Die Simulation merkt davon nichts, weil sie die CSV
  bei jedem Lauf frisch einliest.
- Eigene Klasse statt Änderung an `ShipComparer`: Die Länge-Sortierung der Übersicht soll bleiben.
- `Ordinal` vergleicht Zeichen für Zeichen, unabhängig von der Spracheinstellung des Rechners.

## Prüfen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 2-mittel/05-sortieren-nach-name
```
