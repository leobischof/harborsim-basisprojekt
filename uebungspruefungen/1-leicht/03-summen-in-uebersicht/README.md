# Aufgabe 3 – Container und Passagiere gesamt in der Übersicht

**Stufe:** leicht · **Zielzeit:** 10 Minuten · **Stoff:** Properties, Schleifen, `is` und Cast (UE03–UE05)

## Ausgangslage

Die Hafenleitung will vor dem Start der Simulation auf einen Blick sehen, wie viel Arbeit ansteht:
wie viele Container insgesamt verladen werden und wie viele Passagiere an Bord gehen.

## Anforderungen

1. In der Übersicht (`=== Übersicht ===`) stehen direkt **nach der Schiffsliste** und **vor**
   `Mitarbeiter:` zwei neue Zeilen:
   ```
   Container gesamt: <Summe der Container aller Frachtschiffe>
   Passagiere gesamt: <Summe der Passagiere aller Passagierschiffe>
   ```
2. Gibt es keine Schiffe eines Typs, steht dort `0`.
3. Die Anzahl der Container bzw. Passagiere bleibt in den Schiffsklassen `private`. Von außen darf
   sie nur gelesen, nicht geändert werden.

## Beispielausgabe (Ausschnitt, normale Daten)

```
Schiffe, sortiert nach Länge:
  #11 Baltic Venture, 92 m lang, 16,2 m breit, 5,4 m Tiefgang, Frachtschiff mit 120 Containern
  ...
  #20 Grand Horizon, 316 m lang, 44,6 m breit, 8,9 m Tiefgang, Passagierschiff mit 3200 Passagieren
Container gesamt: 4010
Passagiere gesamt: 6480
Mitarbeiter:
  #21 Adrian (DockWorker, Expert)
```

Die vollständige Soll-Ausgabe liegt in `test/normale-daten/expected.txt`.

## Hinweise

- Die Übersicht druckt `Harbor.PrintOverview` in `Simulation/Harbor.cs`.
- Die Liste enthält `Ship`-Objekte. Ob ein Schiff in Wirklichkeit ein `CargoShip` ist, prüft man mit
  `is` (Python: `isinstance`). Danach kommt man mit einem Cast an dessen eigene Members.
- `Ships/CargoShip.cs` und `Ships/PassengerShip.cs` brauchen dafür je eine Property, die nur `get` hat.

## Testen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 1-leicht/03-summen-in-uebersicht -Eigene
```
