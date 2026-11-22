# Lösung zu Aufgabe 3 – Container und Passagiere gesamt in der Übersicht

Die Änderungen stehen als Patch in [aenderungen.patch](aenderungen.patch). Betroffen sind drei
Dateien: `CargoShip.cs`, `PassengerShip.cs` und `Harbor.cs`.

## Idee

1. **Lesezugriff schaffen:** Das Feld bleibt `private`, dazu kommt eine Property nur mit `get`
   (Python: `@property` ohne Setter):
   ```csharp
   public int ContainerNumber
   {
       get { return _containerNumber; }
   }
   ```
   Dasselbe gibt es als `PassengerNumber` in `PassengerShip`.
2. **Summieren in `PrintOverview`:** Eine Schleife über alle Schiffe. `is` prüft den echten Typ,
   der Cast `((CargoShip)ship)` macht aus dem `Ship` wieder ein `CargoShip`, damit man an
   `ContainerNumber` kommt:
   ```csharp
   int containers = 0;
   int passengers = 0;
   foreach (Ship ship in _ships)
   {
       if (ship is CargoShip)
       {
           containers += ((CargoShip)ship).ContainerNumber;
       }
       else if (ship is PassengerShip)
       {
           passengers += ((PassengerShip)ship).PassengerNumber;
       }
   }
   Console.WriteLine($"Container gesamt: {containers}");
   Console.WriteLine($"Passagiere gesamt: {passengers}");
   ```

## Warum so

- Ohne Cast kennt der Compiler nur `Ship`, und `Ship` hat kein `ContainerNumber`. In Python würde
  man das Attribut einfach abrufen, C# prüft den Typ schon beim Übersetzen.
- Die Summen beginnen bei `0`. Gibt es keine Schiffe eines Typs, steht deshalb automatisch `0` da
  (Anforderung 2).
- Kontrolle von Hand mit `ships.csv`: Container 120 + 340 + 650 + 1100 + 1800 = 4010,
  Passagiere 160 + 420 + 900 + 1800 + 3200 = 6480.

## Prüfen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 1-leicht/03-summen-in-uebersicht
```
