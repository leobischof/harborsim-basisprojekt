# Lösung zu Aufgabe 2 – Sicherheitsregel für große Passagierschiffe

Die Änderung steht als Patch in [aenderungen.patch](aenderungen.patch): nur die geänderten Zeilen,
`-` = Zeile fällt weg, `+` = Zeile kommt dazu.

## Idee

`PassengerShip.LoadingStaff` holt sich den Basisbedarf (Hafenarbeiter) aus `Ship`, ergänzt die
Passagierkoordinatoren und gibt die Matrix zurück. Genau dort, vor dem `return`, kommt die neue
Regel dazu:

```csharp
// Sicherheitsregel: über 1500 Passagiere zusätzlich ein Experten-Lotse
if (_passengerNumber > 1500)
{
    staff.Add(SkillType.Pilot, SkillLevel.Expert, 1);
}
return staff;
```

## Warum so

- Die Regel betrifft nur Passagierschiffe und nur das Beladen, also gehört sie in die
  `override`-Property von `PassengerShip`. `Ship` und `CargoShip` bleiben unberührt.
- `> 1500` und nicht `>= 1500`: "mehr als 1500" schließt 1500 selbst aus. Genau diesen Grenzfall
  prüft Ocean Dream im Testszenario.
- Den Rest erledigt der bestehende Code: `StaffPool.TrySelect` sucht für jede Zeile der Matrix
  passendes Personal. Fehlt der Lotse, startet der Task eben später.

## Prüfen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 1-leicht/02-lotse-grosse-passagierschiffe
```
