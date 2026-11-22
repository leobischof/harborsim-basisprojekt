# Lösung zu Aufgabe 7 – Neuer Schiffstyp Tanker

Die Änderung steht als Patch in [aenderungen.patch](aenderungen.patch). Betroffen sind die neue
Datei `Ships/TankerShip.cs` und in `Program.cs` ein neuer `case` in `ParseShip`.

## Idee

Ein neuer Schiffstyp ist eine neue **Unterklasse** von `Ship`, genau wie `CargoShip` und
`PassengerShip`. Der Rest des Programms (Harbor, StaffPool, Übersicht) arbeitet nur mit `Ship`
und muss deshalb gar nicht angefasst werden. Er ruft `LoadingStaff`, `GetDuration`,
`GetDescription` und `Load` auf, und C# nimmt automatisch die Version des Tankers
(Python: Methoden in der Unterklasse überschreiben).

```csharp
public class TankerShip : Ship                  // Python: class TankerShip(Ship):
{
    private int _cubicMeters;

    public TankerShip(string name, double length, double beam, double draft, int cubicMeters)
        : base(name, length, beam, draft)       // Python: super().__init__(...)
    {
        if (cubicMeters < 0)
        {
            throw new ArgumentException($"{name}: Die Ladung darf nicht negativ sein.");
        }
        _cubicMeters = cubicMeters;
    }
```

Das Personal holt sich zuerst den normalen Bedarf mit `base.LoadingStaff` und ergänzt dann die
Hafenarbeiter für die Schläuche mit der vorhandenen Hilfsmethode `AddStaffWithLeader`:

```csharp
SkillMatrix staff = base.LoadingStaff;
int count = (int)Math.Ceiling(_cubicMeters / 5000.0);
...
AddStaffWithLeader(staff, SkillType.DockWorker, count, leaderLevel);
```

`SkillMatrix.Add` zählt dazu: Die 3 neuen Hafenarbeiter kommen zu den 4 nach Länge hinzu.

Die Dauer:

```csharp
return TimeSpan.FromMinutes(Math.Ceiling(20 + _cubicMeters / 200.0));
```

In `ParseShip` kommt ein Fall dazu:

```csharp
case "Tanker":
    return new TankerShip(name, length, beam, draft, val1);
```

## Warum so

- **200.0 statt 200:** `3500 / 200` ist in C# `17` (ganzzahlig, Python `3500 // 200`), `3500 / 200.0`
  ist `17.5`. Mit `17` käme Polarstern auf 37 statt 38 Minuten. Genau das prüft `mit-tankern`.
  `20 + _cubicMeters / 200` ganz ohne Kommazahl kompiliert übrigens gar nicht: `Math.Ceiling`
  weiß dann nicht, ob es die `double`- oder die `decimal`-Version nehmen soll.
- **Exception im Konstruktor:** `LoadShips` fängt `ArgumentException` schon ab und gibt
  `Zeile N in ships.csv übersprungen: <Grund>` aus. Der Tanker muss nur werfen.
- **Kein neuer Skilltyp:** Für die Schläuche reichen Hafenarbeiter. Ein neuer `SkillType` müsste
  auch in `workers.csv` und im `StaffPool` vorkommen.

## Prüfen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 3-schwer/07-tanker
```
